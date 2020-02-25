using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ThreadigTasks
{
    abstract class ThreadProcessor<T>
    {
        private readonly T[] array;
        private readonly Task[] threads;

        public ThreadProcessor(T[] array, int countOfThreads)
        {
            this.array = array;
            threads = new Task[countOfThreads];
        }

        public async Task Start()
        {
            for (int i = 0; i < threads.Length; i++)
                threads[i] = Task.Run(() => Proc(i));

            await Task.WhenAll(threads);
            Console.WriteLine("All task is done");
        }

        protected void Proc(int indexOfThread)
        {
            var countOfThreads = threads.Length;

            var countItemsForOneThread = array.Length / countOfThreads;
            var startIndex = indexOfThread * countItemsForOneThread;

            if (indexOfThread == countOfThreads - 1)
                countItemsForOneThread = array.Length;

            for (int i = startIndex; i < countItemsForOneThread; i++)
            {
                array[i] = ProcItem(i, indexOfThread);
            }
        }

        protected abstract T ProcItem(int index, int threadIndex); 
    }

    class ThreadRandomProcessor : ThreadProcessor<int>
    {
        private readonly Random[] randoms;

        public ThreadRandomProcessor(int[] array, int countOfThreads)
            : base(array, countOfThreads)
        {
            randoms = new Random[countOfThreads];
            for (int i = 0; i < randoms.Length; i++)
            {
                randoms[i] = new Random(i);
            }
        }

        protected override int ProcItem(int index, int threadIndex)
        {
            return randoms[threadIndex].Next();
        }
    }

    class ThreadFuncProcessor<T> : ThreadProcessor<T>
    {
        private Func<int, T> genFunc;

        public ThreadFuncProcessor(T[] array, int countOfThreads, Func<int, T> func)
            : base(array, countOfThreads)
        {
            genFunc = func;
        }

        protected override T ProcItem(int index, int threadIndex)
        {
            return genFunc(index);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Number of threads: ");
            int threadCount = int.Parse(Console.ReadLine());
            var processor = new ThreadFuncProcessor<int>(new int[500000000], threadCount, i => i + 2000);

            var sw = new Stopwatch();
            sw.Start();

            var task = processor.Start();
            Console.WriteLine("Threads starts!");
            task.Wait();

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}
