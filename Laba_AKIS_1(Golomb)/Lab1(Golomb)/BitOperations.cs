using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Golomb_
{
    class BitOperations
    {
        public static byte[] calculateBits(int n)
        {
            string s = Convert.ToString(n, 2);

            byte[] bits = s.Select(c => byte.Parse(c.ToString()))
                         .ToArray();
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


    }
}
