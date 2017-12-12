using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.IO.Compression;
using zlib;


namespace Laba_AKIS_5
{
    class Program
    {
        public static long width;
        public static long height;
        public static uint[] crcTable;

        private static uint Crc32(byte[] stream, uint crc)
        {
            uint c;
            if (crcTable == null)
            {
                crcTable = new uint[256];
                for (uint n = 0; n <= 255; n++)
                {
                    c = n;
                    for (var k = 0; k <= 7; k++)
                    {
                        if ((c & 1) == 1)
                            c = 0xEDB88320 ^ ((c >> 1) & 0x7FFFFFFF);
                        else
                            c = ((c >> 1) & 0x7FFFFFFF);
                    }
                    crcTable[n] = c;
                }
            }
            c = crc ^ 0xffffffff;
            for (var i = 0; i < stream.Length; i++)
            {
                c = crcTable[(c ^ stream[i]) & 255] ^ ((c >> 8) & 0xFFFFFF);
            }
            return c ^ 0xffffffff;
        }

        static void Main(string[] args)
        {
            var input = File.OpenRead("1.bmp");
            for (int p = 0; p < 18; p++)
            {
                input.ReadByte();
            }
            width = input.ReadByte() + input.ReadByte() * 256 + input.ReadByte() * 65536 + input.ReadByte() * 16777216;
            height = input.ReadByte() + input.ReadByte() * 256 + input.ReadByte() * 65536 + input.ReadByte() * 16777216;
            input.Close();

            File.WriteAllText("Itog.png", "");
            var output = File.OpenWrite("Itog.png");
            byte[] basic = { 137, 80, 78, 71, 13, 10, 26, 10 };//PNG signature
            for (int p = 0; p < basic.Length; p++)
            {
                output.WriteByte(basic[p]);
            }
            //IHDR
            output.WriteByte(0); //IHDR Length
            output.WriteByte(0);
            output.WriteByte(0);
            output.WriteByte(13);

            basic = new byte[17];
            basic[0] = 73;
            basic[1] = 72;
            basic[2] = 68;
            basic[3] = 82;

            basic[4] = (byte)(width / 16777216);
            basic[5] = (byte)((width / 65536) % 256);
            basic[6] = (byte)((width / 256) % 256);
            basic[7] = (byte)(width % 256);

            basic[8] = (byte)(height / 16777216);
            basic[9] = (byte)((height / 65536) % 256);
            basic[10] = (byte)((height / 256) % 256);
            basic[11] = (byte)(height % 256);

            basic[12] = 8; //Bit depth
            basic[13] = 2; //1 - Palette, 2 - Color, 4 - Alpha
            basic[14] = 0; //const Deflate
            basic[15] = 0; //const Filtrating
            basic[16] = 0; //Interlase

            for (int p = 0; p < basic.Length; p++)
            {
                output.WriteByte(basic[p]);
            }

            uint crc = Crc32(basic, 0);
            output.WriteByte((byte)(crc / 16777216));
            output.WriteByte((byte)((crc / 65536) % 256));
            output.WriteByte((byte)((crc / 256) % 256));
            output.WriteByte((byte)(crc % 256));

            byte[] mass = new byte[width * height * 3 + height]; //Get Pixels
            input = File.OpenRead("1.bmp");
            for (int p = 0; p < 54; p++)
            {
                input.ReadByte();
            }
            for (long p = height - 1; p >= 0; p--)
            {
                mass[p * width * 3 + p] = 0;
                for (long q = 0; q < width; q++)
                {
                    mass[p * width * 3 + q * 3 + 2 + (p + 1)] = (byte)input.ReadByte();
                    mass[p * width * 3 + q * 3 + 1 + (p + 1)] = (byte)input.ReadByte();
                    mass[p * width * 3 + q * 3 + (p + 1)] = (byte)input.ReadByte();
                }
            }

            //IDAT  
            File.WriteAllText("Temp", "");
            FileStream outFileStream = new FileStream("Temp", FileMode.Create);
            ZOutputStream outZStream = new ZOutputStream(outFileStream, zlibConst.Z_BEST_COMPRESSION);
            outZStream.Write(mass, 0, mass.Length);
            outZStream.Close();
            outFileStream.Close();
            outFileStream = new FileStream("Temp", FileMode.Open);
            mass = new byte[outFileStream.Length];
            outFileStream.Read(mass, 0, mass.Length);
            outFileStream.Close();
            File.Delete("Temp");

            byte[] itog;
            for (int p = 0; p < mass.Length; p += 65524)
            {
                output.WriteByte(0);
                output.WriteByte(0);
                if (p + 65524 < mass.Length)
                {
                    output.WriteByte(255);
                    output.WriteByte(244);
                    itog = new byte[65528];
                    Array.Copy(mass, p, itog, 4, 65524);
                }
                else
                {
                    output.WriteByte((byte)((mass.Length - p) / 256));
                    output.WriteByte((byte)((mass.Length - p) % 256));
                    itog = new byte[mass.Length - p + 4];
                    Array.Copy(mass, p, itog, 4, mass.Length - p);
                }
                itog[0] = 73;
                itog[1] = 68;
                itog[2] = 65;
                itog[3] = 84;
                for (int q = 0; q < itog.Length; q++)
                {
                    output.WriteByte(itog[q]);
                }
                crc = Crc32(itog, 0);
                output.WriteByte((byte)(crc / 16777216));
                output.WriteByte((byte)((crc / 65536) % 256));
                output.WriteByte((byte)((crc / 256) % 256));
                output.WriteByte((byte)(crc % 256));
            }



            byte[] end = { 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };
            for (int p = 0; p < end.Length; p++)
            {
                output.WriteByte(end[p]);
            }
        }
    }
}