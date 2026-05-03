// Models/DTOs/InstructorDto.cs
namespace CollegeManagementSystem.Models.DTOs;

public class InstructorCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime HireDate { get; set; }
}

public class InstructorUpdateDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime HireDate { get; set; }
}

public class InstructorResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTimeOffset HireDate { get; set; }
    public int ModulesTaught { get; set; }
    public List<ModuleResponseDto> Modules { get; set; } = new();
}