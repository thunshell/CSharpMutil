using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    public class BitMemory : IDisposable
    {
        public Stream BaseStream { get; private set; }

        public long Position { get; private set; }
        public long Length { get; private set; }

        public BitMemory() : this(new MemoryStream())
        {
        }

        public BitMemory(byte[] bytes) : this(new MemoryStream(bytes))
        {
        }

        public BitMemory(Stream stream)
        {
            BaseStream = stream;
            Position = stream.Position * 8;
            Length = stream.Length * 8;
        }

        public void SetLength(long length)
        {
            Length = length;
        }


        public int ReadByte()
        {
            long p = BaseStream.Position;
            int r = BaseStream.ReadByte();
            if (r == -1) return r;
            Position += (BaseStream.Position - p) * 8;
            return r;
        }

        public int ReadInt()
        {
            byte[] r = new byte[4];
            long p = BaseStream.Position;
            int len = BaseStream.Read(r, 0, 4);
            if (len <= 0) return -1;
            Position += (BaseStream.Position - p) * 8;
            return BitConverter.ToInt32(r.Take(len).ToArray(), 0);
        }

        public int ReadInt(int length)
        {
            if (length >= 32) throw new ArgumentException("Parameter length must small than 32.");

            List<int> list = new List<int>();
            int b = 0;
            for (int i = 0; i < length; i++)
            {
                b = ReadBit();
                if (b == -1) break;
                list.Add(b);
            }

            if (list.Count == 0) return -1;
            int r = Convert.ToInt32(string.Join("", list), 2);
            return r;
        }

        /// <summary>
        /// 读取1位
        /// </summary>
        /// <returns>返回0或1，如果在末端则返回-1</returns>
        public int ReadBit()
        {
            if (Position >= Length) return -1;
            int v = BaseStream.ReadByte();
            if (v == -1) return -1;
            long skip = Position % 8;
            if (skip < 7)
                BaseStream.Position--;
            Position++;
            return (v & (int)Math.Pow(2, 7 - skip)) > 0 ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>读取到的数量</returns>
        public int ReadBits(int[] arr, int offset, int count)
        {
            int i = 0;
            do
            {
                int v = ReadBit();
                if (v < 0) break;
                arr[offset + i] = v;
                i++;
            } while (i < count);
            return i;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">0 or 1</param>
        /// <returns>Ture if successfull.</returns>
        public bool WriteBit(int i)
        {
            if (Position > Length) return false;
            int b = BaseStream.ReadByte();
            long skip = Position % 8;
            int v = i == 1 ? (int)Math.Pow(2, 7 - skip) : 0;
            if (skip == 0)
            {
                BaseStream.WriteByte(Convert.ToByte(v));
                BaseStream.Position--;
            }
            else
            {
                BaseStream.Position--;
                BaseStream.WriteByte(Convert.ToByte(b | v));
                if (skip < 7)
                    BaseStream.Position--;
            }
            if(Position++ == Length)
                Length++;
            return true;
        }

        public void WriteBits(int[] bitArray, int offset, int count)
        {
            if(offset < 0 || count <= 0) return;
            for (int i = offset; i < offset + count; i++)
            {
                WriteBit(bitArray[i]);
            }
        }

        public void WriteBits(byte[] bitArray, int offset, int count)
        {
            WriteBits(bitArray.Select<byte, int>(f => (int)f).ToArray(), offset, count);
        }

        public void Dispose()
        {
            BaseStream?.Dispose();
            Position = 0;
        }

        public void Seek(long position, SeekOrigin seekOrigin)
        {
            long thisP = this.Position;
            long streamP = this.BaseStream.Position;
            switch (seekOrigin)
            {
                case SeekOrigin.Begin:
                    thisP = position;
                    streamP = position / 8;
                    break;
                case SeekOrigin.Current:
                    thisP += position;
                    streamP += position / 8;
                    break;
                case SeekOrigin.End:
                    thisP = this.BaseStream.Length - Math.Abs(position);
                    streamP = this.BaseStream.Length - Math.Abs(position) / 8;
                    break;
                default:
                    break;
            }
            thisP = Math.Max(0, Math.Min(thisP, this.Length * 8));
            streamP = Math.Max(0, Math.Min(streamP, this.BaseStream.Length));

            this.Position = thisP;
            this.BaseStream.Position = streamP;
        }

        public byte[] ToArray()
        {
            Seek(0, SeekOrigin.Begin);
            int i = 0;
            int j = 0;
            byte[] array = new byte[this.Length];
            do
            {
                i = ReadBit();
                if (i <= -1)
                    break;
                array[j++] = (byte)i;
            } while (j < Length);
            return array;
        }
    }
}
