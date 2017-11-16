using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Laba_AKIS_2_LZSS_
{
    class Program
    {
        public static byte[] dictionary;
        public static byte[] buffer;
        public static int schet = 0;
        public static int sizeMax = 0;

        public static byte[] getBits(byte[] array)
        {
            int size = array.Length;
            byte[] bits = new byte[8 * size];

            for (int j = 0; j < array.Length; j++)
            {
                byte b = array[j];

                for (int i = 0; i < 8; i++)
                {
                    if ((b & 0x80) != 0)
                    {
                        bits[i + j * 8] = 1;
                    }
                    else
                    {
                        bits[i + j * 8] = 0;
                    }
                    b *= 2;
                }
            }

            return bits;
        }

        public static byte[] getBitsofByte(byte b)
        {
            byte[] bits = new byte[8];

            for (int i = 0; i < 8; i++)
            {
                if ((b & 0x80) != 0)
                {
                    bits[i] = 1;
                }
                else
                {
                    bits[i] = 0;
                }
                b *= 2;
            }

            return bits;
        }

        public static byte getByte(byte[] bits)
        {
            byte result = 0;
            byte[] empty = new byte[] { 0 };

            while (bits.Length < 8)
            {
                bits = empty.Concat(bits).ToArray();
            }

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == 1)
                {
                    result += (byte)Math.Pow(2, 7 - i);
                }
            }
            return result;
        }

        public static byte[] getBytes(byte[] bits)
        {
            List<byte> bitsList = new List<byte>(bits);

            while (bitsList.Count % 8 != 0)
            {
                bitsList.Add(0);
            }

            bits = bitsList.ToArray();

            int bytesCount = bits.Length / 8;

            byte[] bytes = new byte[bytesCount];

            for (int i = 0; i < bytesCount; i++)
            {
                byte[] currentByteBits = new byte[8];
                Array.Copy(bits, i * 8, currentByteBits, 0, 8);
                byte currentByte = getByte(currentByteBits);
                bytes[i] = currentByte;
            }

            return bytes;
        }

        public static byte[] encode(byte[] bytes)
        {
            List<byte> stream = new List<byte>();


            for (int i = 0; i < bytes.Length; i++)
            {
                bool exist = false;
                int size = 0;
                int buf = 0;
                int dic = 0;

                if (i == 0) //первый раз заполняем буфер
                {
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        if ((i + j) != bytes.Length - 1)
                        {
                            buffer[j] = bytes[i + j];
                        }
                        else break;
                    }
                }

                for (int j = 0; j < dictionary.Length; j++) //начинаем проход по словарю
                {
                    if (dictionary[j] == buffer[buf])
                    {
                        exist = true;
                        size++;
                        buf++;
                        if (dic == 0)
                        {
                            dic = j;
                        }
                        if (buf == buffer.Length) break;
                    }
                    else
                    {
                        if ((exist == true) && (size > 2)) break; //если нашли одинаковые подстроки и дошли до символа, который не повторяется, уходим из цикла
                        else if (size <= 2) //если размер подстроки меньше 3х, то мы не будем это сжимать
                        {
                            size = 0;
                            exist = false;
                            buf = 0;
                            dic = 0;
                        }
                    }
                }

                if ((exist == false) || (size <= 2))
                {
                    stream.Add(0);
                    byte[] bits = getBitsofByte(bytes[i]);
                    stream.AddRange(bits);
                    size = 1;
                }
                else
                {
                    schet++;
                    if (sizeMax < size)
                    {
                        sizeMax = size;
                    }
                    stream.Add(1);
                    byte[] bits = getBitsofByte((byte)dic);
                    stream.AddRange(bits);
                    bits = getBitsofByte((byte)size);
                    stream.AddRange(bits);
                }

                for (int j = 0; j < dictionary.Length - size; j++) //сдвигаем словарь, освобождая место под новые символы в конце
                {
                    dictionary[j] = dictionary[j + size];
                }

                int tek = 0;
                for (int j = dictionary.Length - size; j < dictionary.Length; j++) //заполняем конец словаря подстрокой из буфера
                {
                    dictionary[j] = buffer[tek];
                    tek++;
                }

                for (int j = 0; j < buffer.Length; j++) //обновляем буфер
                {
                    if ((i + j + size) < bytes.Length - 1)
                    {
                        buffer[j] = bytes[i + j + size];
                    }
                    else
                    {
                        buffer[j] = 0;
                    }
                }
                if (size > 2)
                {
                    i = i + size - 1;
                }
            }

            return getBytes(stream.ToArray());
        }

        static void Main(string[] args)
        {
            int key = 0;
            dictionary = new byte[250];
            buffer = new byte[125];
            do
            {
                Console.WriteLine("1. Закодировать текстовый файл");
                Console.WriteLine("2. Декодировать текстовый файл");
                Console.WriteLine("3. Выйти");
                Console.Write("Ответ: ");
                int.TryParse(Console.ReadLine(), out key);

                if (key == 1)
                {
                    Console.Write("Введите имя файла для кодирования: ");
                    string filename = Console.ReadLine();

                    byte[] bytes = File.ReadAllBytes(filename);

                    byte[] encoded = encode(bytes);

                    File.WriteAllBytes(filename.Substring(0, filename.Length - 4) + "_LZSS.txt", encoded);

                    Console.WriteLine("Кодирование успешно завершено");
                    Console.WriteLine("SizeMax = " + sizeMax);
                    Console.ReadKey();
                }
            } while (key != 3);
        }
    }
}
