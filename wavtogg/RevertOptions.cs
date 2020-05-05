using CommandLine;

namespace wavtogg
{
    [Verb("revert", HelpText = "Restore all backed up .wav files in a folder.")]
    public class RevertOptions
    {
        [Option(
            'i',
            "input",
            Required = true,
            HelpText = @"Path to the folder containing the .ogg files. e.g: ""C:\bms\bofxv\song"""
        )]
        public string InputPath { get; set; }

        [Option(
            'r',
            "recursive",
            HelpText = "Allows traversing through the input folder recursively."
        )]
        public bool AllowRecursion { get; set; }

        [Option(
            'o',
            "overwrite",
            HelpText = "Allows overwrite, if a file with the same name (with extension) as the backup already exists in the input folder. Those cases are skipped by default."
        )]
        public bool AllowOverwrite { get; set; }

        [Option(
            'b',
            "backup",
            SetName = "backup",
            Default = Program.DefaultBackupFolderName,
            HelpText = "Backup folder name."
        )]
        public string BackupFolderName { get; set; }

        [Option(
            "preserve-ogg",
            HelpText = "If specified, .ogg files with the same name (without extension) as the backed up .wav will not be deleted after the backup is restored."
        )]
        public bool KeepOggFile { get; set; }

        [Option(
            'l',
            "log",
            HelpText = "Path to the log file to be written to."
        )]
        public string LogFilePath { get; set; }
    }
}
