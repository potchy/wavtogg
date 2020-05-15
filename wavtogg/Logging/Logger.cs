using System.Diagnostics;

namespace wavtogg.Logging
{
    public class Logger
    {
        private int Total => _succeeded + _skipped + _failed;
        private int _succeeded;
        private int _skipped;
        private int _failed;

        public Logger(ILoggerOptions options)
        {
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new ConsoleTraceListener());

            if (!string.IsNullOrWhiteSpace(options.LogFilePath))
                Trace.Listeners.Add(new TextWriterTraceListener(options.LogFilePath));
        }

        public void Start(string operation, string fileName)
        {
            Trace.Write($"{operation} \"{fileName}\"... ");
        }

        public void Skip(string reason)
        {
            Trace.WriteLine($"Skipped. {reason}");
            _skipped++;
        }

        public void Success()
        {
            Trace.WriteLine("Done.");
            _succeeded++;
        }

        public void Fail(string message)
        {
            Trace.WriteLine(message);
            _failed++;
        }

        public void Dump()
        {
            Trace.WriteLine(string.Empty);
            Trace.WriteLine($"{Total} files processed. {_succeeded} succeeded. {_skipped} skipped. {_failed} failed.");
        }
    }
}
