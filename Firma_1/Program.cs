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
            // otwarcie połączenia
            using (var connection = factory.CreateConnection())
            {
                // utworzenie kanału komunikacji
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Email",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine("Wprowadz wiadomość dla pracownika: ");
                        string msg = Console.ReadLine();
                        var msgBody = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(exchange: "",
                            routingKey: "Email",
                            basicProperties: null,
                            body: msgBody);
                        Console.WriteLine($" [x] wysłano wiadomosci {msgBody}");
                    }
                }
            }
            Console.WriteLine("Wciśnij [Enter], aby zamknąć");
            Console.ReadLine();

        }

    }
}
