using CSharpMutil.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpMutil.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str1 = "J.)6T ?p&<!J9%_ [umg\"B7/Z7KNXbN'S+,*Q/&\"OLT'FLIDK#!n^ $\"<Atdi^\\Vn%b%)&'cA*VnK\\CJY(sF>c!Jnl@ ;RM]WM;jjH6Gnc75idkL 5]+cPZKEBPWdR> FF(kj1_ _R%W_ _d&/jS!;iuad7h?[L-F$+]]0A3Ck*$I0KZ?;<)CJtqi65XbVc3\\n5ua:Q/=0$W<#N3U;H,MQKqfg 1?:IU pR;6oN[C2E4ZNr8Udn.' p+?#X+ 1>0Kuk$bCDF/(3fl 5]0q)^kJZ!C2H1'TO]RI?Q:&'<5&iP!$Rq;BXRecDN[JB' ,)o8XJOSJ9sDS]hQ;Rj@!ND)bD_ q&C\\g:inYC%)&u#:u,M6Bm%IY!Kb1+\":aAa'S\"ViJgIL b8<W9k6YI\\0MdQkDeLWdPN?9AjX*al>iG1p&i;eVoK&jwJHs9%;Xomop\" 5KatWRT\"JQ#qYuL,JD?M$0QP)IKn06l 1apKDC@\\qJB!(5m+j.7F790m(Vj88I8Q: _CZ(Gm1 %X\\N1 &u!FKHMB~>";
            string str = "2 J\r\nBT\r\n/F1 12 Tf\r\n0 Tc\r\n0 Tw\r\n72.5 712 TD\r\n[(Unencoded streams can be read easily) 65 (,)] TJ\r\n0 -14 TD\r\n[(b) 20 (ut generally tak) 10 (e more space than\\311)] TJ\r\nT* (encoded streams.) Tj\r\n0 -28 TD\r\n[(Se) 25 (v) 15 (eral encoding methods area) 20 (v) 25 (ailable inPDF) 80 (.)] TJ\r\n0 -14 TD\r\n(Some are used for compression and others simply) Tj\r\nT* [(to represent binary data in an) 55 (ASCII format.)] TJ\r\nT* (Some of the compression encoding methods are \\\r\nsuitable ) Tj\r\nT* (for both data and images, while others are\\\r\nsuitable only) Tj\r\nT* (for continuous-tone images.) Tj\r\nET";
            byte[] bs = ASCII85.Decode(Encoding.ASCII.GetBytes(str1));
            Console.WriteLine(string.Join(",", bs));
            //bs = new byte[] { 45, 45, 45, 45, 45, 65, 45, 45, 45, 66 };
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(str)))
            {
                using (MemoryStream encode  =new MemoryStream(bs))
                {
                    LZW.Encode(ms, encode);
                    byte[] bs1 = encode.ToArray();
                    Console.WriteLine(string.Join(",", bs1));
                    Console.ReadKey();
                    string s = Encoding.ASCII.GetString(ASCII85.Encode(bs1));
                    using (MemoryStream decode = new MemoryStream())
                    {
                        LZW.Decode(encode, decode);
                        bs1 = decode.ToArray();
                        Console.WriteLine(Encoding.ASCII.GetString(bs1));
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
