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
            dictIndex += 3; //258开始
            int bitLength = 9;

            int len = 1;
            int v, b;
            int r = 0;
            string w = "";
            string entry = "";
            do
            {
                b = source.ReadByte();
                entry = w + (char)b;
                if (dictionary.ContainsKey(entry))
                    w = entry;
                else
                {
                    dictionary.Add(entry, dictIndex++);
                    v = dictionary[w];

                    r = ((r & (int)Math.Pow(2, len - 1) << bitLength) | v) >> len;
                    target.WriteByte((byte)r);

                    w = ((char)b).ToString();
                    len++;
                }
            } while (b > -1);
        }
    }
}
