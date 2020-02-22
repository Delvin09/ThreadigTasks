using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ThreadigTasks
{
    class Program
    {
        static int[] array = new int[500000000];
        static Thread[] threads;

        static void RandomGen(object state)
        {
            var indexOfThread = (int)state;
            var countOfThreads = threads.Length;

            Random random = new Random(indexOfThread);

            var countItemsForOneThread = array.Length / countOfThreads;
            var startIndex = indexOfThread * countItemsForOneThread;

            if (indexOfThread == countOfThreads - 1)
                countItemsForOneThread = array.Length;

            for (int i = startIndex; i < countItemsForOneThread; i++)
            {
                array[i] = random.Next();
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Number of threads: ");
            int threadCount = int.Parse(Console.ReadLine());
            threads = new Thread[threadCount];

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(RandomGen);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(i);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}
