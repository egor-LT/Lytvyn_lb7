using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace task4
{
    class Event
    {
        public int EventId { get; }
        public string Description { get; }

        public Event(int eventId, string description)
        {
            EventId = eventId;
            Description = description;
        }
    }

    
    class EventManager
    {
        private readonly ConcurrentDictionary<int, List<Action<Event>>> subscribers = new ConcurrentDictionary<int, List<Action<Event>>>();
        private int eventIdCounter = 0;

        public event EventHandler<Event> EventPublished;

        
        public void Subscribe(int eventId, Action<Event> handler)
        {
            if (!subscribers.TryGetValue(eventId, out var eventSubscribers))
            {
                eventSubscribers = new List<Action<Event>>();
                subscribers.TryAdd(eventId, eventSubscribers);
            }

            eventSubscribers.Add(handler);
        }

        
        public void Unsubscribe(int eventId, Action<Event> handler)
        {
            if (subscribers.TryGetValue(eventId, out var eventSubscribers))
            {
                eventSubscribers.Remove(handler);
            }
        }

       
        public void PublishEvent(string description)
        {
            int eventId = Interlocked.Increment(ref eventIdCounter);
            var newEvent = new Event(eventId, description);

           
            if (subscribers.TryGetValue(eventId, out var eventSubscribers))
            {
                foreach (var subscriber in eventSubscribers)
                {
                    subscriber(newEvent);
                }
            }

            
            EventPublished?.Invoke(this, newEvent);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EventManager eventManager = new EventManager();

            
            Action<Event> subscriber1 = e => Console.WriteLine($"Subscriber 1 received event: {e.EventId} - {e.Description}");
            Action<Event> subscriber2 = e => Console.WriteLine($"Subscriber 2 received event: {e.EventId} - {e.Description}");

          
            eventManager.Subscribe(1, subscriber1);
            eventManager.Subscribe(2, subscriber2);

            
            eventManager.PublishEvent("Event 1");
            eventManager.PublishEvent("Event 2");

            
            eventManager.Unsubscribe(1, subscriber1);

            
            eventManager.Subscribe(2, e => Console.WriteLine($"Subscriber 3 received event: {e.EventId} - {e.Description}"));

            
            eventManager.PublishEvent("Event 3");

            Console.ReadLine();
        }
    }
}