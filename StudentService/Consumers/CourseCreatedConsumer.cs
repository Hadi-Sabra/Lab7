using MassTransit;
using Shared.Contracts.Events;
using System.Threading.Tasks;

public class CourseCreatedConsumer : IConsumer<CourseCreated>
{
    public async Task Consume(ConsumeContext<CourseCreated> context)
    {
        var course = context.Message;
        Console.WriteLine($"[Student/Teacher Service] Received event: New Course - {course.CourseName}");

        // Add logic to store in the respective database
        await Task.CompletedTask;
    }
}