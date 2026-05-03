// Models/DTO/StudentDto.cs
namespace CollegeManagementSystem.Models.DTO;

public class StudentCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }  // Make nullable to match entity
    public DateTimeOffset DateOfBirth { get; set; }
    // Remove UserId from here - it will be set automatically or from auth
}

public class StudentResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public long UserId { get; set; }  // Add this to response
    public DateTimeOffset DateOfBirth { get; set; } 
    public List<EnrollmentResponseDto> Enrollments { get; set; } = new();
}

public class StudentUpdateDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public long UserId { get; set; }  // Add this for updates
    public DateTime DateOfBirth { get; set; }
}

// Add this filter DTO
public class StudentFilterDto
{
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? NameContains { get; set; }
    public string? EmailDomain { get; set; }
}