using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using D = System.Diagnostics.Debug;

[assembly: InternalsVisibleTo("JikjiTest")]
namespace Jikji
{
    internal static class Util
    {
        static public byte[] ToByteArray(this Bitmap image)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                             ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = data.Scan0;

            int length = Math.Abs(data.Stride) * image.Height;
            byte[] rst = new byte[length];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rst, 0, length);
            image.UnlockBits(data);

            return rst;
        }

        static public byte[] ToValidMap(this Bitmap image)
        {
            if (image.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                return image.ToByteArray();
            }

            //TODO handle 4bppIndexed

            var pixels = image.Width * image.Height;
            var values = image.ToByteArray();
            var step = Image.GetPixelFormatSize(image.PixelFormat) / 8;

            D.Assert(values.Length == image.Width * image.Height * step);

            var rst = new byte[(pixels / 8)
                            + ((pixels % 8) > 0 ? 1 : 0)];

            for (int pixel = 0; pixel < pixels; pixel ++)
            {
                bool isValid = false;

                for (int offset = 0; offset < step; offset++)
                {
                    var idx = pixel * step + offset;

                    if (values[idx] == 0) continue;

                    isValid = true;
                    break;
                }

                if (!isValid) continue;

                rst[pixel / 8] |= (byte)(0b1 << (7 - (pixel % 8)));
            }

            return rst;
        }

#if DEBUG
        static public void DrawToStream(this Bitmap image, TextWriter o)
        {
            var data = image.ToValidMap();
            var sb = new System.Text.StringBuilder(image.Width * image.Height);

            foreach(var d in data)
            {
                sb.Append(Convert.ToString(d, 2).PadLeft(8, '0'));
            }

            var binStr = sb.ToString();
            
            for(int i = 0; i < binStr.Length; i++)
            {
                if (i % image.Width == 0) o.Write(Environment.NewLine);
                o.Write(binStr[i] == '0' ? '□' : '■');
            }
        }
#endif
        
        static private readonly uint[] _lookup32 = new Func<uint[]>(() =>
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        })();
        
        static public string ToHexString(this byte[] values)
        {
            var lookup32 = _lookup32;
            var result = new char[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                var val = lookup32[values[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        static public bool TryParseFromHex(this string hexString, out byte[] rst)
        {
            bool TryParseToByte(int unicode, out byte val)
            {
                val = 0;

                // 0~9
                if (0x2F < unicode && unicode < 0x3A)
                {
                    val = (byte)(unicode - 48);
                    return true;
                }
                // A~F
                if (0x40 < unicode && unicode < 0x47)
                {
                    val = (byte)(unicode - 65 + 10);
                    return true;
                }
                // a~f
                if (0x60 < unicode && unicode < 0x67)
                {
                    val = (byte)(unicode - 97 + 10);
                    return true;
                }

                return false;
            };

            if (hexString.Length % 2 == 1)
            {
                rst = new byte[0];
                return false;
            }

            rst = new byte[hexString.Length / 2];

            for (int i = 0; i < rst.Length; i++)
            {
                if (!(TryParseToByte(hexString[i * 2], out byte h)
                   && TryParseToByte(hexString[(i * 2) + 1], out byte l))){
                    return false;
                }

                rst[i] = (byte)(h << 4 | l);
            }

            return true;
        }
    }
}
