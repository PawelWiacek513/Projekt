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
                    channel.ExchangeDeclare(exchange: "Email", type: "fanout");
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine("Wprowadź wiadomość do pracowników: ");
                        string msg = Console.ReadLine();
                        var msgBody = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(exchange: "Email",
                            routingKey: "",
                            basicProperties: null,
                            body: msgBody);
                        Console.WriteLine($"[x] Wysłano wiadomosci{msgBody}");
                    }
                }
            }
            Console.WriteLine("Wciśnij [Enter], aby wyłączyć aplikację");
            Console.ReadLine();

        }

    }
}
