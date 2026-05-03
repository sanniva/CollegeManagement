using System.ComponentModel.DataAnnotations;


namespace CollegeManagementSystem.Data.Entities; 

public class Course
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DurationYears { get; set; }
    
    // Navigation Properties
    public List<Enrollment> Enrollments { get; set; } = new();
    public List<Module> Modules { get; set; } = new();
}