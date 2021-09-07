using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp2
{
    public class RabbitMQService
    {
        private readonly string _hostName = "localhost";
        public IConnection GetRabbitMQConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = _hostName
            };
            return connectionFactory.CreateConnection();
        }
    }
    public class Publisher
    {
        private readonly RabbitMQService _rabbitMQService;
        public Publisher(string queueName, string message)
        {
            _rabbitMQService = new RabbitMQService();

            using (var connection = _rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, false, false, false, null);

                    channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(message));

                    //Console.WriteLine("{0} queue'su üzerine, \"{1}\" mesajı yazıldı.", queueName, message);
                }
            }
        }
    }
    public class Consumer
    {
        private readonly RabbitMQService _rabbitMQService;
        private string getMessage { get; set; }
        public Consumer(string queueName)
        {
            _rabbitMQService = new RabbitMQService();

            using (var connection = _rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        this.getMessage = Encoding.UTF8.GetString(body);
                    };
                    channel.BasicCancel(channel.BasicConsume(queueName, true, consumer));
                    //channel.BasicConsume(queueName, true, consumer);
                }
            }
        }
        public string GetMessage()
        {
            return getMessage;
        }
    }
}
