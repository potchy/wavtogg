using CommandLine;
using wavtogg.Verbs.Convert;
using wavtogg.Verbs.Revert;

namespace wavtogg
{
    class Program
    {
        internal const string DefaultBackupFolderName = "wavtogg_backup";

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ConvertOptions, RevertOptions>(args)
                .WithParsed<ConvertOptions>(options => new ConvertImpl(options).Run())
                .WithParsed<RevertOptions>(options => new RevertImpl(options).Run());
        }
    }
}
