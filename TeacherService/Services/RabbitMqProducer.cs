using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace TeacherService.Services
{
    public class RabbitMqProducer
    {
        public void SendMessage(object message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.QueueDeclare("teacherQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            
            channel.BasicPublish(exchange: "", routingKey: "teacherQueue", basicProperties: null, body: body);
        }
    }
}