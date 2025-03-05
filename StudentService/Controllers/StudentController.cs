using Microsoft.AspNetCore.Mvc;
using StudentService.Data;
using StudentService.Models;
using StudentService.Services;
using Microsoft.EntityFrameworkCore;

namespace StudentService.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext _context;
        private readonly RabbitMqProducer _rabbitMqProducer;

        public StudentController(StudentDbContext context, RabbitMqProducer rabbitMqProducer)
        {
            _context = context;
            _rabbitMqProducer = rabbitMqProducer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            _rabbitMqProducer.SendMessage(student);

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return Ok(student);
        }
    }
}