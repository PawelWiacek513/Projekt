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
            Console.WriteLine("Witajcie w aplikacji, która odbiera wiadomości!");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Email",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.Span);
                    Console.WriteLine($" [x] otrzymano {message}");
                };
                channel.BasicConsume(queue: "Email",
                    autoAck: true,
                    consumer: consumer);
                Console.WriteLine("Wciśnij [Enter], aby wyłączyć aplikację");
                Console.ReadLine();
            }
        }
    }
}
