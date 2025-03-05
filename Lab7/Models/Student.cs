using System;

namespace EnrollmentService.Models
{
    public class Student
    {
        public int Id { get; set; }  // Unique identifier for the student
        public string FirstName { get; set; }  // First name of the student
        public string LastName { get; set; }   // Last name of the student
        public string Email { get; set; }  // Email of the student
        public string PhoneNumber { get; set; }  // Phone number of the student
        public string BranchId { get; set; }  // The branch (tenant) the student belongs to

        // Add any additional fields you may need
        // For example: DateOfBirth, Address, EnrollmentDate, etc.
    }
}