// Models/DTOs/ModuleDto.cs
namespace CollegeManagementSystem.Models.DTOs;

public class ModuleCreateDto
{
    public string Title { get; set; } = null!;
    public int Credits { get; set; }
    public int CourseId { get; set; }
}

public class ModuleUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int Credits { get; set; }
    public int CourseId { get; set; }
}

public class ModuleResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int Credits { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public List<InstructorResponseDto> Instructors { get; set; } = new();
}