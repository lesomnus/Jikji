using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jikji;
using System;

namespace JikjiTest
{
    [TestClass]
    public class ApplicationTest
    {
        static public Dict ASCIIDict;
        static public Dict KoreanDict;

        [ClassInitialize]
        static public void Initialize(TestContext context)
        {
            {
                var mold = new Mold("굴림체", 12, 12);
                mold.Area.X -= 3;
                var printer = new Printer(mold);

                // 기본 라틴
                // Basic Latin: 0021 ~ 007E
                // 0000~0020, and 007F are control charactor.
                printer
                    .Add(0x0021, 0x007E)

                    // 원 기호
                    // WON SIGN: 0x20A9
                    .Add(0x20A9);

                ASCIIDict = printer.Print();
            }

            // where is the OrderedTest???
            //PressKorean();
            //ExportKorean();
            {
                var mold = new Mold("굴림체", 12, 12);
                mold.Area.X -= 3;
                var printer = new Printer(mold);

                printer
                    // 한글 완성형
                    // Hangul Syllables: AC00 ~ D7AF
                    // D7A4~D7AF are invalid charactor.
                    .Add(0xAC00, 0xD7A3)

                    // 한글 자모
                    // Hangul Compatibility Jamo: 3130 ~ 318F
                    // 3130 is invalid charactor.
                    // 3164~318F are not commonly used.
                    .Add(0x3131, 0x3163);

                KoreanDict = printer.Print();

                var isDone = KoreanDict.TryExportTo(@"KoreanDict.dat");
            }
        }

        [TestMethod]
        public void PressBookAndFind()
        {
            {
                var rasterA = ASCIIDict.Mold.Press('A');

                var isFound = ASCIIDict.TryFind(rasterA, out int value);
                Assert.IsTrue(isFound, "Pressing fault");
                Assert.AreEqual('A', (char)value);
            }

            {
                var rasterA = ASCIIDict.Mold.Press('A', Style.Bold);

                var isFound = ASCIIDict.TryFind(rasterA, out int value);
                Assert.IsTrue(isFound, "Pressing fault");
                Assert.AreEqual('A', (char)value);
            }
        }

        [TestMethod]
        public void PressKorean()
        {
            var mold = new Mold("굴림체", 12, 12);
            mold.Area.X -= 3;
            var printer = new Printer(mold);

            printer
            // 한글 완성형
            // Hangul Syllables: AC00 ~ D7AF
            // D7A4~D7AF are invalid charactor.
                .Add(0xAC00, 0xD7A3)

            // 한글 자모
            // Hangul Compatibility Jamo: 3130 ~ 318F
            // 3130 is invalid charactor.
            // 3164~318F are not commonly used.
                .Add(0x3131, 0x3163);

            KoreanDict = printer.Print();
        }

        [TestMethod]
        public void ExportKorean()
        {
            var isDone = KoreanDict.TryExportTo(@"KoreanDict.dat");
            Assert.IsTrue(isDone, "Failed to export");
        }

        [TestMethod]
        public void ImportKorean()
        {
            var isDone = Dict.TryImportFrom(@"KoreanDict.dat", out Dict __unused);
            Assert.IsTrue(isDone, "Failed to export");
        }
        
#if DEBUG
        [TestMethod]
        public void DrawTest()
        {
            var raster = ASCIIDict.Mold.Press('A');
            raster.DrawToStream(Console.Out);

            raster = ASCIIDict.Mold.Press('각');
            raster.DrawToStream(Console.Out);

            raster = ASCIIDict.Mold.Press('뷁');
            raster.DrawToStream(Console.Out);

            raster = ASCIIDict.Mold.Press('A', Style.Bold);
            raster.DrawToStream(Console.Out);

            raster = ASCIIDict.Mold.Press('각', Style.Bold);
            raster.DrawToStream(Console.Out);

            raster = ASCIIDict.Mold.Press('뷁', Style.Bold);
            raster.DrawToStream(Console.Out);
        }
#endif

        [TestMethod]
        public void ExportAndImportTest()
        {
            var path = @"ASCIIDict.dat";
            var isDone = ASCIIDict.TryExportTo(path);
            Assert.IsTrue(isDone, "Failed to export");

            isDone = Dict.TryImportFrom(path, out Dict imported);
            Assert.IsTrue(isDone, "Failed to import");


            {
                var rasterA = ASCIIDict.Mold.Press('A');
                var isFound = imported.TryFind(rasterA, out int value);
                Assert.IsTrue(isFound);
                Assert.AreEqual('A', (char)value, "Import fault");
            }

            {
                var rasterA = imported.Mold.Press('A');
                var isFound = ASCIIDict.TryFind(rasterA, out int value);
                Assert.IsTrue(isFound);
                Assert.AreEqual('A', (char)value, "Import fault");
            }
        }

        [ClassCleanup]
        static public void Cleanup()
        {
            System.IO.File.Delete(@"ASCIIDict.dat");
            System.IO.File.Delete(@"KoreanDict.dat");
        }
    }
}
