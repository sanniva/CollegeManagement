// Models/DTOs/CourseDto.cs

using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Models.DTO;

public class CourseCreateDto
{
    public string Name { get; set; } = null!;
    public int DurationYears { get; set; }
}

public class CourseUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DurationYears { get; set; }
}

public class CourseResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DurationYears { get; set; }
    public int ModuleCount { get; set; }
    public int EnrollmentCount { get; set; }
    public List<ModuleResponseDto> Modules { get; set; } = new();
    public List<EnrollmentResponseDto> Enrollments { get; set; } = new();
}

public class CourseListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DurationYears { get; set; }
    public int ModuleCount { get; set; }
    public int EnrollmentCount { get; set; }
}