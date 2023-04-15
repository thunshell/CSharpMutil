using CSharpMutil.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpMutil.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int a = (int)Math.Pow(2, 32);
            //string str = "2 J\rBT\r/F1 12 Tf\r0 Tc\r0 Tw\r72.5 712 TD\r[ ( Unencoded streams can be read easily ) 65 ( , ) ] TJ\r0 −14 TD\r[ ( b ) 20 ( ut generally tak ) 10 ( e more space than \\311 ) ] TJ\rT* ( encoded streams . ) Tj\r0 −28 TD\r[ ( Se ) 25 ( v ) 15 ( eral encoding methods are a ) 20 ( v) 25 ( ailable in PDF ) 80 ( . ) ] TJ\r0 −14 TD\r( Some are used for compression and others simply ) Tj\rT* [ ( to represent binary data in an ) 55 ( ASCII format . ) ] TJ\rT* ( Some of the compression encoding methods are \\\rsuitable ) Tj\rT* ( for both data and images , while others are \\\rsuitable only ) Tj\rT* ( for continuous−tone images . ) Tj\rET";
            string str = "AAA";
            byte[] bs = new byte[] { 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 };
            using (BitMemory bm = new BitMemory())
            {
                bm.WriteBits(bs, 0, bs.Length);
                using (BitMemory bm1 = new BitMemory())
                {
                    new CCITTFax(7).EncodeT42D(bm, bm1);
                    byte[] bs1 = bm1.ToArray();
                    ;
                }
            }
            
            
            Console.ReadKey();
        }
    }
}
