using Microsoft.AspNetCore.Mvc;
using TeacherService.Data;
using TeacherService.Models;
using TeacherService.Services;
using Microsoft.EntityFrameworkCore;

namespace TeacherService.Controllers
{
    [Route("api/teachers")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly TeacherDbContext _context;
        private readonly RabbitMqProducer _rabbitMqProducer;

        public TeacherController(TeacherDbContext context, RabbitMqProducer rabbitMqProducer)
        {
            _context = context;
            _rabbitMqProducer = rabbitMqProducer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher([FromBody] Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            _rabbitMqProducer.SendMessage(teacher);

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            return Ok(teacher);
        }
    }
}