using System.IO;
using System.Linq;

namespace wavtogg.Utils
{
    public static class WavUtils
    {
        // reference:
        // http://soundfile.sapp.org/doc/WaveFormat/
        // https://mh-nexus.de/en/hxd/
        private const byte FormatChunkOffset = 0xc;
        private static readonly byte[] FormatChunkExpectedContent = { 0x66, 0x6d, 0x74 };

        // https://github.com/tpn/winddk-8.1/blob/f6e6e4da7d1894536cf1fa774911df1218ef912e/Include/shared/mmreg.h#L2393
        private const int WFormatTagOffset = 0x14;

        // https://github.com/tpn/winddk-8.1/blob/f6e6e4da7d1894536cf1fa774911df1218ef912e/Include/shared/mmreg.h#L2109
        private const int WAVE_FORMAT_ADPCM = 0x0002;
        private const int WAVE_FORMAT_DVI_ADPCM = 0x0011; // 17
        private static readonly int[] WFormatTagExpectedContent = {WAVE_FORMAT_ADPCM, WAVE_FORMAT_DVI_ADPCM};

        /// <remarks>
        /// I actually don't know much about WAV headers, so I'm sure this algorithm is very very naive.
        ///
        /// I ran it across a sample size of 1.347.475 files, compared its results to MediaInfo's
        /// but I didn't get not even 1 false positive/negative, so I think it suits the purposes of this application pretty well.
        /// You can find the sample data in the test project.
        /// </remarks>
        public static bool IsAdpcm(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // format chunk check
                fs.Seek(FormatChunkOffset, SeekOrigin.Begin);
                var formatChunkContent = new byte[FormatChunkExpectedContent.Length];
                fs.Read(formatChunkContent, 0, formatChunkContent.Length);

                if (!formatChunkContent.SequenceEqual(FormatChunkExpectedContent))
                    return false;

                // wFormatTag check
                fs.Seek(WFormatTagOffset, SeekOrigin.Begin);
                int wFormatTagContent = fs.ReadByte();

                return WFormatTagExpectedContent.Contains(wFormatTagContent);
            }
        }
    }
}
