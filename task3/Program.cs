using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace task3
{
    class OperationLog
    {
        private readonly object lockObject = new object();
        private readonly List<string> log = new List<string>();

        public void AddLogEntry(int threadId, string operation)
        {
            lock (lockObject)
            {
                string logEntry = $"Thread {threadId} at {DateTime.Now}: {operation}";
                Console.WriteLine(logEntry);
                log.Add(logEntry);
            }
        }
    }

    class Program
    {
        static OperationLog operationLog = new OperationLog();
        static int threadCount = 3;

        static void SimulateOperation(int threadId)
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                string operation = $"Operation {i + 1}";
                operationLog.AddLogEntry(threadId, operation);
                Thread.Sleep(random.Next(100, 500)); 
            }
        }

        static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCount; i++)
            {
                int currentThreadId = i;
                Thread thread = new Thread(() => SimulateOperation(currentThreadId));
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("All threads completed their operations.");
            Console.ReadLine();
        }
    }
}