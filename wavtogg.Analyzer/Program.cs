using CsvHelper;
using MediaInfo;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace wavtogg.Analyzer
{
    // this is just a throwaway program I used to collect the sample data.
    // I figure it might come in handy again someday.

    class Program
    {
        static readonly byte[] fmt = { 0x66, 0x6d, 0x74 };

        static void Main(string[] args)
        {
            string folder = args[0];

            using (var fileStream = File.Create(@"csv.csv"))
            using (var streamWriter = new StreamWriter(fileStream, Encoding.Unicode))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteField("Path");
                csvWriter.WriteField("Format");
                csvWriter.WriteField("BytesAt0xc");
                csvWriter.WriteField("BytesAt0xcSequenceEqual0x66-0x6d-0x74");
                csvWriter.WriteField("ByteAt0x14");
                csvWriter.WriteField("ByteAt0x14EqualsTo2or17");
                csvWriter.WriteField("Match");
                csvWriter.NextRecord();

                foreach (string wavPath in Directory.EnumerateFiles(folder, "*.wav", SearchOption.AllDirectories))
                {
                    // Path
                    csvWriter.WriteField(wavPath);

                    // Format
                    string format = new MediaInfoWrapper(wavPath).AudioCodec;
                    csvWriter.WriteField(format);

                    using (FileStream fs = File.OpenRead(wavPath))
                    {
                        // BytesAt0xc
                        fs.Seek(0xc, SeekOrigin.Begin);
                        var bytesAt0x0c = new byte[3];
                        fs.Read(bytesAt0x0c, 0, bytesAt0x0c.Length);

                        csvWriter.WriteField(BitConverter.ToString(bytesAt0x0c));

                        // BytesAt0x0SequenceEqual0x66-0x6d-0x74
                        bool bytesAt0xcSequenceEqual0x66_0x6d_0x74 = bytesAt0x0c.SequenceEqual(fmt);
                        csvWriter.WriteField(bytesAt0xcSequenceEqual0x66_0x6d_0x74 ? "YES" : "NO");

                        // ByteAt0x14
                        fs.Seek(0x14, SeekOrigin.Begin);
                        int byteAt0x14 = fs.ReadByte();

                        csvWriter.WriteField(byteAt0x14);

                        // ByteAt0x14EqualsTo2or17
                        bool byteAt0x14EqualsTo2or17 = new[] { 2, 17 }.Contains(byteAt0x14);
                        csvWriter.WriteField(byteAt0x14EqualsTo2or17 ? "YES" : "NO");

                        // Match
                        bool match = "ADPCM".Equals(format, StringComparison.OrdinalIgnoreCase) == (bytesAt0xcSequenceEqual0x66_0x6d_0x74 && byteAt0x14EqualsTo2or17);
                        csvWriter.WriteField(match ? "SUCCESS" : "FAIL");
                    }

                    csvWriter.NextRecord();
                    streamWriter.Flush();
                }
            }
        }
    }
}