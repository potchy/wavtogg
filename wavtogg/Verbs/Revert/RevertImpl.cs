using System.IO;
using System.Linq;
using wavtogg.Logging;

namespace wavtogg.Verbs.Revert
{
    public class RevertImpl
    {
        private readonly RevertOptions _options;

        public RevertImpl(RevertOptions options)
        {
            _options = options;
        }

        public void Run()
        {
            var logger = new Logger(_options);
            Revert(_options.InputPath);
            logger.Dump();

            void Revert(string folder)
            {
                string backupFolderPath = Path.Combine(folder, _options.BackupFolderName);

                if (Directory.Exists(backupFolderPath))
                {
                    foreach (string wavBackupPath in Directory.EnumerateFiles(backupFolderPath, "*.wav"))
                    {
                        logger.Start("Restoring", wavBackupPath);

                        string wavName = Path.GetFileName(wavBackupPath);
                        string wavPath = Path.Combine(folder, wavName);

                        if (File.Exists(wavPath) && !_options.AllowOverwrite)
                        {
                            logger.Skip(".wav already exists.");
                            continue;
                        }

                        File.Delete(wavPath);
                        File.Move(wavBackupPath, wavPath);

                        if (!_options.KeepOggFile)
                        {
                            string oggPath = Path.ChangeExtension(wavPath, ".ogg");
                            File.Delete(oggPath);
                        }

                        logger.Success();
                    }

                    if (!Directory.EnumerateFileSystemEntries(backupFolderPath).Any())
                        Directory.Delete(backupFolderPath);
                }

                if (_options.AllowRecursion)
                    foreach (string subfolder in Directory.EnumerateDirectories(folder))
                        Revert(subfolder);
            }
        }
    }
}
