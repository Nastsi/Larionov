using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_AKIS_3_Hamming_
{
    class Program
    {
        static void Main(string[] args)
        {
            int key = 0;
            Random rand = new Random();
            int[] bits = new int[4];
            int[] r = new int[3];
            int[] encode = new int[7];

            do
            {
                Console.WriteLine("1. Сгенерировать последовательность бит.");
                Console.WriteLine("2. Закодировать последовательность кодом Хэмминга.");
                Console.WriteLine("3. Произвести ошибку в закодированной последовательности.");
                Console.WriteLine("4. Декодировать и восстановить");
                Console.WriteLine("5. Выйти");
                Console.WriteLine();
                Console.Write("Ответ: ");
                int.TryParse(Console.ReadLine(), out key);

                if (key == 1)
                {
                    Console.WriteLine();
                    Console.Write("Получившая последовательность: ");

                    for (int i = 0; i < 4; i++)
                    {
                        bits[i] = rand.Next(0, 2);
                        Console.Write(bits[i] + " ");
                    }
                }
                
                if (key == 2)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        encode[i] = bits[i];
                    }
                    encode[4] = bits[0] ^ bits[1] ^ bits[2];
                    encode[5] = bits[1] ^ bits[2] ^ bits[3];
                    encode[6] = bits[0] ^ bits[1] ^ bits[3];

                    Console.Write("Получившая последовательность: ");

                    for (int i = 0; i < 7; i++)
                    {
                        Console.Write(encode[i] + " ");
                    }
                }

                Console.WriteLine();
                Console.WriteLine();

            } while (key != 5);
        }
    }
}
