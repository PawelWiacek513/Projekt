using System;
using System.Text;
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
                channel.ExchangeDeclare(exchange: "Email", type: "fanout");
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                    exchange: "Email",
                    routingKey: "");
                Console.WriteLine("[x] Waiting for email. ");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.Span);
                    Console.WriteLine($" [x] otrzymano {message}");
                };
                channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);
                Console.WriteLine("Wciśnij [Enter], aby wyłączyć aplikację");
                Console.ReadLine();
            }
        }
    }
}

