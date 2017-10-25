using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Laba_PA_3
{
    class Program
    {
        static public long n = 1000;
        static public int threads;
        static public int[,] matrA;
        static public int[,] matrB;
        static public int[,] matrC;
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
            done = 0;
            times = new double[threads + 1];

            for (int i = 0; i < threads; i++)
            {
                AllThreads[i] = new Thread(new ParameterizedThreadStart(Multiplication));
                borders[i] = new AllThreadsBorders();
                borders[i].Left = ((int)n / threads) * i;
                if (i == 2)
                {
                    borders[i].Right = borders[i].Left + ((int)n / threads);
                }
                else borders[i].Right = borders[i].Left + ((int)n / threads - 1);
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

        public static void Multiplication(object borders)
        {
            AllThreadsBorders border = (AllThreadsBorders)borders;
            int result = 0;

            for (int i = 0; i < n; i++)
            {
                for (int p = border.Left; p <= border.Right; p++)
                {
                    int a = 0;
                    int b = 0;
                    do {
                        result += matrA[i, a] * matrB[b, p];
                        a++;
                        b++;
                    } while (a != n);
                    
                    matrC[i, p] = result;
                    result = 0;
                    //Console.WriteLine("I'm done with matrC[" + i + ", " + p + "]");
                }
            }
            done++;
        }

        static void Main(string[] args)
        {
            Random rand = new Random();
            matrA = new int[n,n];
            matrB = new int[n, n];
            matrC = new int[n, n];


            for (int i = 0; i < n; i++)
            {
                for (int p = 0; p < n; p++)
                {
                    matrA[i,p] = rand.Next(1, 10);
                    matrB[i, p] = rand.Next(1, 10);
                }
            }

            //Console.WriteLine("Матрица А");
            //for (int i = 0; i < n; i++)
            //{
            //    for (int p = 0; p < n; p++)
            //    {
            //        Console.Write(matrA[i, p] + " ");
            //    }
            //    Console.WriteLine();
            //}

            //Console.WriteLine();
            //Console.WriteLine("Матрица B");
            //for (int i = 0; i < n; i++)
            //{
            //    for (int p = 0; p < n; p++)
            //    {
            //        Console.Write(matrB[i, p] + " ");
            //    }
            //    Console.WriteLine();
            //}

            threads = 1;
            Start();

            Console.WriteLine();
            Console.WriteLine("1 поток. Время выполнения = " + times[1]);
            double timefirst = times[1];
            //Console.WriteLine("Матрица C");
            //for (int i = 0; i < n; i++)
            //{
            //    for (int p = 0; p < n; p++)
            //    {
            //        Console.Write(matrC[i, p] + " ");
            //    }
            //    Console.WriteLine();
            //}

            threads = 3;
            Start();

            Console.WriteLine();
            Console.WriteLine("3 потока. Время выполнения = " + times[3]);
            Console.WriteLine("Коэффициент: " + timefirst / times[3]);
            //Console.WriteLine("Матрица C");
            //for (int i = 0; i < n; i++)
            //{
            //    for (int p = 0; p < n; p++)
            //    {
            //        Console.Write(matrC[i, p] + " ");
            //    }
            //    Console.WriteLine();
            //}

            Console.ReadKey();
        }
    }
}
