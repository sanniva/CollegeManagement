namespace CollegeManagementSystem.Models.DTO;

public class AuthResponseDto
{
    public AuthResponseDto()
    {
        Message = string.Empty;
        Token = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
    }

    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public long UserId { get; set; }
}