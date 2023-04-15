using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Binary
{
    public class CCITTFax
    {
        public static readonly byte[] EOL = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        public static List<byte[]> RTC = new List<byte[]> { EOL, EOL, EOL, EOL, EOL, EOL };
        public static readonly List<byte[]> WhiteTerminatingCodes = new List<byte[]>
        {
            new byte[]{ 0,0,1,1,0,1,0 }, //0
            new byte[]{ 0,0,0,1,1,1 },//1
            new byte[]{ 0,1,1,1 },//2
            new byte[]{ 1,0,0,0 },//3
            new byte[]{ 1,0,1,1 },//4
            new byte[]{ 1,1,0,0 },
            new byte[]{ 1,1,1,0 },
            new byte[]{ 1,1,1,1 },
            new byte[]{ 1,0,0,1,1 },
            new byte[]{ 1,0,1,0,0 },
            new byte[]{ 0,0,1,1,1 },//10
            new byte[]{ 0,1,0,0,0 },
            new byte[]{ 0,0,1,0,0,0 },
            new byte[]{ 0,0,0,0,1,1 },
            new byte[]{ 1,1,0,1,0,0 },
            new byte[]{ 1,1,0,1,0,1 },//15
            new byte[]{ 1,0,1,0,1,0 },
            new byte[]{ 1,0,1,0,1,1 },
            new byte[]{ 0,1,0,0,1,1,1 },
            new byte[]{ 0,0,0,1,1,0,0 },
            new byte[]{ 0,0,0,1,0,0,0 },//20
            new byte[]{ 0,0,1,0,1,1,1 },
            new byte[]{ 0,0,0,0,0,1,1 },
            new byte[]{ 0,0,0,0,1,0,0 },
            new byte[]{ 0,1,0,1,0,0,0 },
            new byte[]{ 0,1,0,1,0,1,1 },//25
            new byte[]{ 0,0,1,0,0,1,1 },
            new byte[]{ 0,1,0,0,1,0,0 },
            new byte[]{ 0,0,1,1,0,0,0 },
            new byte[]{ 0,0,0,0,0,0,1,0 },
            new byte[]{ 0,0,0,0,0,0,1,1 },//30
            new byte[]{ 0,0,0,1,1,0,1,0 },
            new byte[]{ 0,0,0,1,1,0,1,1 },
            new byte[]{ 0,0,0,1,0,0,1,0 },
            new byte[]{ 0,0,0,1,0,0,1,1 },
            new byte[]{ 0,0,0,1,0,1,0,0 },//35
            new byte[]{ 0,0,0,1,0,1,0,1 },
            new byte[]{ 0,0,0,1,0,1,1,0 },
            new byte[]{ 0,0,0,1,0,1,1,1 },
            new byte[]{ 0,0,1,0,1,0,0,0 },
            new byte[]{ 0,0,1,0,1,0,0,1 },//40
            new byte[]{ 0,0,1,0,1,0,1,0 },
            new byte[]{ 0,0,1,0,1,0,1,1 },
            new byte[]{ 0,0,1,0,1,1,0,0 },
            new byte[]{ 0,0,1,0,1,1,0,1 },
            new byte[]{ 0,0,0,0,0,1,0,0 },//45
            new byte[]{ 0,0,0,0,0,1,0,1 },
            new byte[]{ 0,0,0,0,1,0,1,0 },
            new byte[]{ 0,0,0,0,1,0,1,1 },
            new byte[]{ 0,1,0,1,0,0,1,0 },
            new byte[]{ 0,1,0,1,0,0,1,1 },//50
            new byte[]{ 0,1,0,1,0,1,0,0 },
            new byte[]{ 0,1,0,1,0,1,0,1 },
            new byte[]{ 0,0,1,0,0,1,0,0 },
            new byte[]{ 0,0,1,0,0,1,0,1 },
            new byte[]{ 0,1,0,1,1,0,0,0 },//55
            new byte[]{ 0,1,0,1,1,0,0,1 },
            new byte[]{ 0,1,0,1,1,0,1,0 },
            new byte[]{ 0,1,0,1,1,0,1,1 },
            new byte[]{ 0,1,0,0,1,0,1,0 },
            new byte[]{ 0,1,0,0,1,0,1,1 },//60
            new byte[]{ 0,0,1,1,0,0,1,0 },
            new byte[]{ 0,0,1,1,0,0,1,1 },
            new byte[]{ 0,0,1,1,0,1,0,0 },
        };

        public static readonly List<byte[]> BlackTerminatingCodes = new List<byte[]>
        {
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 1, 1 },
            new byte[]{ 0, 1,0 },
            new byte[]{ 1, 1 },
            new byte[]{ 1, 0 },
            new byte[]{ 0, 1, 1 },
            new byte[]{ 0, 0, 1, 1 }, //5
            new byte[]{ 0, 0, 1, 0 },
            new byte[]{ 0, 0, 0, 1, 1 },
            new byte[]{ 0, 0, 0, 1, 0, 1 },
            new byte[]{ 0, 0, 0, 1, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 0, 0 }, //10
            new byte[]{ 0, 0, 0, 0, 1, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 0 },//15
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1},
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0},//20
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0},
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1},
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0},
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1},
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },//25
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0 },//30
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1 },//35
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0 },//40
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1 },//45
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0 },//50
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1 },//55
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
            new byte[]{ 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0 },//60
            new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0 },
            new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1 }
        };

        public static readonly Dictionary<int, byte[]> WhiteMakeUpCodes = new Dictionary<int, byte[]>
        {
            { 64,   new byte[]{ 1, 1, 0, 1, 1 } }, //64
            { 128,  new byte[]{ 1, 0, 0, 1, 0 } },//128
            { 192,  new byte[]{ 0, 1, 0, 1, 1, 1 } },//192
            { 256,  new byte[]{ 0, 1, 1, 0, 1, 1, 1 } },//256
            { 320,  new byte[]{ 0, 0, 1, 1, 0, 1, 1, 0 } },//320
            { 384,  new byte[]{ 0, 0, 1, 1, 0, 1, 1, 1 } },//384
            { 448,  new byte[]{ 0, 1, 1, 0, 0, 1, 0, 0 } },//448
            { 512,  new byte[]{ 0, 1, 1, 0, 0, 1, 0, 1 } },//512
            { 576,  new byte[]{ 0, 1, 1, 0, 1, 0, 0, 0 } },//576
            { 640,  new byte[]{ 0, 1, 1, 0, 0, 1, 1, 1 } },//640
            { 704,  new byte[]{ 0, 1, 1, 0, 0, 1, 1, 0, 0 } },//704
            { 768,  new byte[]{ 0, 1, 1, 0, 0, 1, 1, 0, 1 } },//768
            { 832,  new byte[]{ 0, 1, 1, 0, 1, 0, 0, 1, 0 } },//832
            { 896,  new byte[]{ 0, 1, 1, 0, 1, 0, 0, 1, 1 } },//896
            { 960,  new byte[]{ 0, 1, 1, 0, 1, 0, 1, 0, 0 } },//960
            { 1024, new byte[]{ 0, 1, 1, 0, 1, 0, 1, 0, 1 } },//1024
            { 1088, new byte[]{ 0, 1, 1, 0, 1, 0, 1, 1, 0 } },//1088
            { 1152, new byte[]{ 0, 1, 1, 0, 1, 0, 1, 1, 1 } },//1152
            { 1216, new byte[]{ 0, 1, 1, 0, 1, 1, 0, 0, 0 } },//1216
            { 1280, new byte[]{ 0, 1, 1, 0, 1, 1, 0, 0, 1 } },//1280
            { 1344, new byte[]{ 0, 1, 1, 0, 1, 1, 0, 1, 0 } },//1344
            { 1408, new byte[]{ 0, 1, 1, 0, 1, 1, 0, 1, 1 } },//1408
            { 1472, new byte[]{ 0, 1, 0, 0, 1, 1, 0, 0, 0 } },//1472
            { 1536, new byte[]{ 0, 1, 0, 0, 1, 1, 0, 0, 1 } },//1536
            { 1600, new byte[]{ 0, 1, 0, 0, 1, 1, 0, 1, 0 } },//1600
            { 1664, new byte[]{ 0, 1, 1, 0, 0, 0 } }, //1664
            { 1728, new byte[]{ 0, 1, 0, 0, 1, 1, 0, 1, 1 } },//1728
            //EOL
        };

        public static readonly Dictionary<int, byte[]> BlackMakeupCodes = new Dictionary<int, byte[]>
        {
            { 64,   new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 } },
            { 128,  new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0 } },
            { 192,  new byte[]{ 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1 } },
            { 256,  new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1 } },
            { 320,  new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1 } },
            { 384,  new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0 } },
            { 448,  new byte[]{ 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1 } },
            { 512,  new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0 } },
            { 576,  new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1 } },
            { 640,  new byte[]{ 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0 } },
            { 704,  new byte[]{ 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1 } },
            { 768,  new byte[]{ 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0 } },
            { 832,  new byte[]{ 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1 } },
            { 896,  new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0 } },
            { 960,  new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1 } },
            { 1024, new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0 } },
            { 1088, new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1 } },
            { 1152, new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0 } },
            { 1216, new byte[]{ 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1 } },
            { 1280, new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0 } },
            { 1344, new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1 } },
            { 1408, new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0 } },
            { 1472, new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1 } },
            { 1536, new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0 } },
            { 1600, new byte[]{ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1 } },
            { 1664, new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0 } },
            { 1728, new byte[]{ 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1 } }
        };

        public static readonly Dictionary<int, byte[]> MakeupCodes = new Dictionary<int, byte[]>
        {
            { 1792, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 } },   //1792 
            { 1856, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 } },     //1856 
            { 1920, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1 } },     //1920 
            { 1984, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 } },  //1984 
            { 2048, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1 } },  //2048 
            { 2112, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 } },  //2112 
            { 2176, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1 } },  //2176 
            { 2240, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0 } },  //2240 
            { 2304, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1 } },  //2304 
            { 2368, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 } },  //2368 
            { 2432, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1 } },  //2432 
            { 2496, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0 } },  //2496 
            { 2560, new byte[]{ 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 } },  //2560
        };

        public static byte[] Pass = new byte[] { 0, 0, 0, 1 };
        public static byte[] V0 = new byte[] { 1 };
        public static byte[] VR1 = new byte[] { 0, 1, 1 };
        public static byte[] VR2 = new byte[] { 0, 0, 0, 0, 1, 1 };
        public static byte[] VR3 = new byte[] { 0, 0, 0, 0, 0, 1, 1 };
        public static byte[] VL1 = new byte[] { 0, 1, 0 };
        public static byte[] VL2 = new byte[] { 0, 0, 0, 0, 1, 0 };
        public static byte[] VL3 = new byte[] { 0, 0, 0, 0, 0, 1, 0 };
        public static byte[] Horizal = new byte[] { 0, 0, 1 };

        public int Width { get; set; }
        public CCITTFax(int width)
        {
            Width = width;
        }

        /// <summary>
        /// 2维编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void EncodeT62D(BitMemory source, BitMemory target)
        {
            Encode2D(source, target, 0);

            for (int i = 0; i < 2; i++)
                target.WriteBits(EOL, 0, EOL.Length);
        }

        /// <summary>
        /// 1维2维混合编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="rows1D">1维编码行数，默认1行</param>
        public void EncodeT42D(BitMemory source, BitMemory target, int rows1D = 1)
        {
            Encode2D(source, target, rows1D);
            for (int i = 0; i < 6; i++)
                target.WriteBits(EOL.Concat(new byte[] { 1 }).ToArray(), 0, EOL.Length + 1);
        }

        void Encode2D(BitMemory source, BitMemory target, int rows1D)
        {
            source.Seek(0, SeekOrigin.Begin);
            target.Seek(0, SeekOrigin.Begin);
            if (rows1D > 0)
            {
                Encode1D(source, target, rows1D);
                source.Seek(-Width, SeekOrigin.Current);
            }

            byte[] EOL0 = EOL.Concat(new byte[] { 0 }).ToArray();
            int height = (int)(source.Length / Width);
            if (rows1D <= 0)
                rows1D = height;
            int a0, a1, a2, b1, b2;
            int[] reference = new int[Width];
            int[] codeLine = new int[Width];
            for (int i = rows1D; i < height; i++)
            {
                if (rows1D > 0)
                {
                    source.ReadBits(reference, 0, Width);
                    target.WriteBits(EOL0, 0, EOL.Length + 1);
                }
                source.ReadBits(codeLine, 0, Width);
                a0 = codeLine[0] == 0 ? -1 : 0;
                while (a0 < Width)
                {
                    a1 = Detect_a1(codeLine, a0);
                    b1 = Detect_b1(reference, codeLine, a0);
                    b2 = Detect_b2(reference, b1);

                    //通过模式
                    if (b2 < a1)
                    {
                        target.WriteBits(Pass, 0, Pass.Length);
                        a0 = b2;
                        continue;
                    }

                    //垂直模式
                    int length = a1 - b1;
                    if (Math.Abs(length) <= 3)
                    {
                        switch (length)
                        {
                            case -3: target.WriteBits(VL3, 0, VL3.Length); break;
                            case -2: target.WriteBits(VL2, 0, VL2.Length); break;
                            case -1: target.WriteBits(VL1, 0, VL1.Length); break;
                            case 0: target.WriteBits(V0, 0, V0.Length); break;
                            case 1: target.WriteBits(VR1, 0, VR1.Length); break;
                            case 2: target.WriteBits(VR2, 0, VR2.Length); break;
                            case 3: target.WriteBits(VR3, 0, VR3.Length); break;
                            default:
                                break;
                        }
                        a0 = a1;
                        continue;
                    }

                    //水平模式001 + M(a0a1) + M(a1a2)
                    a2 = Detect_a2(codeLine, a1);
                    target.WriteBits(Horizal, 0, Horizal.Length);
                    byte[] bs = Get1DCode(codeLine[a0] == 0, a1 - a0);
                    target.WriteBits(bs, 0, bs.Length);
                    bs = Get1DCode(codeLine[a1] == 0, a2 - a1);
                    target.WriteBits(bs, 0, bs.Length);
                    a0 = a2;
                }
                reference = codeLine;
            }
        }

        int Detect_a1(int[] codeLine, int a0)
        {
            int j = a0 + 1;
            while (j < Width)
            {
                if ((a0 == -1 && codeLine[j] == 0) || codeLine[j] != codeLine[a0])
                    return j;
                j++;
            }
            return Math.Min(j, Width);
        }

        int Detect_b1(int[] referenceLine, int[] codeLine, int a0)
        {
            if (a0 >= Width) return Width;

            int valueA0 = codeLine[a0];
            int j = Math.Max(a0, 0);
            bool jump = false;
            while (j < Width)
            {
                if (referenceLine[j] == valueA0)
                    jump = true;
                else if (jump)
                    return j;
                j++;
            }
            return j;
        }

        int Detect_b2(int[] referenceLine, int b1)
        {
            int j = b1 + 1;
            while (j < Width)
            {
                if (referenceLine[j] != referenceLine[b1])
                    return j;
                j++;
            }
            return Math.Min(j, Width);
        }

        int Detect_a2(int[] codeLine, int a1)
        {
            int j = a1 + 1;
            while (j < Width)
            {
                if (codeLine[j] != codeLine[a1])
                    return j;
                j++;
            }
            return Math.Min(j, Width);
        }

        /// <summary>
        /// 1维编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="rowsCount">0表示所有行使用1维编码</param>
        public void Encode1D(BitMemory source, BitMemory target, int rowsCount = 0)
        {
            int height = (int)(source.Length / Width);
            if (rowsCount <= 0)
                rowsCount = height;
            List<List<KeyValuePair<int, int>>> rows = CountRunLength(source, rowsCount);
            byte[] eol = rowsCount == height ? EOL : EOL.Concat(new byte[] { 1 }).ToArray();
            for (int i = 0; i < rowsCount; i++)
            {
                target.WriteBits(eol, 0, eol.Length);

                if (rows[i][0].Key == 0)
                    target.WriteBits(WhiteMakeUpCodes[0], 0, WhiteMakeUpCodes[0].Length);
                for (int j = 0; j < rows[i].Count; j++)
                {
                    int length = rows[i][j].Value;
                    bool isBlack = rows[i][j].Key == 0;

                    byte[] bs = Get1DCode(isBlack, length);
                    target.WriteBits(bs, 0, bs.Length);
                }
            }

            if(rowsCount == height)
                for(int i = 0; i < 6; i++)
                    target.WriteBits(EOL, 0, EOL.Length);
        }

        byte[] Get1DCode(bool isBlack, int length)
        {
            int v = length;
            List<byte> codes= new List<byte>();
            while (v >= 1792)
            {
                for (int h = MakeupCodes.Count - 1; h >= 0; h--)
                {
                    int key = MakeupCodes.Keys.ElementAt(h);
                    if (v >= key)
                    {
                        codes.AddRange(MakeupCodes[key]);
                        v -= key;
                    }
                }
            }

            if (isBlack)
            {
                while (v >= 64)
                {
                    for (int h = BlackMakeupCodes.Count - 1; h >= 0; h--)
                    {
                        int key = BlackMakeupCodes.Keys.ElementAt(h);
                        if (v >= key)
                        {
                            codes.AddRange(BlackMakeupCodes[key]);
                            v -= key;
                        }
                    }
                }
                codes.AddRange(BlackTerminatingCodes[v]);
            }
            else
            {
                while (v >= 64)
                {
                    for (int h = WhiteMakeUpCodes.Count - 1; h >= 0; h--)
                    {
                        int key = WhiteMakeUpCodes.Keys.ElementAt(h);
                        if (v >= key)
                        {
                            codes.AddRange(WhiteMakeUpCodes[key]);
                            v -= key;
                        }
                    }
                }
                codes.AddRange(WhiteTerminatingCodes[v]);
            }
            return codes.ToArray();
        }

        /// <summary>
        /// 游程统计
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public List<List<KeyValuePair<int, int>>> CountRunLength(BitMemory bm, int rowsCount = 0)
        {
            if(rowsCount == 0)
                rowsCount = (int)(bm.Length / Width);
            List<List<KeyValuePair<int, int>>> rows = new List<List<KeyValuePair<int, int>>>();
            bm.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < rowsCount; i++)
            {
                List<KeyValuePair<int, int>> row = new List<KeyValuePair<int, int>>();
                int value = bm.ReadBit();
                int len = 1;
                int v = 0;
                for (int j = 1; j < Width; j++)
                {
                    v = bm.ReadBit();
                    if (value == v)
                        len++;
                    else
                    {
                        row.Add(new KeyValuePair<int, int>(value, len));
                        len = 1;
                        value = v;
                    }
                }
                row.Add(new KeyValuePair<int, int>(value, len));
                rows.Add(row);
            }
            return rows;
        }
    }
}
