using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Golomb_
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice;
            do
            {
                //Console.Clear();
                Console.WriteLine("1. Закодировать текстовый файл");
                Console.WriteLine("2. Декодировать текстовый файл");
                Console.WriteLine("3. Выйти");
                Console.Write("Введите номер опции: ");
                int.TryParse(Console.ReadLine(), out choice);

                if (choice == 1)
                {
                    Console.Write("Введите имя файла для кодирования: ");
                    string filename = Console.ReadLine();

                    byte[] bytes = File.ReadAllBytes(filename);

                    byte[] encoded = Golomb.encode(bytes);

                    File.WriteAllBytes(filename.Substring(0, filename.Length - 4) + "_Golomb.txt", encoded);

                    Console.WriteLine("Кодирование успешно завершено");
                    Console.ReadKey();
                }
                else if (choice == 2)
                {
                    Console.Write("Введите имя файла для декодирования: ");
                    string filename = Console.ReadLine();
                    byte[] encodedBytes = File.ReadAllBytes(filename);

                    byte[] decodedBytes = Golomb.decode(encodedBytes);

                    File.WriteAllBytes(filename.Substring(0, filename.Length - 4) + "_Decoded.txt", decodedBytes);

                    Console.WriteLine("Декодирование успешно завершено");
                    Console.ReadKey();
                }
            } while (choice != 3);
        }

        /// <summary>
        /// Метод для построения таблицы из класса Golomb
        /// </summary>
        void sort()
        {
            Dictionary<byte, int> dict = new Dictionary<byte, int>();

            byte[] bytes = File.ReadAllBytes("Bible.txt");

            foreach (var item in bytes)
            {
                if (!dict.ContainsKey(item))
                {
                    dict.Add(item, 1);
                }
                else
                {
                    dict[item]++;
                }
            }

            for (byte i = 0; ; i++)
            {
                if (!dict.ContainsKey(i))
                {
                    dict.Add(i, 0);
                }

                if (i == 255)
                {
                    break;
                }
            }

            List<KeyValuePair<byte, int>> list = dict.OrderByDescending(x => x.Value).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                Console.Write(list[i].Key + ", ");
            }
        }
    }
}
