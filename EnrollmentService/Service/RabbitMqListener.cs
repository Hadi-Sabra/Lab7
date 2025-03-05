using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using EnrollmentService.Data;
using EnrollmentService.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

public class RabbitMqListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqListener(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "studentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "teacherQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var studentConsumer = new EventingBasicConsumer(_channel);
        studentConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var student = JsonConvert.DeserializeObject<Enrollment>(message);

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>();
                dbContext.Enrollments.Add(student);
                await dbContext.SaveChangesAsync();
            }
        };

        var teacherConsumer = new EventingBasicConsumer(_channel);
        teacherConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var teacher = JsonConvert.DeserializeObject<Enrollment>(message);

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>();
                dbContext.Enrollments.Add(teacher);
                await dbContext.SaveChangesAsync();
            }
        };

        _channel.BasicConsume(queue: "studentQueue", autoAck: true, consumer: studentConsumer);
        _channel.BasicConsume(queue: "teacherQueue", autoAck: true, consumer: teacherConsumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
