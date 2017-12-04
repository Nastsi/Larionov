using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Drawing;

namespace AKIS_4
{
    class Program
    {
        public static int width;
        public static int height;
        public static byte[] Delay = { 16, 0 };
        public static int[][] print;
        public static int tek_bits;
        public static bool reboot;
        public static int pictures = 6;


        class LzwStringTable
        {
            public LzwStringTable(int numBitsPerCode)
            {
                maxCode = (int)Math.Pow(2, numBitsPerCode);
                for (int p = 0; p < nextCode; p++)
                {
                    table[p.ToString()] = p;
                }
            }

            public void AddCode(string s)
            {
                if (nextCode == maxCode)
                {
                    tek_bits++;
                    maxCode *= 2;
                }
                table[s] = nextCode;
                nextCode++;
                if (nextCode == 4096)
                {
                    reboot = true;
                    tek_bits = 9;
                    table = new Dictionary<string, int>();
                    maxCode = (int)Math.Pow(2, 9);
                    nextCode = 258;
                    for (int p = 0; p < nextCode; p++)
                    {
                        table[p.ToString()] = p;
                    }

                }
            }

            public int GetCode(string s)
            {
                return table[s];
            }

            public bool Contains(string s)
            {
                return table.ContainsKey(s);
            }

            private Dictionary<string, int> table = new Dictionary<string, int>();
            private int nextCode = 258;
            private int maxCode;
        }

        static void WriteCode(int code)
        {
            int[][] temp = new int[print.Length + 1][];
            for (int p = 0; p < print.Length; p++)
            {
                temp[p] = new int[2];
                temp[p][0] = print[p][0];
                temp[p][1] = print[p][1];
            }
            temp[print.Length] = new int[2];
            temp[print.Length][0] = code;
            temp[print.Length][1] = tek_bits;
            print = temp;
        }

        static void Main(string[] args)
        {
            var input = File.OpenRead("0.bmp");
            for (int p = 0; p < 18; p++)
            {
                input.ReadByte();
            }
            width = input.ReadByte() + input.ReadByte() * 256 + input.ReadByte() * 65536 + input.ReadByte() * 16777216;
            height = input.ReadByte() + input.ReadByte() * 256 + input.ReadByte() * 65536 + input.ReadByte() * 16777216;
            input.Close();

            File.WriteAllText("Itog.gif", "");
            var output = File.OpenWrite("Itog.gif"); //Making Correct Header
            byte[] basic = { 71, 73, 70, 56, 57, 97 }; //GIF89a
            for (int p = 0; p < basic.Length; p++)
            {
                output.WriteByte(basic[p]);
            }
            output.WriteByte((byte)(width % 256));
            output.WriteByte((byte)((width / 256) % 256));
            output.WriteByte((byte)(height % 256));
            output.WriteByte((byte)((height / 256) % 256));
            output.WriteByte(247); //Correct Packet
            output.WriteByte(0); //Background Color
            output.WriteByte(0); //Pixel Aspect Ratio
            for (int p = 0; p < 256; p += 51) //Global Color Table
            {
                for (int q = 0; q <= 256; q += 43)
                {
                    if (q == 86) q--;
                    if (q == 171) q--;
                    if (q == 256) q--;
                    for (int r = 0; r < 256; r += 51)
                    {
                        output.WriteByte((byte)p);
                        output.WriteByte((byte)q);
                        output.WriteByte((byte)r);
                    }
                }
            }
            for (int p = 0; p < 12; p++)
            {
                output.WriteByte(0);
            }

            for (int image = 0; image < 1; image++)
            {
                //Graphic Control Extension
                output.WriteByte(33); //Introducer
                output.WriteByte(249); //Graphic Control Lable
                output.WriteByte(4); //Block Size
                output.WriteByte(1); //Data Block
                output.WriteByte(Delay[0]);
                output.WriteByte(Delay[1]);
                output.WriteByte(252); //Index Transarent
                output.WriteByte(0); //Block Terminator

                //Image Descriptor
                output.WriteByte(44); //Image Separator
                output.WriteByte(0); //Left Position
                output.WriteByte(0);
                output.WriteByte(0); //Top Position
                output.WriteByte(0);
                output.WriteByte((byte)(width % 256));
                output.WriteByte((byte)((width / 256) % 256));
                output.WriteByte((byte)(height % 256));
                output.WriteByte((byte)((height / 256) % 256));
                output.WriteByte(0); //Packet

                //Image Data

                //Console.WriteLine("Изображение №" + image);
                output.WriteByte(8); //Minimum LZW Code size
                //input = File.OpenRead("0.bmp");
                //for (int p = 0; p < 54; p++)
                //{
                //    input.ReadByte();
                //}
                Bitmap bitmap = new Bitmap("0.bmp");
                int[][] mass = new int[height][];
                for (int p = 0; p < height; p++)
                {
                    mass[p] = new int[width];
                }
                for (int p = height - 1; p >= 0; p--)
                {
                    for (int q = 0; q < width; q++)
                    {
                        Color pixel = bitmap.GetPixel(q, p);
                        int blue = pixel.B;
                        int green = pixel.G;
                        int red = pixel.R;

                        int red_pos;
                        if (red % 51 < 26)
                        {
                            red_pos = red / 51;
                        }
                        else
                        {
                            red_pos = red / 51 + 1;
                        }

                        int blue_pos;
                        if (blue % 51 < 26)
                        {
                            blue_pos = blue / 51;
                        }
                        else
                        {
                            blue_pos = blue / 51 + 1;
                        }
                        int green_pos;
                        if (green % 43 < 22)
                        {
                            green_pos = green / 43;
                        }
                        else
                        {
                            green_pos = green / 43 + 1;
                        }
                        mass[p][q] = blue_pos + green_pos * 6 + red_pos * 42;
                    }
                }
                tek_bits = 9;
                reboot = false;
                LzwStringTable table = new LzwStringTable(tek_bits);
                string match = mass[0][0].ToString();
                int pos = 1;
                print = new int[1][];
                print[0] = new int[2];
                print[0][0] = 256;
                print[0][1] = tek_bits;
                Console.WriteLine("Кодирую байты в LZW");
                while (pos < width * height)
                {
                    string nextMatch = match + " " + mass[pos / width][pos % width].ToString();

                    if (table.Contains(nextMatch))
                    {
                        match = nextMatch;
                    }
                    else
                    {
                        WriteCode(table.GetCode(match));
                        table.AddCode(nextMatch);
                        if (reboot)
                        {
                            reboot = false;
                            int[][] temp = new int[print.Length + 1][];
                            for (int p = 0; p < print.Length; p++)
                            {
                                temp[p] = new int[2];
                                temp[p][0] = print[p][0];
                                temp[p][1] = print[p][1];
                            }
                            temp[print.Length] = new int[2];
                            temp[print.Length][0] = 256;
                            temp[print.Length][1] = 12;
                            print = temp;
                        }
                        match = mass[pos / width][pos % width].ToString();
                    }
                    pos += 1;
                    //if (pos % 100 == 0) Console.WriteLine(pos);
                }

                WriteCode(table.GetCode(match));
                WriteCode(257);
                Console.WriteLine("Подготавливаю байты к записи");
                bool[] write = new bool[0];
                for (int p = 0; p < print.Length; p++)
                {
                    bool[] temp = new bool[write.Length + print[p][1]];
                    for (int q = temp.Length - 1; q >= print[p][1]; q--)
                    {
                        temp[q] = write[q - print[p][1]];
                    }
                    for (int q = print[p][1] - 1; q >= 0; q--)
                    {
                        if (print[p][0] % 2 == 1)
                        {
                            temp[q] = true;
                        }
                        else
                        {
                            temp[q] = false;
                        }
                        print[p][0] /= 2;
                    }
                    write = temp;
                }
                if (write.Length % 8 != 0)
                {
                    bool[] temp = new bool[write.Length + (8 - write.Length % 8)];
                    for (int p = temp.Length - 1; p >= (8 - write.Length % 8); p--)
                    {
                        temp[p] = write[p - (8 - write.Length % 8)];
                    }
                    for (int p = 8 - write.Length % 8 - 1; p >= 0; p--)
                    {
                        temp[p] = false;
                    }
                    write = temp;
                }

                Console.WriteLine("Записываю...");
                byte[] to_write = new byte[write.Length / 8];
                for (int p = 0; p < to_write.Length; p++)
                {
                    to_write[p] = 0;
                    byte temp = 1;
                    for (int q = write.Length - 1 - p * 8; q >= write.Length - 8 - p * 8; q--)
                    {
                        if (write[q])
                        {
                            to_write[p] += temp;
                        }
                        temp *= 2;
                    }
                }

                int multipler = 0;
                int length = to_write.Length;
                while (true)
                {
                    if (length > 255)
                    {
                        output.WriteByte(255);
                        for (int p = 0; p < 255; p++)
                        {
                            output.WriteByte(to_write[multipler * 255 + p]);
                        }
                        length -= 255;
                    }
                    else
                    {
                        output.WriteByte((byte)length);
                        for (int p = 0; p < length; p++)
                        {
                            output.WriteByte(to_write[multipler * 255 + p]);
                        }
                        break;
                    }
                    multipler++;
                }
                Console.WriteLine();
                output.WriteByte(0); //Block Terminator
            }


            output.WriteByte(59); //Trailer (End)
            Console.ReadKey();
        }
    }
}
