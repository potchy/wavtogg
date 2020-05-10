using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using MediaInfo;

namespace wavtogg
{
    class Program
    {
        internal const string DefaultBackupFolderName = "wavtogg_backup";

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ConvertOptions, RevertOptions>(args)
                .WithParsed<ConvertOptions>(Convert)
                .WithParsed<RevertOptions>(Revert);
        }

        static void SetupLogger(string logFilePath)
        {
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new ConsoleTraceListener());

            if (!string.IsNullOrWhiteSpace(logFilePath))
                Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));
        }

        static void Convert(ConvertOptions options)
        {
            SetupLogger(options.LogFilePath);
            Convert(options.InputPath);

            void Convert(string folder)
            {
                string folderName = Path.GetFileName(folder);
                if (folderName.Equals(options.BackupFolderName, StringComparison.OrdinalIgnoreCase))
                    return;

                foreach (string wavPath in Directory.EnumerateFiles(folder, "*.wav"))
                {
                    Trace.Write($"Converting \"{wavPath}\"... ");

                    if (options.AdpcmOnly)
                    {
                        var mediaInfo = new MediaInfoWrapper(wavPath);

                        if (!string.Equals(mediaInfo.AudioCodec, "ADPCM", StringComparison.OrdinalIgnoreCase))
                        {
                            Trace.WriteLine("Skipped. Not ADPCM encoded.");
                            continue;
                        }
                    }

                    string oggPath = Path.ChangeExtension(wavPath, ".ogg");
                    if (File.Exists(oggPath) && !options.AllowOverwrite)
                    {
                        Trace.WriteLine("Skipped. .ogg already exists.");
                        continue;
                    }

                    Process ffmpeg = Process.Start(new ProcessStartInfo
                    {
                        FileName = options.FfmpegPath,
                        Arguments = $"-i \"{wavPath}\" -codec:a libvorbis -q:a {options.Quality} -y \"{oggPath}\"",
                        UseShellExecute = false,
                        RedirectStandardError = true
                    });

                    string ffmpegOutput = ffmpeg?.StandardError.ReadToEnd();
                    ffmpeg?.WaitForExit();

                    if (ffmpeg?.ExitCode != 0)
                        throw new Exception($"Unexpected ffmpeg output: {ffmpegOutput}");

                    if (!options.DoNotBackup)
                    {
                        string backupFolderPath = Path.Combine(folder, options.BackupFolderName);
                        Directory.CreateDirectory(backupFolderPath);

                        string wavName = Path.GetFileName(wavPath);
                        string wavBackupPath = Path.Combine(backupFolderPath, wavName);

                        File.Move(wavPath, wavBackupPath);
                    }

                    Trace.WriteLine("Done.");
                }

                if (options.AllowRecursion)
                    foreach (string subfolder in Directory.EnumerateDirectories(folder))
                        Convert(subfolder);
            }
        }

        static void Revert(RevertOptions options)
        {
            SetupLogger(options.LogFilePath);
            Revert(options.InputPath);

            void Revert(string folder)
            {
                string backupFolderPath = Path.Combine(folder, options.BackupFolderName);

                if (Directory.Exists(backupFolderPath))
                {
                    foreach (string wavBackupPath in Directory.EnumerateFiles(backupFolderPath, "*.wav"))
                    {
                        Trace.Write($"Restoring \"{wavBackupPath}\"... ");

                        string wavName = Path.GetFileName(wavBackupPath);
                        string wavPath = Path.Combine(folder, wavName);
                        
                        if (File.Exists(wavPath) && !options.AllowOverwrite)
                        {
                            Trace.WriteLine("Skipped. .wav already exists.");
                            continue;
                        }

                        File.Delete(wavPath);
                        File.Move(wavBackupPath, wavPath);

                        if (!options.KeepOggFile)
                        {
                            string oggPath = Path.ChangeExtension(wavPath, ".ogg");
                            File.Delete(oggPath);
                        }   

                        Trace.WriteLine("Done.");
                    }

                    if (!Directory.EnumerateFileSystemEntries(backupFolderPath).Any())
                        Directory.Delete(backupFolderPath);
                }

                if (options.AllowRecursion)
                    foreach (string subfolder in Directory.EnumerateDirectories(folder))
                        Revert(subfolder);
            }
        }
    }
}
