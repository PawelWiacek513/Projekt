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
                channel.ExchangeDeclare(exchange: "direct_email", type: "direct");
                for (int i = 0; i < 99; i++)
                {
                    string error = RandomError();
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_email",
                        routingKey: error,
                        basicProperties: null,
                        body: body);
                    PleaseWait();
                    Console.WriteLine(" [{0}] \t Sent \t [{1}] \t {2}", i, error, message);
                }
            }
        }

        private static void PleaseWait()
        {
            var random = new Random();
            int time = random.Next(100, 1000);
            Thread.Sleep(time);
        }

        private static string RandomError()
        {
            var random = new Random();
            var list = new List<string>() { "info", "warning", "error" };
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
