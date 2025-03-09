using Lab7.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Events; // Use the shared event

[ApiController]
[Route("api/courses")]
public class CourseController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CourseController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto courseDto)
    {
        var newCourse = new Course
        {
            Id = 123, 
            Name = courseDto.Name,
            Description = courseDto.Description
        };

        await _publishEndpoint.Publish(new CourseCreated(newCourse.Id, newCourse.Name, newCourse.Description));

        return Ok(new { message = "Course created and event published" });
    }
}

public class CourseDto
{
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public string CourseName { get; set; }
}
