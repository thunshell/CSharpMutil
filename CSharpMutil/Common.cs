using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil
{
    public static class Common
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

        public static bool Contains(this byte[] parent, byte[] child)
        {
            if (parent.Length <= 0 || child.Length <= 0 || child.Length > parent.Length || !parent.Contains(child[0]))
                return false;
            int index = parent.ToList().IndexOf(child[0]);
            foreach (var item in child)
            {
                if (parent[index++] != item)
                    return false;
            }
            return true;
        }


        public static bool Contains(this List<byte> parent, List<byte> child)
        {
            if (parent.Count <= 0 || child.Count <= 0 || child.Count > parent.Count || !parent.Contains(child[0]))
                return false;
            int index = 0;
            int count;
            do
            {
                index = parent.ToList().IndexOf(child[0], index);
                if(index == -1) return false;
                count = 0;
                foreach (var item in child)
                {
                    if (parent[index++] != item)
                        break;
                    count++;
                }
            } while (count != child.Count);
            return true;
        }
    }
}
