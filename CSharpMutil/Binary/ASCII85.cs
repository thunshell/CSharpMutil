using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    /*
     * ASCII base 85
     * 编码:
     *     将需要编码的数据按每4个字节(每个字节8位)一组(b1,b2,b3,b4)(不足4个的用\0补充)转成5个字节(c1,c2,c3,c4,c5),满足等式:
     *         b1 * 256^3 + b2 * 256^2 + b3 * 256 + b4 = c1 * 85^4 + c2 * 85^3 + c3 * 85^2 + c4 * 85 + c5
     *     转后的每个字节再加33;
     *     编码后的每个字节总是在!(33)到u(117)之间或是z(122),编码后在末尾追加~>结束标识;
     *     如果4个字节都是0,则转成一个z(122);
     *     如果有n(1,2,3)个不足4个字节的补足(4-n)个\0到4个字节,转后的5个字节取前n+1个字节;
     *     例如如字符串"A",只有1个字节,则n=1,追加4-n=3个\0,转后的字节取前n+1=2个编码后结果是"5l~>".
     * 类似256进制转换为85进制
     */
    public class ASCII85
    {
        public static byte[] Encode(byte[] bs)
        {
            using (MemoryStream source = new MemoryStream(bs))
            {
                using (MemoryStream target = new MemoryStream())
                {
                    Encode(source, target);
                    return target.ToArray();
                }
            }
        }

        public static byte[] Decode(byte[] bs)
        {
            using (MemoryStream source = new MemoryStream(bs))
            {
                using (MemoryStream target = new MemoryStream())
                {
                    Decode(source, target);
                    return target.ToArray();
                }
            }
        }

        public static string Encode(string str)
        {
            byte[] bs = Encode(Encoding.UTF8.GetBytes(str));
            return Encoding.UTF8.GetString(bs);
        }
        public static string Decode(string str)
        {
            byte[] bs = Decode(Encoding.UTF8.GetBytes(str));
            return Encoding.UTF8.GetString(bs);
        }

        public static void Decode(Stream encoded, Stream decoded)
        {
            bool trimStart = false, trimEnd = false;
            encoded.Position = 0;
            if (encoded.ReadByte() == (byte)'<' && encoded.ReadByte() == (byte)'~')
                trimStart = true;
            encoded.Position = encoded.Length - 2;
            if (encoded.ReadByte() == (byte)'~' && encoded.ReadByte() == (byte)'>')
                trimEnd = true;
            encoded.Position = trimStart ? 2 : 0;
            long length = trimEnd ? encoded.Length - 2 : encoded.Length;

            byte[] bs;
            uint[] bs1;
            int t, len;
            while (encoded.Position < length)
            {
                if (encoded.ReadByte() == 122)
                {
                    decoded.Write(new byte[4], 0, 4);
                    continue;
                }

                len = 0;
                bs1 = new uint[5];
                encoded.Position--;
                for (int i = 0; i < 5; i++)
                {
                    t = Common.SkipIfWhiteSpace(encoded);
                    if (t < 0)
                        bs1[i] = 84;
                    else
                    {
                        bs1[i] = (uint)t - 33;
                        len++;
                    }
                }

                uint value = bs1[0] * 85 * 85 * 85 * 85 +
                            bs1[1] * 85 * 85 * 85 +
                            bs1[2] * 85 * 85 +
                            bs1[3] * 85 +
                            bs1[4];
                bs = BitConverter.GetBytes(value).Reverse().Take(len - 1).ToArray();
                decoded.Write(bs, 0, bs.Count());
            }
        }

        public static void Encode(Stream source, Stream target)
        {
            source.Position = 0;
            target.Position = 0;

            byte[] c = new byte[5];
            int len = 0, b1, b2, b3, b4;
            while (source.Position < source.Length)
            {
                b1 = source.ReadByte();
                b2 = source.ReadByte();
                b3 = source.ReadByte();
                b4 = source.ReadByte();

                len = 1;
                if (b2 > -1) len++; else b2 = 0;
                if (b3 > -1) len++; else b3 = 0;
                if (b4 > -1) len++; else b4 = 0;

                if ((b1 | b2 | b3 | b4) == 0)
                {
                    target.WriteByte((byte)122);
                    continue;
                }


                uint a = (uint)((b1 << 24) | (b2 << 16) | (b3 << 8) | (b4));
                uint b;
                for (int i = 0; i < 5; i++)
                {
                    b = a % 85;
                    a = a / 85;
                    c[4 - i] = ((byte)(b + 33));
                }
                for (int i = 0; i < len + 1; i++)
                    target.WriteByte(c[i]);
            }
        }
    }
}
