using System;
using System.Diagnostics;
using System.IO;
using MediaInfo;
using wavtogg.Logging;

namespace wavtogg.Verbs.Convert
{
    public class ConvertImpl
    {
        private readonly ConvertOptions _options;

        public ConvertImpl(ConvertOptions options)
        {
            _options = options;
        }

        public void Run()
        {
            var logger = new Logger(_options);
            Convert(_options.InputPath);
            logger.Dump();

            void Convert(string folder)
            {
                string folderName = Path.GetFileName(folder);
                if (folderName.Equals(_options.BackupFolderName, StringComparison.OrdinalIgnoreCase))
                    return;

                foreach (string wavPath in Directory.EnumerateFiles(folder, "*.wav"))
                {
                    logger.Start("Converting", wavPath);

                    if (_options.AdpcmOnly)
                    {
                        var mediaInfo = new MediaInfoWrapper(wavPath);

                        if (!string.Equals(mediaInfo.AudioCodec, "ADPCM", StringComparison.OrdinalIgnoreCase))
                        {
                            logger.Skip("Not ADPCM encoded.");
                            continue;
                        }
                    }

                    string oggPath = Path.ChangeExtension(wavPath, ".ogg");
                    if (File.Exists(oggPath) && !_options.AllowOverwrite)
                    {
                        logger.Skip(".ogg already exists.");
                        continue;
                    }

                    Process ffmpeg = Process.Start(new ProcessStartInfo
                    {
                        FileName = _options.FfmpegPath,
                        Arguments = $"-i \"{wavPath}\" -codec:a libvorbis -q:a {_options.Quality} -y \"{oggPath}\"",
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    });

                    string ffmpegOutput = ffmpeg.StandardError.ReadToEnd();
                    ffmpeg.WaitForExit();

                    if (ffmpeg.ExitCode != 0)
                    {
                        logger.Fail($"Unexpected ffmpeg output: {ffmpegOutput}");
                        continue;
                    }

                    if (!_options.DoNotBackup)
                    {
                        string backupFolderPath = Path.Combine(folder, _options.BackupFolderName);
                        Directory.CreateDirectory(backupFolderPath);

                        string wavName = Path.GetFileName(wavPath);
                        string wavBackupPath = Path.Combine(backupFolderPath, wavName);
                        File.Move(wavPath, wavBackupPath);
                    }

                    logger.Success();
                }

                if (_options.AllowRecursion)
                    foreach (string subfolder in Directory.EnumerateDirectories(folder))
                        Convert(subfolder);
            }
        }
    }
}
