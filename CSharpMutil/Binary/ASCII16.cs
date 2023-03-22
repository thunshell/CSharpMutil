using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    /// <summary>
    /// ASCII16编码:
    ///     将1个字节(X)转为16进制的两个长度的字符串(yy),再把yy以为两个字节返回.
    ///     例如对字符串AP编码:
    ///         A的16进制为41,得到4和1两个字节
    ///         P的16进制为50,得到5和0两个字节
    ///         编码结果为字符串4150
    /// </summary>
    public class ASCII16
    {
        public static string Encode(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in data)
            {
                builder.AppendFormat("{0:X2}", b);
            }
            return builder.ToString();
        }

        public static byte[] Decode(string encoded)
        {
            //编码末尾可能有结束符>
            if (encoded.Length % 2 == 1 && encoded.Last() == '>')
                encoded.TrimEnd('>');
            //忽略空白字符
            foreach (var item in Common.WhiteSpaces)
                encoded.Replace((char)item, '\0');

            byte[] data = new byte[encoded.Length / 2];

            for (int i = 0; i < encoded.Length; i += 2)
            {
                string hexByte = encoded.Substring(i, 2);
                //偶数位出现>代表0
                if (hexByte[1] == '>')
                    hexByte = "0" + hexByte[0];
                byte b;
                if (!byte.TryParse(hexByte, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
                {
                    throw new ArgumentException("Invalid encoded string: contains non-hexadecimal characters.");
                }
                data[i / 2] = b;
            }

            return data;
        }
    }
}
