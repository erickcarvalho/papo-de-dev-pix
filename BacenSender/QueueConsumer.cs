using System;
using System.Text;
//dotnet add package RabbitMQ.Client --version 6.1.0
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace BacenSender
{
    public class QueueConsumer
    {
        public static void Start()
        {
            var connection = RabbitConn.GetChannel();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} - Mensagem Recebida - Signed");

                BacenConn.SendMessage(message);

                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} - Mensagem Enviada - Bacen");                

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "pix",
                        routingKey: "pix_route_audit",
                        basicProperties: null,
                        body: body);  

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "pix_signed",autoAck: false,consumer: consumer);
            Console.WriteLine("Channel state:" + channel.IsOpen);
        }

    }

}