using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jikji;

namespace JikjiTest
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void ByteArrayToHexString()
        {
            var sbj = new byte[]
            {
                0x00, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F,
                0xAB, 0xCD, 0xEF, 0x1F,
                0x2E, 0x3D, 0x4C, 0x5B,
            };

            var rst = sbj.ToHexString();

            Assert.AreEqual("000102030405060708090A0B0C0D0E0FABCDEF1F2E3D4C5B", rst);
        }

        [TestMethod]
        public void ParseHexStringToByteArray()
        {
            var sbj = "000102030405060708090A0B0C0D0E0FABCDEF1F2E3D4C5B";

            bool isDone = sbj.TryParseFromHex(out byte[] rst);

            Assert.AreEqual(true, isDone);
            CollectionAssert.AreEqual(new byte[]
            {
                0x00, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F,
                0xAB, 0xCD, 0xEF, 0x1F,
                0x2E, 0x3D, 0x4C, 0x5B,
            }, rst);
        }

        [TestMethod]
        public void ParseHexStringWithLowerCaseToByteArray()
        {
            var sbj = "000102030405060708090A0B0C0D0E0FABcdEF1F2E3D4C5B";

            bool isDone = sbj.TryParseFromHex(out byte[] rst);

            Assert.AreEqual(true, isDone);
            CollectionAssert.AreEqual(new byte[]
            {
                0x00, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F,
                0xAB, 0xCD, 0xEF, 0x1F,
                0x2E, 0x3D, 0x4C, 0x5B,
            }, rst);
        }

        [TestMethod]
        public void ParseNonHexStringToByteArray()
        {
            var sbj = "000102030405060708090A0B0C0D0E0FABcdEG1F2E3D4C5B";

            bool isDone = sbj.TryParseFromHex(out byte[] rst);

            Assert.AreEqual(false, isDone);
        }
    }
}
