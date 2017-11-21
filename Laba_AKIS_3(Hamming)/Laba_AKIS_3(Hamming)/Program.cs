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

            do
            {
                Console.WriteLine();
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

                //Console.ReadKey();
            } while (key != 5);
        }
    }
}
