using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    public class LZW
    {
        public static void Encode(Stream source, Stream target)
        {
            source.Seek(0, SeekOrigin.Begin);

            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int dictIndex = 0;
            while (dictIndex <= byte.MaxValue)
                dictionary.Add(((char)dictIndex).ToString(), dictIndex++);

            //256表示clear-table,257表示EOD
            dictIndex += 2; //258开始
            int bitLength = 9;

            int len = 1;
            int b, v, prev = 0;
            string w = "";
            string entry;

            //开头256
            v = 256;
            Encode(target, v, ref prev, ref len, bitLength);

            while ((b = source.ReadByte()) > -1)
            {
                entry = w + (char)b;
                if (dictionary.ContainsKey(entry))
                    w = entry;
                else
                {
                    dictionary.Add(entry, dictIndex++);
                    //511 1023 2047;
                    //初始9位,每次不足时增1位,最大12位
                    if (dictIndex == Math.Pow(2, 9) - 1 || dictIndex == Math.Pow(2, 10) - 1 || dictIndex == Math.Pow(2, 11) - 1)
                        bitLength++;
                    v = dictionary[w];
                    Encode(target, v, ref prev, ref len, bitLength);
                    w = ((char)b).ToString();
                }
            }

            if (!string.IsNullOrEmpty(w))
            {
                v = dictionary[w];
                Encode(target, v, ref prev, ref len, bitLength);
            }

            //末尾追加EOD
            v = 257;
            Encode(target, v, ref prev, ref len, bitLength);
            if (prev > 0)
                target.WriteByte((byte)prev);
        }

        /// <summary>
        /// 将前一个数值的二进制右len位和当前数值二进制拼接,返回拼接后左8位
        /// </summary>
        /// <param name="target"></param>
        /// <param name="v">当前数值</param>
        /// <param name="prev">前一个数值的二进制右len位</param>
        /// <param name="len"></param>
        /// <param name="bitLength">当前编码长度9,10,11,12</param>
        static void Encode(Stream target, int v, ref int prev, ref int len, int bitLength)
        {
            int t = (prev << bitLength) | v;
            int r = t >> len;
            target.WriteByte((byte)r);

            prev = (v & ((int)Math.Pow(2, len) - 1));

            //当len等于8时,右移8位刚好为一个字节
            if (len++ == 8)
            {
                r = t & ((int)Math.Pow(2, 8) - 1);
                target.WriteByte((byte)r);
                len = 1;
                prev = 0;
            }
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
        public static void Decode(Stream source, Stream target)
        {
            source.Seek(0, SeekOrigin.Begin);

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            int dictIndex = 0;
            while (dictIndex <= byte.MaxValue)
                dictionary.Add(dictIndex, ((char)dictIndex++).ToString());


            //256表示clear-table,257表示EOD
            dictIndex += 2; //258开始
            int bitLength = 9;

            //string w = "";
            //while (source.Position < source.Length - 1)
            //{
            //    int k = source.ReadByte();
            //    string entry = null;
            //    if (dictionary.ContainsKey(k))
            //        entry = dictionary[k];
            //    else if (k == dictionary.Count)
            //        entry = w + w[0];

            //    foreach (var c in entry)
            //        target.WriteByte((byte)c);

            //    // new sequence; add it to the dictionary
            //    dictionary.Add(dictionary.Count, w + entry[0]);

            //    w = entry;
            //}

            string entry = "";
            int len = 0;
            int prev = source.ReadByte();
            int a = source.ReadByte();
            string s = Convert.ToString(prev, 2).PadLeft(8, '0') + Convert.ToString(a, 2).PadLeft(8, '0');
            int v = Convert.ToInt32(s.Substring(len, bitLength), 2);
            prev = a;
            len++;

            while (source.Position < source.Length)
            {
                //511 1023 2047;
                //初始9位,每次不足时增1位,最大12位
                if (v == Math.Pow(2, 9) - 1 || v == Math.Pow(2, 10) - 1 || v == Math.Pow(2, 11) - 1)
                    bitLength++;
                a = source.ReadByte();
                if(len >= 8)
                {
                    prev = a;
                    len = 0;
                    continue;
                }

                s = Convert.ToString(prev, 2).PadLeft(8, '0') + Convert.ToString(a, 2).PadLeft(8, '0');
                v = Convert.ToInt32(s.Substring(len, bitLength), 2);
                if (v == 256 || v == 257)
                {
                    continue;
                }
                if (dictionary.ContainsKey(v))
                {
                    if (!string.IsNullOrWhiteSpace(entry))
                    {
                        entry += dictionary[v][0];
                        dictionary.Add(dictionary.Count + 2, entry);
                    }
                    entry = dictionary[v];
                }
                else
                {
                    entry += entry[0];
                    dictionary.Add(v, entry);
                }
                foreach (var c in dictionary[v])
                    target.WriteByte((byte)c);

                prev = a;
                len++;
            }
        }
    }
}
