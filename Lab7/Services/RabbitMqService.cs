using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" }; // Update if needed
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void PublishMessage(string queueName, object message)
    {
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}