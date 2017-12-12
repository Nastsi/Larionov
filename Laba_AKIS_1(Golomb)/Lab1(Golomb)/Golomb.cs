using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Golomb_
{
    class Golomb
    {
        static byte[] dict = { 32, 238, 229, 232, 224, 242, 241, 237, 226, 235, 240, 228, 236, 44, 243, 234, 239,
            255, 227, 251, 225, 231, 252, 10, 245, 247, 233, 230, 46, 248, 254, 200, 49,
            246, 59, 50, 249, 58, 195, 95, 61, 51, 209, 204, 193, 197, 210, 52, 192, 45,
            244, 53, 194, 205, 206, 54, 55, 56, 63, 57, 48, 207, 253, 202, 196, 223, 91,
            93, 33, 34, 213, 208, 212, 199, 203, 211, 214, 250, 216, 215, 221, 198, 184,
            40, 41, 222, 217, 220, 201, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 35, 36, 37, 38,
            39, 42, 43, 47, 60, 62, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77,
            78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 92, 94, 96, 97, 98, 99, 100,
            101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
            117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132,
            133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148,
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164,
            165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180,
            181, 182, 183, 185, 186, 187, 188, 189, 190, 191, 218, 219 };

        const double p = 0.85;

        public static byte[] encode(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)Array.IndexOf(dict, bytes[i]);
            }

            int m = (int)Math.Floor(Math.Log(1 / (1 + p), p) + 1);

            List<byte> stream = new List<byte>();

            for (int i = 0; i < bytes.Length; i++)
            {
                int x = bytes[i];

                int q = (int)Math.Truncate((double)(x / m));

                for (int j = 0; j < q; j++)
                {
                    stream.Add(0);
                }

                stream.Add(1);

                int r = x % m;

                byte[] rBits = BitOperations.calculateBits(r);

                while (rBits.Length < Math.Log(m, 2))
                {
                    rBits = new byte[] { 0 }.Concat(rBits).ToArray();
                }

                stream.AddRange(rBits);
            }

            return BitOperations.getBytes(stream.ToArray());
        }

        public static byte[] decode(byte[] encoded)
        {
            byte[] bits = BitOperations.getBits(encoded);

            int m = (int)Math.Floor(Math.Log(1 / (1 + p), p) + 1);
            int rSize = (int)Math.Log(m, 2);

            List<byte> result = new List<byte>();

            int cursor = 0;
            int current = 0;

            while (cursor < bits.Length)
            {
                if (bits[cursor] == 1)
                {
                    int q = current;
                    cursor++;

                    string rBits = "";

                    for (int i = 0; i < rSize; i++)
                    {
                        rBits += bits[cursor];
                        cursor++;
                    }

                    int r = Convert.ToInt32(rBits, 2);
                    int x = q * m + r;
                    x = dict[x];
                    current = 0;
                    result.Add((byte)x);
                }
                else
                {
                    cursor++;
                    current++;
                }
            }

            return result.ToArray();
        }
    }
}
