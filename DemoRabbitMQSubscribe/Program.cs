using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRabbitMQSubscribe
{
    class Program
    {
        // CloudAMQP URL in format amqp://user:pass@hostName:port/vhost
        private static string url = ConfigurationManager.AppSettings["CLOUDAMQP_URL"];

        // Create a ConnectionFactory and set the Uri to the CloudAMQP url
        // the connectionfactory is stateless and can safetly be a static resource in your app

        static readonly ConnectionFactory connFactory = new ConnectionFactory();

        static void Main(string[] args)
        {
            connFactory.Uri = url.Replace("amqp://", "amqps://");

            //string message = null;
            using (var conn = connFactory.CreateConnection())
            using (var channel = conn.CreateModel())
            {

                var queueName = "DemoQueue";
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (ch, ea) =>
                {
                    var body = ea.Body;
                    // ... process the message
                    Console.WriteLine("\r\nMessage receive: {0}", Encoding.UTF8.GetString(body));
                    channel.BasicAck(ea.DeliveryTag, false);
                };

                String consumerTag = channel.BasicConsume(queueName, false, consumer);

                Console.WriteLine("Process running. Press any key to exit.");
                Console.ReadKey();

                //// ensure that the queue exists before we access it
                //channel.QueueDeclare("queue1", false, false, false, null);
                //var queueName = "DemoQueue";
                //// do a simple poll of the queue
                //var data = channel.BasicGet(queueName, false);
                //// the message is null if the queue was empty
                //if (data == null) return;
                //// convert the message back from byte[] to a string
                //message = Encoding.UTF8.GetString(data.Body);
                //// ack the message, ie. confirm that we have processed it
                //// otherwise it will be requeued a bit later
                //channel.BasicAck(data.DeliveryTag, false);
            }

        }
    }
}
