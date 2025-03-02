using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab7.Controllers
{
    [ApiController]
    [Route("api/sample")]
    public class SampleController : ControllerBase
    {
        [HttpGet("students")]
        [Authorize(Policy = "StudentOnly")]
        public IActionResult GetStudentData()
        {
            return Ok("This data is for students only.");
        }

        [HttpGet("teachers")]
        [Authorize(Policy = "TeacherOnly")]
        public IActionResult GetTeacherData()
        {
            return Ok("This data is for teachers only.");
        }
    }
}