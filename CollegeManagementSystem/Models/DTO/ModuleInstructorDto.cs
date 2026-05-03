// Models/DTOs/ModuleInstructorDto.cs
namespace CollegeManagementSystem.Models.DTOs;

public class ModuleInstructorCreateDto
{
    public int ModuleId { get; set; }
    public int InstructorId { get; set; }
}

public class ModuleInstructorResponseDto
{
    public int ModuleInstructorId { get; set; }
    public int ModuleId { get; set; }
    public string ModuleTitle { get; set; } = null!;
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = null!;
}