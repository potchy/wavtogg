using NUnit.Framework;
using wavtogg.Utils;

namespace wavtogg.Tests
{
    [TestFixture]
    public class WavUtilsTests
    {
        [TestCase(@"Samples\beatmania IIDX.IIDX19 Lincle.Kagachi.02.wav", true)]
        [TestCase(@"Samples\BeMusicSeeker difficulty tables BMS PACK.Ende.SFK.WAV", true)]
        [TestCase(@"Samples\BeMusicSeeker difficulty tables BMS PACK.Kern Typhoon.Bass 01.wav", true)]
        [TestCase(@"Samples\beatmania IIDX.IIDX23 copula.STARLIGHT DANCEHALL.008V.wav", false)]
        [TestCase(@"Samples\BeMusicSeeker difficulty tables BMS PACK.Coexistence.bgm3.wav", false)]
        public void IsAdpcm(string path, bool expectedResult)
        {
            bool actualResult = WavUtils.IsAdpcm(path);
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
