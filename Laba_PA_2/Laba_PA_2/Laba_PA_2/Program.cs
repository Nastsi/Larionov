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
        static public long n = 100000000;
        static public int threads;
        static public double[] mass;
        static public double[] prefix;
        static public int done;
        static public double[] times;
        static public bool[] done1;

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
            done = 0;
            times = new double[threads + 1];
            done1 = new bool[threads];

            for (int i = 0; i < threads; i++)
            {
                done1[i] = false;
            }

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
            }
            Stopwatch Time = new Stopwatch();
            Time.Start();
            for (int i = 0; i < threads; i++)
            {
                AllThreads[i].Start(borders[i]);
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
            AllThreadsBorders border = (AllThreadsBorders)borders;
            

            for (int i = border.Left; i <= border.Right; i++)
            {
                if (i == border.Left)
                {
                    prefix[i] = mass[i];
                }
                else prefix[i] = mass[i] + prefix[i - 1];
            }

            while (done1[0] != true)
            {
                if (border.NumOfProcessor == 1)
                {
                    done1[0] = true;
                }
                else Thread.Sleep(1);
            }
            
            if (border.NumOfProcessor == 3)
            {
                while(done1[1] != true)
                {
                    Thread.Sleep(1);
                }
            }

            do
            {
                if (border.NumOfProcessor == 1)
                    break;
                else if (done1[border.NumOfProcessor - 2] == true)
                {
                    for (int i = border.Left - 1; i <= border.Right; i++)
                    {
                        prefix[i] = mass[i] + prefix[i - 1];
                    }
                    done1[border.NumOfProcessor - 1] = true;
                }
            } while (done1[border.NumOfProcessor - 1] == false);

            done++;
        }

        static void Main(string[] args)
        {
            int key = 0;
            Random rand = new Random();
            mass = new double[n];
            prefix = new double[n];

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
                    for (int i = 0; i < n; i++)
                    {
                        mass[i] = rand.NextDouble() * 100;
                    }

                    threads = 1;
                    Start();
                    Console.WriteLine("1 thread. Last prefix = " + prefix[n - 1] + ". Time =  " + times[1]);

                    threads = 3;
                    Start();
                    Console.WriteLine("3 threads. Last prefix = " + prefix[n - 1] + ". Time =  " + times[3]);
                }

            } while (key != 2);
        }
    }
}
