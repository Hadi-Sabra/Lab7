using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public string CourseName { get; set; }
    }
}
