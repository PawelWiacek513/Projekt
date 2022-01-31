using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
                for (int i = 0; i < 99; i++)
                {
                    string bindingKey = RandomBindingKey();
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "topic_logs",
                        routingKey: bindingKey,
                        basicProperties: null,
                        body: body);
                    PleaseWait();
                    Console.WriteLine(" [{0}] \t Sent \t [{1}] \t {2}", i, bindingKey, message);
                }
            }
        }

        private static void PleaseWait()
        {
            var random = new Random();
            int time = random.Next(100, 1000);
            Thread.Sleep(time);
        }

        private static string RandomBindingKey()
        {
            var random = new Random();
            var list = new List<string>() { "app1.warning", "app2.critical", "app2.critical","app2.warning" };
            int index = random.Next(list.Count);
            return list[index];
        }

        private static string GetMessage(string[] args)
        {
            var date = DateTime.Now.ToLongTimeString();
            var msg = ((args.Length > 0) ? string.Join(" ", args) : "wysłano");
            return String.Format("{0} [{1}]", msg, date);
        }
    }
}
