namespace CollegeManagementSystem.Models.DTO;

public class RegisterDto
{
    public RegisterDto()
    {
        Email = string.Empty;
        Password = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Phone = string.Empty;
    }

    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Phone { get; set; }
    public int? Age { get; set; }  // Add Age
    public DateTime? DateOfBirth { get; set; }
    public DateTime? HireDate { get; set; }
}