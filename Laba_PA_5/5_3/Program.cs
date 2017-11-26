using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;
using Meta.Numerics.Functions;

namespace _5_3
{
    class Program
    {
        [DllImport("kernel32")]
        static extern int GetCurrentThreadId();
        static public int proc;
        static public int[] job;
        static public ConcurrentBag<int>[] itog;
        static public bool[] good;
        static public bool allGood;
        static public int[] public_a;
        static public int[] public_b;
        static public long[] timer;
        static public int repitter = 2;
        //static public int module = 997;
        //static public int module = 99991;
        //static public int module = 97;
        //static public int module = 9999991;
        static public int module = 251;

        class borders
        {
            public int LeftBorder;
            public int RightBorder;
            public int Proc;
            public int Answer;
        };


        static void Work(object boards)
        {
            int processors = proc;
            int m = module;
            int[] local_a = public_a;
            int[] local_b = public_b;

            borders c = (borders)boards;
            foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
            {
                int utid = GetCurrentThreadId();
                if (utid == pt.Id)
                {
                    if (c.Proc == 1)
                    {
                        pt.ProcessorAffinity = (IntPtr)(1);
                    }
                    else if (c.Proc == 2)
                    {
                        pt.ProcessorAffinity = (IntPtr)(2);
                    }
                    else if (c.Proc == 3)
                    {
                        pt.ProcessorAffinity = (IntPtr)(4);
                    }
                    else if (c.Proc == 4)
                    {
                        pt.ProcessorAffinity = (IntPtr)(8);
                    }
                    else
                    {
                        pt.ProcessorAffinity = (IntPtr)(15);
                    }
                    break;
                }
            }
            good[c.Proc - 1] = true;

            while (!allGood)
            {
                Thread.Sleep(1);
            }
            for (int z = 0; z < 4; z++)
            {
                for (int repit = 0; repit < repitter; repit++)
                {
                    for (int n = c.LeftBorder; n < c.RightBorder; n++)
                    {
                        int x;
                        int y;
                        int q;
                        int r;
                        int x1 = 0;
                        int x2 = 1;
                        int y1 = 1;
                        int y2 = 0;
                        int b = m;
                        int a = ((local_a[z] * n) + local_b[z]) % m;
                        while (b > 0)
                        {
                            q = a / b;
                            r = a - q * b;
                            x = x2 - q * x1;
                            y = y2 - q * y1;
                            a = b;
                            b = r;
                            x2 = x1;
                            x1 = x;
                            y2 = y1;
                            y1 = y;
                        }
                        if (x2 < 0) x2 += m;
                        itog[c.Answer].Add(x2);
                    }
                }
                job[z]++;
                while(job[z] != processors)
                {
                    Thread.Sleep(10);
                }
            }
        }

        static double Analyze(int[] mass)
        {
            double[] X = new double[mass.Length * 4];
            Console.WriteLine("Рассчитываю дискретное преобразование фурье");
            for(int p = 0; p < X.Length; p++)
            {
                X[p] = 0;
                for(int q = 0; q < mass.Length; q++)
                {
                    int tek = mass[q];
                    for(int r = 0; r < 8; r++)
                    {                      
                        Complex element = Complex.Exp(Complex.Divide(Complex.Multiply(new Complex(2 * Math.PI * (q * 8 + r) * p, 0), new Complex(0, 1)), new Complex(mass.Length * 8, 0)));
                        if (tek % 2 == 1)
                        {
                            X[p] += element.Real;
                        }
                        else
                        {
                            X[p] -= element.Real;
                        }
                    }
                    tek /= 2;
                }
                X[p] = Math.Abs(X[p]);
            }
            double T = Math.Sqrt(Math.Log(1 / 0.05) * mass.Length * 8);
            Console.WriteLine("T = " + T);
            double N0 = 0.95 * mass.Length * 8 / 2;
            Console.WriteLine("N0 = " + N0);
            double N1 = 0;
            for(int p = 0; p < X.Length; p++)
            {
                if(X[p] > 0.01 && X[p] < T)
                {
                    N1++;
                }
            }
            Console.WriteLine("N1 = " + N1);
            double d = (N1 - N0) / Math.Sqrt(mass.Length * 8 * 0.95 * 0.05 / 4);
            Console.WriteLine("d = " + d);
            double P = AdvancedMath.Erfc(Math.Abs(d) / Math.Sqrt(2));
            Console.WriteLine("P = {0:F20}", P);
            return P;
        }

        static void Splitting(int ans)
        {
            allGood = false;
            Thread[] threads = new Thread[proc];
            good = new bool[proc];
            for (int p = 0; p < proc; p++)
            {
                good[p] = false;
                borders tek = new borders();
                tek.Proc = p + 1;
                tek.Answer = ans;
                tek.LeftBorder = module / proc * p;
                if (p == proc - 1)
                {
                    tek.RightBorder = module;
                }
                else
                {
                    tek.RightBorder = module / proc * (p + 1);
                }

                threads[p] = new Thread(new ParameterizedThreadStart(Work));
                threads[p].Start(tek);
            }
            job = new int[4];
            for(int p = 0; p < 4; p++)
            {
                job[p] = 0;
            }
            bool temp = false;
            while (true)
            {
                temp = true;
                for (int p = 0; p < proc; p++)
                {
                    temp &= good[p];
                }
                if (temp) break;
            }
            Stopwatch watch = new Stopwatch();
            watch.Start();
            allGood = true;
            threads[proc - 1].Join();
            while (true)
            {
                if (job[3] == proc)
                {
                    break;
                }
                Thread.Sleep(10);
            }
            watch.Stop();

            timer[ans] = watch.ElapsedMilliseconds;
        }

        static void Main(string[] args)
        {
            Random rand = new Random();
            timer = new long[2];
            itog = new ConcurrentBag<int>[2];
            itog[0] = new ConcurrentBag<int>();
            itog[1] = new ConcurrentBag<int>();
            int g = rand.Next(100);
            int x0 = rand.Next(100);
            public_a = new int[4];
            public_b = new int[4];
            for(int p = 0; p < 4; p++)
            {
                x0 = (x0 * g) % module;
                public_a[p] = x0;
                x0 = (x0 * g) % module;
                public_b[p] = x0;
            }
            Console.WriteLine("Работа 1 потока...");
            proc = 1;
            Splitting(0);
            Console.WriteLine("Время работы: " + timer[0] + "мс");
            Console.WriteLine();

            proc = 4;
            Console.WriteLine("Работа " + proc + " потоков...");
            Splitting(1);
            Console.WriteLine("Время работы: " + timer[1] + "мс");
            Console.WriteLine();

            //bool different = false;
            //for (int p = 0; p < itog[0].Count; p++)
            //{
            //    if (itog[0].ElementAt(p) != itog[1].ElementAt(p))
            //    {
            //        different = true;
            //        break;
            //    }
            //}
            //if (different)
            //{
            //    Console.WriteLine("Получены различные последовательности");
            //}
            //else
            //{
            //    Console.WriteLine("Последовательности идентичны");
            //}
            
            Console.WriteLine();
            //Console.WriteLine("Анализирую первую последовательность");
            //double p1 = Analyze(itog[0].ToArray());
            //Console.WriteLine();
            Console.WriteLine("Анализирую последовательность");
            double p2 = Analyze(itog[1].ToArray());
            Console.WriteLine();
            //if(p2 > p1)
            //{
            //    Console.WriteLine("Вторая последовательность является более случайной");
            //}
            //else
            //{
            //    Console.WriteLine("Первая последовательность является более случайной");
            //}
            Console.Read();
        }
    }
}
