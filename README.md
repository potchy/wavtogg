# wavtogg

.wav to .ogg bulk converter with encoding filters and backup support. Requires FFmpeg. Windows only.

## convert

### options:
```
--ffmpeg           Required. Path to ffmpeg's executable. e.g: "C:\ffmpeg\bin\ffmpeg.exe"

-q, --qscale       (Default: 5) Audio quality. Range from -1 to 10.

-i, --input        Required. Path to the folder containing the .wav files. e.g: "C:\bms\bofxv\song"

--adpcm-only       If specified, only ADPCM encoded files will be converted.

-r, --recursive    Allows traversing through the input folder recursively.

-o, --overwrite    Allows overwrite, if .ogg equivalents already exist in the input folder. Those cases are skipped by default.

-b, --backup       (Default: wavtogg_backup) Backup folder name. Folders with this name are automatically excluded from the conversion process.

--no-backup        Disables backup. Warning: this will DELETE your original .wav files after the conversion to .ogg is completed. Recovery will be impossible afterwards.

-l, --log          Path to the log file to be written to.

--help             Display this help screen.

--version          Display version information.
```

### usage example:
`wavtogg convert --ffmpeg "C:\ffmpeg\bin\ffmpeg.exe" -i "C:\bms\bofxv" -r -l log.txt`

## revert

### options
```
-i, --input        Required. Path to the folder containing the .ogg files. e.g: "C:\bms\bofxv\song"

-r, --recursive    Allows traversing through the input folder recursively.

-o, --overwrite    Allows overwrite, if a file with the same name (with extension) as the backup already exists in the input folder. Those cases are skipped by default.

-b, --backup       (Default: wavtogg_backup) Backup folder name.

--preserve-ogg     If specified, .ogg files with the same name (without extension) as the backed up .wav will not be deleted after the backup is restored.

-l, --log          Path to the log file to be written to.

--help             Display this help screen.

--version          Display version information.
```

### usage example:
`wavtogg revert -i "C:\bms\bofxv" -r -l log.txt`
