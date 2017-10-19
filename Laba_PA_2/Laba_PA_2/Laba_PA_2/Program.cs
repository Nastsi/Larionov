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
        static public long n = 10;
        static public int threads;
        static public long[] mass;
        static public long[] prefix;
        static public bool allGood;
        static public bool[] good;
        static public int done;
        static public double[] times;

        class AllThreadsBorders
        {
            public int Left;
            public int Right;
            public int NumOfProcessor;
        };

        public static void Start()
        {
            Thread[] AllThreads = new Thread[threads];
            AllThreadsBorders[] borders = new AllThreadsBorders[threads];
            allGood = false;
            done = 0;
            good = new bool[threads];
            times = new double[threads + 1];

            for (int i = 0; i < threads; i++)
            {
                AllThreads[i] = new Thread(new ParameterizedThreadStart(Algorithm));
                borders[i] = new AllThreadsBorders();
                borders[i].Left = (mass.Length / threads) * i;
                if (i == 2)
                {
                    borders[i].Right = borders[i].Left + (mass.Length / threads);
                }
                else borders[i].Right = borders[i].Left + (mass.Length / threads - 1);
                borders[i].NumOfProcessor = i + 1;
                good[i] = false;
            }
            Stopwatch Time = new Stopwatch();
            Time.Start();
            for (int i = 0; i < threads; i++)
            {
                AllThreads[i].Start(borders[i]);
            }

            while (!allGood)
            {
                bool now = true;
                for (int i = 0; i < threads; i++)
                {
                    if (good[i] == false)
                    {
                        now = false;
                        break;
                    }
                }
                if (now == true)
                {
                    allGood = true;
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            AllThreads[threads - 1].Join();

            while (done != threads)
            {
                Thread.Sleep(10);
            }
            
            Time.Stop();
            times[threads] = Time.Elapsed.TotalMilliseconds;

        }

        public static void Algorithm(object borders)
        {

        }

        static void Main(string[] args)
        {
            int key = 0;
            Random rand = new Random();
            mass = new long[n];

            for (int i = 0; i < n; i++)
            {
                mass[i] = rand.Next(1, 50);
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
                    Start();
                }

            } while (key != 2);
        }
    }
}
