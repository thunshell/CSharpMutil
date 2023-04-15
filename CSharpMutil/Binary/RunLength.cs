using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    public class RunLength
    {
        const int EOD = 128;

        public static string Decode(string s)
        {
            return Encoding.Default.GetString(Decode(Encoding.Default.GetBytes(s)));
        }

        public static byte[] Decode(byte[] bytes)
        {
            using (MemoryStream source = new MemoryStream(bytes))
            {
                using (MemoryStream target = new MemoryStream())
                {
                    Decode(source, target);
                    return target.ToArray();
                }
            }
        }
        public static void Decode(Stream source, Stream target)
        {
            source.Position = 0;
            int b;
            bool isLength = true;
            int length = 0;
            while ((b = source.ReadByte()) > -1)
            {
                if (b == EOD) break;
                if (isLength)
                    length = b > 128 ? 257 - b : b;
                else
                {
                    for (int i = 0; i <= length; i++)
                        target.WriteByte((byte)b);
                }
                isLength = !isLength;
            }
        }

        public static string Encode(string s)
        {
            return Encoding.Default.GetString(Encode(Encoding.Default.GetBytes(s)));
        }

        public static byte[] Encode(byte[] bytes)
        {
            using (MemoryStream source = new MemoryStream(bytes))
            {
                using (MemoryStream target = new MemoryStream())
                {
                    Encode(source, target);
                    return target.ToArray();
                }
            }
        }

        public static void Encode(Stream source, Stream target)
        {
            source.Position = 0;
            int b = 0;
            int length = 0;
            int repeatB = source.ReadByte();
            while (b > -1)
            {
                b = source.ReadByte();
                if (b != repeatB || length >= 127 || b == -1)
                {
                    target.WriteByte((byte)length);
                    target.WriteByte((byte)(char)repeatB);
                    repeatB = b;
                    length = 0;
                }
                else
                    length++;
            }
            target.WriteByte((byte)128);
        }
    }
}
