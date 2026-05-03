using System.ComponentModel.DataAnnotations;

namespace CollegeManagementSystem.Data.Entities;

public class Student
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string FirstName { get; set; } = string.Empty;  // Add default
    
    public string LastName { get; set; } = string.Empty;  // Add default
    
    [Range(15, 40)]
    public int Age { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;  // Add default
    
    public string? Phone { get; set; }  // Make nullable with ?
    
    public DateTime DateOfBirth { get; set; }
    
    public long UserId { get; set; }
    
    // Navigation properties
    public List<Enrollment> Enrollments { get; set; } = new();
}