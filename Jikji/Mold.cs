using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace Jikji
{
    [Serializable]
    public class Mold
    {
        [NonSerialized]
        private Font _font;
        [NonSerialized]
        private Font _fontb;
        private string _prototype;
        public string Font {
            get {
                return _prototype;
            }
            set {
                _prototype = value;
                _font = new Font(value, Size.Width, FontStyle.Regular, GraphicsUnit.Pixel);
                _fontb = new Font(value, Size.Width, FontStyle.Bold, GraphicsUnit.Pixel);
            }
        }
        public Size Size;
        
        public Rectangle Area;
        [NonSerialized]
        public SolidBrush Brush;
        [NonSerialized]
        public StringFormat StringFormat;
        public SmoothingMode SmoothingMode;
        public StringFormatFlags StringFormatFlags;
        public InterpolationMode InterpolationMode;

        public Mold(in string font, int width, int height) : this(font, new Size(width, height)){}
        public Mold(
            in string font, in Size size,
            SmoothingMode smoothingMode = SmoothingMode.None,
            StringFormatFlags stringFormatFlags = StringFormatFlags.NoClip,
            InterpolationMode interpolationMode = InterpolationMode.Low)
        {
            Size = size;
            Font = font;
            Area = new Rectangle(0, 0, Size.Width, Size.Height);
            Brush = new SolidBrush(Color.White);
            StringFormat = new StringFormat(StringFormatFlags = stringFormatFlags);
            SmoothingMode = smoothingMode;
            InterpolationMode = interpolationMode;
        }

        // AdHoc
        public static bool IsHalf(int unicode)
        {
            // ' ' ~ '~'
            if (0x0019 < unicode && unicode < 0x007F) return true;

            // '₩'
            if (0x20A9 == unicode) return true;

            return false;
        }

        public Bitmap Press(int unicode, Style weight = Style.Regular)
        {
            // https://docs.microsoft.com/ko-kr/dotnet/api/system.drawing.bitmap.-ctor?view=netframework-4.6.2
            var rst = new Bitmap(IsHalf(unicode) ? Size.Width / 2 : Size.Width, Size.Height);

            using (var g = Graphics.FromImage(rst))
            {
                byte[] unicodeBytes = System.BitConverter.GetBytes(unicode);
                string trg = System.Text.Encoding.Unicode.GetString(unicodeBytes);

                var isBold = (weight & Style.Weight) == Style.Bold;
                g.SmoothingMode = SmoothingMode;
                g.PageUnit = GraphicsUnit.Pixel;
                g.InterpolationMode = InterpolationMode;
                g.DrawString(trg, isBold ? _fontb : _font, Brush, Area, StringFormat);
            }

            return rst;
        }

        //
        // Does parallel pressing need?
        // Serial pressing takes under 2 seconds to press about 10,000 Korean characters in 12x12 size on release assembly.
        // The thing is that I think performance is not important because Dict will be reused from exported one.
        //
        // The code below is reserved for future use (if parallel logic is needed).
        // Note that Graphics.DrawString cannot be executed concurrently.
        //
        //public Bitmap[] Press(int from, int to, Style weight = Style.Regular)
        //{
        //    return Press(from, to, weight, new ParallelOptions()
        //    {
        //        MaxDegreeOfParallelism = Environment.ProcessorCount,
        //    });
        //}
        //public Bitmap[] Press(int from, int to, Style weight, ParallelOptions po)
        //{
        //    return _PressSerial(from, to, weight);
        //}
        //private Bitmap[] _PressSerial(int from, int to, Style weight = Style.Regular)
        //{
        //    int size = to - from + 1;
        //    var rst = new Bitmap[size];
        //    for(int i = 0; i < size; i++)
        //    {
        //        rst[i] = Press(i + from, weight);
        //    }
        //    return rst;
        //}

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context) {
            Brush = new SolidBrush(Color.White);
            StringFormat = new StringFormat(StringFormatFlags);
            Font = _prototype;
        }
    }
}
