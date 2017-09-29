using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Laba_PA_1
{
    class Program
    {
        [DllImport("kernel32")]
        static extern int GetCurrentThreadId();
        static public int threads;
        static public int[] mass;
        static public int[] min;
        static public int minimum;
        static public bool[] good;
        static public bool allGood;
        static public int done;
        static public int[] result;
        static public double times;

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
            min = new int[threads];

            for (int i = 0; i < threads; i++)
            {
                AllThreads[i] = new Thread(new ParameterizedThreadStart(FindingMin));
                borders[i] = new AllThreadsBorders();
                borders[i].Left = (mass.Length / threads) * i;
                borders[i].Right = borders[i].Left + (mass.Length / threads - 1);
                borders[i].NumOfProcessor = i + 1;
                good[i] = false;
            }
            result = new int[threads];
            for (int p = 0; p < threads; p++)
            {
                result[p] = 0;
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

            minimum = min[0];
            for (int i = 1; i < threads; i++)
            {
                if (min[i] < minimum)
                {
                    minimum = min[i];
                }
            }

            Thread[] AllThreads2 = new Thread[threads];
            allGood = false;
            done = 0;
            for (int i = 0; i < threads; i++)
            {
                AllThreads2[i] = new Thread(new ParameterizedThreadStart(Compare));
                good[i] = false;
            }
            for (int i = 0; i < threads; i++)
            {
                AllThreads2[i].Start(borders[i]);
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
            AllThreads2[threads - 1].Join();

            while (done != threads)
            {
                Thread.Sleep(10);
            }
            Time.Stop();
            times = Time.Elapsed.TotalMilliseconds;

        }

        public static void FindingMin(object borders)
        {
            AllThreadsBorders border = (AllThreadsBorders)borders;

            foreach (ProcessThread process in Process.GetCurrentProcess().Threads)
            {
                int id = GetCurrentThreadId();
                if (id == process.Id)
                {
                    if (border.NumOfProcessor == 1)
                    {
                        process.ProcessorAffinity = (IntPtr)(1);
                    }
                    else if (border.NumOfProcessor == 2)
                    {
                        process.ProcessorAffinity = (IntPtr)(2);
                    }
                    else if (border.NumOfProcessor == 3)
                    {
                        process.ProcessorAffinity = (IntPtr)(4);
                    }
                    else if (border.NumOfProcessor == 4)
                    {
                        process.ProcessorAffinity = (IntPtr)(8);
                    }
                    else if (border.NumOfProcessor == 5)
                    {
                        process.ProcessorAffinity = (IntPtr)(15);
                    }
                }
            }
            good[border.NumOfProcessor - 1] = true;

            while (!allGood)
            {
                Thread.Sleep(10);
            }

            min[border.NumOfProcessor - 1] = mass[border.Left];

            for (int i = border.Left; i <= border.Right; i++)
            {
                if (mass[i] < min[border.NumOfProcessor - 1])
                {
                    min[border.NumOfProcessor - 1] = mass[i];
                }
            }
            done++;
        }

        public static void Compare(object borders)
        {
            AllThreadsBorders border = (AllThreadsBorders)borders;

            foreach (ProcessThread process in Process.GetCurrentProcess().Threads)
            {
                int id = GetCurrentThreadId();
                if (id == process.Id)
                {
                    if (border.NumOfProcessor == 1)
                    {
                        process.ProcessorAffinity = (IntPtr)(1);
                    }
                    else if (border.NumOfProcessor == 2)
                    {
                        process.ProcessorAffinity = (IntPtr)(2);
                    }
                    else if (border.NumOfProcessor == 3)
                    {
                        process.ProcessorAffinity = (IntPtr)(4);
                    }
                    else if (border.NumOfProcessor == 4)
                    {
                        process.ProcessorAffinity = (IntPtr)(8);
                    }
                    else if (border.NumOfProcessor == 5)
                    {
                        process.ProcessorAffinity = (IntPtr)(15);
                    }
                }
            }
            good[border.NumOfProcessor - 1] = true;

            while (!allGood)
            {
                Thread.Sleep(10);
            }

            for (int i = border.Left; i <= border.Right; i++)
            {
                if (mass[i] == min[border.NumOfProcessor - 1])
                {
                    result[border.NumOfProcessor - 1]++;
                }
            }
            done++;
        }

        static void Main(string[] args)
        {
            long n = 300000000;
            Random rand = new Random();
            int key = 0;
            threads = 0;

            mass = new int[n];
            for (int i = 0; i < n; i++)
            {
                mass[i] = rand.Next(5, 100);
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
                    for (int i = 1; i <= 5; i++)
                    {
                        threads = i;
                        Start();
                        int tek2 = 0;
                        for (int p = 0; p < threads; p++)
                        {
                            tek2 += result[p];
                        }
                        Console.WriteLine();
                        Console.WriteLine("Time: " + times + ". Result: " + tek2);
                    }                    
                }

            } while (key != 2);
        }
    }
}
