// Models/DTOs/EnrollmentDto.cs
namespace CollegeManagementSystem.Models.DTO;

public class EnrollmentCreateDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
}

public class EnrollmentResponseDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
}