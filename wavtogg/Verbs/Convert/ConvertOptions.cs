using CommandLine;
using wavtogg.Logging;

namespace wavtogg.Verbs.Convert
{
    [Verb("convert", HelpText = "Converts all .wav files in a folder to .ogg.")]
    public class ConvertOptions : ILoggerOptions
    {
        [Option(
            "ffmpeg",
            Required = true,
            HelpText = @"Path to ffmpeg's executable. e.g: ""C:\ffmpeg\bin\ffmpeg.exe"""
        )]
        public string FfmpegPath { get; set; }

        [Option(
            'q',
            "qscale",
            Default = 5,
            HelpText = "Audio quality. Range from -1 to 10."
        )]
        public int Quality { get; set; }

        [Option(
            'i',
            "input",
            Required = true,
            HelpText = @"Path to the folder containing the .wav files. e.g: ""C:\bms\bofxv\song"""
        )]
        public string InputPath { get; set; }

        [Option(
            "adpcm-only",
            HelpText = "If specified, only ADPCM encoded files will be converted."
        )]
        public bool AdpcmOnly { get; set; }

        [Option(
            'r',
            "recursive",
            HelpText = "Allows traversing through the input folder recursively."
        )]
        public bool AllowRecursion { get; set; }

        [Option(
            'o',
            "overwrite",
            HelpText = "Allows overwrite, if .wav equivalents already exist in the input folder. Those cases are skipped by default."
        )]
        public bool AllowOverwrite { get; set; }

        [Option(
            'b',
            "backup",
            SetName = "backup",
            Default = Program.DefaultBackupFolderName,
            HelpText = "Backup folder name. Folders with this name are automatically excluded from the conversion process."
        )]
        public string BackupFolderName { get; set; }

        [Option(
            "no-backup",
            SetName = "no-backup",
            HelpText = "Disables backup."
        )]
        public bool DoNotBackup { get; set; }

        [Option(
            'l',
            "log",
            HelpText = "Path to the log file to be written to."
        )]
        public string LogFilePath { get; set; }
    }
}