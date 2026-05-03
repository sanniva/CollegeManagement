using System.ComponentModel.DataAnnotations.Schema;
using CollegeManagementSystem.Data.Entities;

namespace CollegeManagementSystem.Data.Entities;
public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }
    [ForeignKey("StudentId")] 
    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;
}