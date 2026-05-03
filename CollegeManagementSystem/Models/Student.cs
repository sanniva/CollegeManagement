using System.ComponentModel.DataAnnotations;

namespace CollegeManagementSystem.Models
{
    public class Student
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(16, 60)]
        public int Age { get; set; }

        [Required]
        public string Course { get; set; }
        
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        
    }
}