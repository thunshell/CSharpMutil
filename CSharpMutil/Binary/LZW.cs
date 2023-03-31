using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    public class LZW
    {
        public int EarlyChange { get; set; }

        public LZW(int earlyChange = 1)
        {
            EarlyChange = earlyChange;
        }

        public byte[] Encode(byte[] input)
        {
            using (MemoryStream ms = new MemoryStream(input))
            {
                using (MemoryStream ms1 = new MemoryStream())
                {
                    Encode(ms, ms1);
                    return ms1.ToArray();
                }
            }
        }
        public byte[] Decode(byte[] input)
        {
            using (MemoryStream ms = new MemoryStream(input))
            {
                using (MemoryStream ms1 = new MemoryStream())
                {
                    Decode(ms, ms1);
                    return ms1.ToArray();
                }
            }
        }
        public void Encode(Stream source, Stream target)
        {
            source.Seek(0, SeekOrigin.Begin);

            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int dictIndex = 0;
            while (dictIndex <= byte.MaxValue)
                dictionary.Add(((char)dictIndex).ToString(), dictIndex++);

            //256表示clear-table,257表示EOD
            dictIndex++; //258开始
            int bitLength = 9;

            int len = 1;
            int b, v, prev = 0;
            string w = "";
            string wb;

            //开头256
            v = 256;
            Encode(target, v, ref prev, ref len, bitLength);
            StringBuilder sb = new StringBuilder();
            sb.Append(Convert.ToString(v, 2).PadLeft(bitLength, '0'));

            while ((b = source.ReadByte()) > -1)
            {
                wb = w + (char)b;
                if (dictionary.ContainsKey(wb))
                    w = wb;
                else
                {
                    v = dictionary[w];
                    sb.Append(Convert.ToString(v, 2).PadLeft(bitLength, '0'));
                    Encode(target, v, ref prev, ref len, bitLength);
                    w = ((char)b).ToString();

                    //511 1023 2047;
                    //初始9位,每次不足时增1位,最大12位
                    dictionary.Add(wb, ++dictIndex);
                    if (dictIndex == Math.Pow(2, bitLength) - this.EarlyChange)
                    {
                        bitLength++;
                        len++;
                    }
                }
            }

            if (!string.IsNullOrEmpty(w))
            {
                v = dictionary[w];
                sb.Append(Convert.ToString(v, 2).PadLeft(bitLength, '0'));
                Encode(target, v, ref prev, ref len, bitLength);
            }

            //末尾追加EOD
            v = 257;
            sb.Append(Convert.ToString(v, 2).PadLeft(bitLength, '0'));
            Encode(target, v, ref prev, ref len, bitLength);
            if (prev > 0)
                Encode(target, 0, ref prev, ref len, bitLength);

            //string a = sb.ToString();
            //for (int i = 0; i < sb.Length; i += 8)
            //{
            //    string d = a.Substring(i, sb.Length - i < 8 ? sb.Length - i : 8);
            //    int c = Convert.ToInt32(d, 2);
            //    target.WriteByte((byte)c);
            //    Console.Write(c + ",");
            //}
        }

        /// <summary>
        /// 将前一个数值的二进制右len位和当前数值二进制拼接,返回拼接后左8位
        /// </summary>
        /// <param name="target"></param>
        /// <param name="v">当前数值</param>
        /// <param name="prev">前一个数值的二进制右len位</param>
        /// <param name="len"></param>
        /// <param name="bitLength">当前编码长度9,10,11,12</param>
        void Encode(Stream target, int v, ref int prev, ref int len, int bitLength)
        {
            int t = (prev << bitLength) | v;
            int r = t >> len;
            target.WriteByte((byte)r);

            prev = (v & ((int)Math.Pow(2, len) - 1));

            //当len等于8时,右移8位刚好为一个字节
            if (len >= 8)
            {
                int l = len;
                int temp = prev;
                prev = 0;
                len -= 8;
                Encode(target, temp, ref prev, ref len, bitLength);
            }
            else
                len += bitLength - 8;
        }


        /// <summary>
        ///  输入序列    输出序列            添加到表中的代码   用新代码表示的序列
        ///  -	        256（clear-table）	-	            -
        ///  45	        45	                258         	45 45
        ///  45 45	    258         	    259	            45 45 45
        ///  45 45	    258         	    260	            45 45 65
        ///  65	        65	                261         	65 45
        ///  45 45 45   259	                262         	45 45 45 66
        ///  66	        66	                -           	-
        ///  -	        257(EOD)	        -           	-
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Decode(Stream source, Stream target)
        {
            source.Seek(0, SeekOrigin.Begin);

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            int dictIndex = 0;
            while (dictIndex <= byte.MaxValue)
                dictionary.Add(dictIndex, ((char)dictIndex++).ToString());

            //256表示clear-table,257表示EOD
            dictionary.Add(dictIndex++, null);
            dictionary.Add(dictIndex++, null);

            int bitLength = 9;

            string entry = "";
            int startIndex = 0;
            int prev = source.ReadByte();
            int a;
            string s;
            int v;

            while (source.Position < source.Length)
            {
                a = source.ReadByte();
                if(startIndex >= 8)
                {
                    prev = a;
                    startIndex = 0;
                    continue;
                }
                if (a == 134)
                    ;

                s = Convert.ToString(prev, 2).PadLeft(8, '0') + Convert.ToString(a, 2).PadLeft(8, '0');
                if(s.Length - startIndex < bitLength)
                {
                    int b = source.ReadByte();
                    s += Convert.ToString(b, 2).PadLeft(8, '0');
                    a = b;
                }
                v = Convert.ToInt32(s.Substring(startIndex, bitLength), 2);
                Console.Write(v + ",");
                prev = a;
                startIndex = 8 - (s.Length - (startIndex + bitLength));
                if (v == 256) continue;
                if (v == 257) break;
                string w = entry;
                if (dictionary.ContainsKey(v))
                {
                    if (!string.IsNullOrEmpty(entry))
                    {
                        entry += dictionary[v][0];
                        dictionary.Add(dictionary.Count, entry);
                    }
                    entry = dictionary[v];
                }
                else
                {
                    entry += entry[0];
                    dictionary.Add(v, entry);
                }
                //511 1023 2047;
                //初始9位,每次不足时增1位,最大12位
                if (dictionary.Last().Key == Math.Pow(2, bitLength) - (this.EarlyChange + 1))
                    bitLength++;

                foreach (var c in dictionary[v])
                    target.WriteByte((byte)c);

            }
            Console.Write("\r\n");
        }
    }
}
