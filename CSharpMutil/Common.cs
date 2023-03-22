using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil
{
    public class Common
    {
        public static List<byte> WhiteSpaces = new List<byte>(new byte[] { 0, 9, 10, 12, 13, 32 });

        /// <summary>
        /// 读取一个字符,如果是空白字符则继续读取,直到结束或非空白字符
        /// </summary>
        /// <param name="s">被读取的字符串</param>
        /// <param name="position">开始读取位置</param>
        /// <returns></returns>
        public static char SkipIfWhiteSpace(string s, int position)
        {
            char r = '\0';
            while (position < s.Length && WhiteSpaces.Contains((byte)r))
                r = s[position];
            return r;
        }

        /// <summary>
        /// 读取一个字节,如果是空白字符则继续读取,直到结束或非空白字符
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static int SkipIfWhiteSpace(Stream encoded)
        {
            int r = 0;
            while (r > -1 && WhiteSpaces.Contains((byte)r))
                r = encoded.ReadByte();
            return r;
        }
    }
}
