using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class Program
    {
        static int m;
        static int a;
        static int c;
        static int u0;

        static int[] un = new int[threadRun];

        static int size = 10000000;
        static int[] array = new int[size];
        static int[] parallelArray = new int[size];

        const int threadRun = 4;

        static double timeElapsed;
        static double parallelTimeElapsed;

        static void Main(string[] args)
        {
            double w;
            Console.Write("Выберите число w для модуля 2^w от 1 до 32: ");
            double.TryParse(Console.ReadLine(), out w);

            m = (int)Math.Pow(2, w);
            Random r = new Random();
            a = r.Next(1, m);
            c = r.Next(2, m);
            u0 = r.Next(2, m);

            ExecuteInOneThread();

            for (int i = 0; i < threadRun; i++)
            {
                un[i] = calculateUn((size / 4) * i);
            }

            ExecuteInManyThread(threadRun);

            Console.WriteLine(threadRun + " Thread Coeffcient = " + timeElapsed / parallelTimeElapsed);

            bool check = true;

            for (int i = 0; i < size / threadRun; i++)
            {
                if (array[i] != parallelArray[i])
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                Console.WriteLine("Results are equal");
            }
            else
            {
                Console.WriteLine("Results are not equal");
            }
            Console.ReadKey();
        }
        

        private static void ExecuteInOneThread()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            array[0] = u0;

            for (int i = 1; i < array.Length; i++)
            {
                array[i] = (a * array[i - 1] + c) % m;
            }

            sw.Stop();

            Console.WriteLine("1 Thread Time Elapsed = " + (sw.Elapsed.TotalMilliseconds * 1000));
            timeElapsed = sw.Elapsed.TotalMilliseconds * 1000;
        }

        private static void ExecuteInManyThread(int threadsCount)
        {
            int[] counters = new int[threadsCount];

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Task[] tasks = new Task[threadsCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                int j = i;
                tasks[i] = Task.Factory.StartNew(() => CalculateCountInSubArray(j, array.Length / tasks.Length));
            }

            Task.WaitAll(tasks);

            Console.WriteLine(threadsCount + " Thread Time Elapsed = " + (sw.Elapsed.TotalMilliseconds * 1000));
            parallelTimeElapsed = sw.Elapsed.TotalMilliseconds * 1000;
        }

        private static void CalculateCountInSubArray(int index, int size)
        {
            int startIndex = index * size;
            int endIndex = (index + 1) * size;

            if (index == 0)
            {
                parallelArray[startIndex] = u0;

                for (int i = startIndex + 1; i < endIndex && i < parallelArray.Length; i++)
                {
                    parallelArray[i] = (a * parallelArray[i - 1] + c) % m;
                }
            }
            else
            {
                int ui = un[index];

                parallelArray[startIndex] = ui;

                for (int i = startIndex + 1; i < endIndex && i < parallelArray.Length; i++)
                {
                    parallelArray[i] = (a * parallelArray[i - 1] + c) % m;
                }
            }
        }

        private static int calculateUn(int n)
        {
            int divide = 1;

            for (int i = 1; i < n; i++)
            {
                divide += (int)BigInteger.ModPow(a, i, m);
            }

            divide %= m;

            int un = (((int)BigInteger.ModPow(a, n, m)) * u0 + divide * c) % m;
            return un;
        }
    }
}

