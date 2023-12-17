using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lb7
{
    class DistributedSystemNode
    {
        public string NodeName { get; }
        public bool IsActive { get; private set; }

        private readonly List<DistributedSystemNode> connectedNodes;

        public DistributedSystemNode(string nodeName)
        {
            NodeName = nodeName;
            IsActive = true;
            connectedNodes = new List<DistributedSystemNode>();
        }

        public void ConnectNode(DistributedSystemNode node)
        {
            connectedNodes.Add(node);
        }

        public async Task SendMessageAsync(string message)
        {
            foreach (var node in connectedNodes)
            {
                
                await Task.Delay(TimeSpan.FromSeconds(1));

                
                await node.ReceiveMessageAsync(message);
            }
        }

        public async Task ReceiveMessageAsync(string message)
        {
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine($"{NodeName} received message: '{message}'");
        }

        public async Task NotifyStatusAsync()
        {
            while (true)
            {
               
                await Task.Delay(TimeSpan.FromSeconds(5));

                
                foreach (var node in connectedNodes)
                {
                    await node.ReceiveStatusNotificationAsync(NodeName, IsActive);
                }
            }
        }

        public async Task ReceiveStatusNotificationAsync(string nodeName, bool isActive)
        {
           
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine($"Node '{nodeName}' is {(isActive ? "active" : "inactive")}");
        }

        public async Task SimulateSystem()
        {
            
            var statusNotificationTask = NotifyStatusAsync();

          
            var messageTasks = new List<Task>();
            messageTasks.Add(SendMessageAsync("Hello from Node"));

            await Task.WhenAll(statusNotificationTask, Task.WhenAll(messageTasks));
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var nodeA = new DistributedSystemNode("Node A");
            var nodeB = new DistributedSystemNode("Node B");
            var nodeC = new DistributedSystemNode("Node C");

            
            nodeA.ConnectNode(nodeB);
            nodeA.ConnectNode(nodeC);

            nodeB.ConnectNode(nodeA);
            nodeB.ConnectNode(nodeC);

            nodeC.ConnectNode(nodeA);
            nodeC.ConnectNode(nodeB);

            
            await nodeA.SimulateSystem();
        }
    }
}