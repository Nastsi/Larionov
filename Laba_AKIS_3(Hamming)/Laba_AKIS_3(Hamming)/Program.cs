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
            int[] encode = new int[7];
            int[] S = new int[3];

            do
            {
                Console.WriteLine("1. Сгенерировать последовательность бит.");
                Console.WriteLine("2. Закодировать последовательность кодом Хэмминга.");
                Console.WriteLine("3. Произвести ошибку в закодированной последовательности.");
                Console.WriteLine("4. Восстановить");
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

                if (key == 3)
                {
                    int error = rand.Next(0, 8);
                    if (encode[error] == 0) encode[error] = 1;
                    else encode[error] = 0;

                    Console.Write("Получившая последовательность: ");

                    for (int i = 0; i < 7; i++)
                    {
                        Console.Write(encode[i] + " ");
                    }
                }

                if (key == 4)
                {
                    S[0] = encode[4] ^ encode[0] ^ encode[1] ^ encode[2];
                    S[1] = encode[5] ^ encode[1] ^ encode[2] ^ encode[3];
                    S[2] = encode[6] ^ encode[0] ^ encode[1] ^ encode[3];

                    Console.Write("Синдром последовательности: ");

                    for (int i = 0; i < 3; i++)
                    {
                        Console.Write(S[i] + " ");
                    }

                    int error = 0;

                    if (S[0] == 0)
                    {
                        if (S[1] == 0)
                        {
                            if (S[2] == 0) Console.WriteLine("Искажений нет.");
                            else error = 6;
                        }
                        else
                        {
                            if (S[2] == 0) error = 5;
                            else error = 3;
                        }
                    }
                    else
                    {
                        if (S[1] == 0)
                        {
                            if (S[2] == 0) error = 4;
                            else error = 0;
                        }
                        else
                        {
                            if (S[2] == 0) error = 2;
                            else error = 1;
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("Ошибка в " + (error + 1) + " бите.");

                    if (encode[error] == 0) encode[error] = 1;
                    else encode[error] = 0;

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
