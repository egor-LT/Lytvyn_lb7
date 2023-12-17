using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace task2
{
    class Resource
    {
        public string Name { get; }
        private SemaphoreSlim semaphore;
        private Mutex mutex;

        public Resource(string name, int initialCount)
        {
            Name = name;
            semaphore = new SemaphoreSlim(initialCount, initialCount);
            mutex = new Mutex();
        }

        public void AccessResource(int priority)
        {
            Console.WriteLine($"Thread with priority {priority} trying to access resource {Name}...");

            if (priority > 1)
            {
                semaphore.Wait(); 
                Console.WriteLine($"Thread with priority {priority} acquired resource {Name}.");
            }
            else
            {
                bool mutexAcquired = mutex.WaitOne(TimeSpan.FromSeconds(2)); 

                if (mutexAcquired)
                {
                    semaphore.Wait(); 
                    mutex.ReleaseMutex();
                    Console.WriteLine($"Thread with priority {priority} acquired resource {Name}.");
                }
                else
                {
                    Console.WriteLine($"Thread with priority {priority} couldn't acquire resource {Name}.");
                }
            }
        }

        public void ReleaseResource()
        {
            semaphore.Release();
            Console.WriteLine($"Resource {Name} released.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Resource cpuResource = new Resource("CPU", 1);
            Resource ramResource = new Resource("RAM", 2);
            Resource diskResource = new Resource("Disk", 3);

            
            ThreadPool.QueueUserWorkItem(_ => cpuResource.AccessResource(2)); 
            ThreadPool.QueueUserWorkItem(_ => ramResource.AccessResource(1)); 
            ThreadPool.QueueUserWorkItem(_ => diskResource.AccessResource(1)); 

           
            Thread.Sleep(TimeSpan.FromSeconds(5));
            cpuResource.ReleaseResource();
            ramResource.ReleaseResource();
            diskResource.ReleaseResource();

            Console.ReadLine();
        }
    }
}