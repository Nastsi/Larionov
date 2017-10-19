using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Laba_PA_2
{
    class Program
    {
        static public long n = 10000000;
        static public int threads;
        static public long[] mass;
        static public long[] prefix;

        static void Main(string[] args)
        {
            int key = 0;
            Random rand = new Random();
            mass = new long[n];

            for (int i = 0; i < n; i++)
            {
                mass[i] = rand.Next(1, 50);
                Console.Write(mass[i] + " ");
            }

            do
            {
                Console.WriteLine();
                Console.WriteLine("1. Start.");
                Console.WriteLine("2. Exit.");
                Console.WriteLine();
                Console.Write("Answer: ");
                int.TryParse(Console.ReadLine(), out key);

                if (key == 1)
                {
                    threads = 1;

                    threads = 3;
                }

            } while (key != 2);
        }
    }
}
