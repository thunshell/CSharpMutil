using CSharpMutil.Binary;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpMutil.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str1 = "J..)6T`?p&<!J9%_[umg\"B7/Z7KNXbN'S+,*Q/&\"OLT'FLIDK#!n`$\"<Atdi`\\Vn%b%)&'cA*VnK\\CJY(sF>c!Jnl@RM]WM;jjH6Gnc75idkL5]+cPZKEBPWdR>FF(kj1_R%W_d&/jS!;iuad7h?[L−F$+]]0A3Ck*$I0KZ?;<)CJtqi65XbVc3\\n5ua:Q/=0$W<#N3U;H,MQKqfg1?:lUpR;6oN[C2E4ZNr8Udn.'p+?#X+1>0Kuk$bCDF/(3fL5]Oq)^kJZ!C2H1'TO]Rl?Q:&'<5&iP!$Rq;BXRecDN[IJB`,)o8XJOSJ9sDS]hQ;Rj@!ND)bD_q&C\\g:inYC%)&u#:u,M6Bm%IY!Kb1+\":aAa'S`ViJglLb8<W9k6Yl\\\\0McJQkDeLWdPN?9A'jX*al>iG1p&i;eVoK&juJHs9%;Xomop\"5KatWRT\"JQ#qYuL,JD?M$0QP)lKn06l1apKDC@\\qJ4B!!(5m+j.7F790m(Vj88l8Q:_CZ(Gm1%X\\N1&u!FKHMB~>";
            string str = "2 J \rBT\r/F1 12 Tf\r0 Tc\r0 Tw\r72.5 712 TD\r[ ( Unencoded streams can be read easily ) 65 ( , ) ] TJ\r0 −14 TD\r[ ( b ) 20 ( ut generally tak ) 10 ( e more space than \\311 ) ] TJ\rT* ( encoded streams . ) Tj\r0 −28 TD\r[ ( Se ) 25 ( v ) 15 ( eral encoding methods are a ) 20 ( v) 25 ( ailable in PDF ) 80 ( . ) ] TJ\r0 −14 TD\r( Some are used for compression and others simply ) Tj\rT* [ ( to represent binary data in an ) 55 ( ASCII format . ) ] TJ\rT* ( Some of the compression encoding methods are \\\rsuitable ) Tj\rT* ( for both data and images , while others are \\\rsuitable only ) Tj\rT* ( for continuous−tone images . ) Tj\rET";
            byte[] bs = Encoding.ASCII.GetBytes(str1);
            byte[] bs5 = ASCII85.Decode(bs);
            //bs = new byte[] { 45, 45, 45, 45, 45, 65, 45, 45, 45, 66 };
            Console.WriteLine(string.Join(",", bs5));
            byte[] bs3 = Encoding.UTF8.GetBytes(str);
            byte[] bs2 = new LZW(2).Encode(bs3);
            //byte[] bs1 = ASCII85.Encode(bs2);
            //Console.WriteLine(string.Join(",", bs2));
            //string s = Encoding.ASCII.GetString(bs1);
            //bs = ASCII85.Decode(bs1);
            byte[] bs4 = new LZW(2).Decode(bs2);//new LZWDecode().Decode(bs2);//
            //Console.WriteLine(string.Join(",", bs4));
            string ss = Encoding.UTF8.GetString(bs4);
            Console.WriteLine(ss);
            Console.ReadKey();
        }
    }

    public class LZWDecode
    {
        public byte[] Decode(byte[] data)
        {
            if (data[0] == 0x00 && data[1] == 0x01)
                throw new Exception("LZW flavour not supported.");

            MemoryStream outputStream = new MemoryStream();

            InitializeDictionary();

            _data = data;
            _bytePointer = 0;
            _nextData = 0;
            _nextBits = 0;
            int code, oldCode = 0;
            byte[] str;

            while ((code = NextCode) != 257)
            {
                if (code == 256)
                {
                    InitializeDictionary();
                    code = NextCode;
                    if (code == 257)
                    {
                        break;
                    }
                    outputStream.Write(_stringTable[code], 0, _stringTable[code].Length);
                    oldCode = code;

                }
                else
                {
                    if (code < _tableIndex)
                    {
                        str = _stringTable[code];
                        outputStream.Write(str, 0, str.Length);
                        AddEntry(_stringTable[oldCode], str[0]);
                        oldCode = code;
                    }
                    else
                    {
                        str = _stringTable[oldCode];
                        outputStream.Write(str, 0, str.Length);
                        AddEntry(str, str[0]);
                        oldCode = code;
                    }
                }
            }

            if (outputStream.Length >= 0)
            {
#if !NETFX_CORE && !UWP
                outputStream.Capacity = (int)outputStream.Length;
                return outputStream.GetBuffer();
#else
                return outputStream.ToArray();
#endif
            }
            return null;
        }

        /// <summary>
        /// Initialize the dictionary.
        /// </summary>
        void InitializeDictionary()
        {
            _stringTable = new byte[8192][];

            for (int i = 0; i < 256; i++)
            {
                _stringTable[i] = new byte[1];
                _stringTable[i][0] = (byte)i;
            }

            _tableIndex = 258;
            _bitsToGet = 9;
        }

        /// <summary>
        /// Add a new entry to the Dictionary.
        /// </summary>
        void AddEntry(byte[] oldstring, byte newstring)
        {
            int length = oldstring.Length;
            byte[] str = new byte[length + 1];
            Array.Copy(oldstring, 0, str, 0, length);
            str[length] = newstring;

            _stringTable[_tableIndex++] = str;

            if (_tableIndex == 511)
                _bitsToGet = 10;
            else if (_tableIndex == 1023)
                _bitsToGet = 11;
            else if (_tableIndex == 2047)
                _bitsToGet = 12;
        }

        /// <summary>
        /// Returns the next set of bits.
        /// </summary>
        int NextCode
        {
            get
            {
                try
                {
                    _nextData = (_nextData << 8) | (_data[_bytePointer++] & 0xff);
                    _nextBits += 8;

                    if (_nextBits < _bitsToGet)
                    {
                        _nextData = (_nextData << 8) | (_data[_bytePointer++] & 0xff);
                        _nextBits += 8;
                    }

                    int code = (_nextData >> (_nextBits - _bitsToGet)) & _andTable[_bitsToGet - 9];
                    _nextBits -= _bitsToGet;

                    return code;
                }
                catch
                {
                    return 257;
                }
            }
        }

        readonly int[] _andTable = { 511, 1023, 2047, 4095 };
        byte[][] _stringTable;
        byte[] _data;
        int _tableIndex, _bitsToGet = 9;
        int _bytePointer;
        int _nextData = 0;
        int _nextBits = 0;
    }
}
