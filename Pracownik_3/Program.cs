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
                channel.ExchangeDeclare(exchange: "direct_email", type: "direct");
                var queueName = channel.QueueDeclare().QueueName;
                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: {0} [info] [warrning] [error]", Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine("Press [enter] to exit");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }
                foreach (var error in args)
                {
                    Console.WriteLine(String.Format("Bind for {0}", error));
                    channel.QueueBind(queue: queueName,
                        exchange: "direct_email",
                        routingKey: error);
                }
                Console.WriteLine(" [x] waiting for messages");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var Body = ea.Body;
                    var message = Encoding.UTF8.GetString(Body.Span);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}' \t : \t '{1}'", routingKey, message);
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
