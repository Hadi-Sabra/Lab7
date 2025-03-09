using Microsoft.AspNetCore.Mvc;
using EnrollmentService.Data;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentDbContext _context;

        public EnrollmentController(EnrollmentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments()
        {
            var enrollments = await _context.Enrollments.ToListAsync();
            return Ok(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollStudent([FromBody] Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEnrollments), new { id = enrollment.Id }, enrollment);
        }
    }
}