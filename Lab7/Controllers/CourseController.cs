using Microsoft.AspNetCore.Mvc;

[Route("api/courses")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly RabbitMqService _rabbitMqService;

    public CourseController(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost("add-course")]
    public IActionResult AddCourse([FromBody] CourseDto courseDto)
    {
        if (courseDto == null) return BadRequest("Invalid data");

        // Publish Course Data to RabbitMQ for Enrollment Microservice
        _rabbitMqService.PublishMessage("studentQueue", courseDto);
        _rabbitMqService.PublishMessage("teacherQueue", courseDto);

        return Ok(new { message = "Course added and message sent to Enrollment Service!" });
    }
}

public class CourseDto
{
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public string CourseName { get; set; }
}
