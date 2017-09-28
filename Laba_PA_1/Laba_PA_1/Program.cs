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

            Console.Write("mass: ");
            for (int i = 0; i< mass.Length; i++)
            {
                Console.Write(mass[i] + " ");
            }

            Console.WriteLine();
            Console.Write("Minimums: ");
            for (int i = 0; i < min.Length; i++)
            {
                Console.Write(min[i] + " ");
            }

            minimum = min[0];
            for (int i = 1; i < threads; i++)
            {
                if (min[i] < minimum)
                {
                    minimum = min[i];
                }
            }
            Console.WriteLine();
            Console.WriteLine(minimum);
        }

        public static void FindingMin(object borders)
        {
            AllThreadsBorders border = (AllThreadsBorders)borders;

            foreach (ProcessThread process in Process.GetCurrentProcess().Threads)
            {
                int id = GetCurrentThreadId();
                if (id == process.Id)
                {
                    for (int i = 1; i <= threads; i++)
                    {
                        if (border.NumOfProcessor == i)
                        {
                            process.ProcessorAffinity = (IntPtr)(i);
                        }
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

        static void Main(string[] args)
        {
            long n = 1000000;
            Random rand = new Random();
            int key = 0;
            threads = 0;

            mass = new int[n];
            for(int i = 0; i < n; i++)
            {
                mass[i] = rand.Next(5, 1000);
            }

            do
            {
                Console.WriteLine();
                Console.WriteLine("1. Enter size of massive.");
                Console.WriteLine("2. Enter numbers of threads.");
                Console.WriteLine("3. Start.");
                Console.WriteLine("4. Exit.");
                Console.WriteLine();
                Console.Write("Answer: ");
                int.TryParse(Console.ReadLine(), out key);
                
                if (key == 1)
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.Write("Enter size of massive: ");
                    long.TryParse(Console.ReadLine(), out n);
                    for (int i = 0; i < n; i++)
                    {
                        mass[i] = rand.Next(0, 100);
                    }
                    Console.Clear();
                }

                if (key == 2)
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.Write("Enter number of threads: ");
                    int.TryParse(Console.ReadLine(), out threads);
                    Console.Clear();
                }

                if (key == 3)
                {
                    Start();
                }

            } while (key != 4);
        }
    }
}
