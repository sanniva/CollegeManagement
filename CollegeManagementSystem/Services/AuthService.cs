// using CollegeManagementSystem.Models.DTO;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using CollegeManagementSystem.Data;
// using CollegeManagementSystem.Data.Entities;
//
// namespace CollegeManagementSystem.Services;
//
// public class AuthService(
//     UserManager<IdentityUser<long>> userManager,
//     RoleManager<IdentityRole<long>> roleManager,
//     IConfiguration configuration,
//     AppDbContext dbContext)
// {
//     public async Task<AuthResponseDto> RegisterStudentAsync(RegisterDto registerDto)
//     {
//         // Check if user exists
//         var userExists = await userManager.FindByEmailAsync(registerDto.Email);
//         if (userExists != null)
//             return new AuthResponseDto 
//             { 
//                 Success = false, 
//                 Message = "User already exists!" 
//             };
//
//         // Create Identity user
//         var user = new IdentityUser<long>
//         {
//             UserName = registerDto.Email,
//             Email = registerDto.Email,
//             EmailConfirmed = true
//         };
//
//         var result = await userManager.CreateAsync(user, registerDto.Password);
//         if (!result.Succeeded)
//         {
//             var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//             return new AuthResponseDto 
//             { 
//                 Success = false, 
//                 Message = $"User creation failed: {errors}" 
//             };
//         }
//
//         // Add to Student role
//         if (await roleManager.RoleExistsAsync("Student"))
//             await userManager.AddToRoleAsync(user, "Student");
//
//         // Create Student entity - FIXED: Added Phone and Age
//         var student = new Student
//         {
//             FirstName = registerDto.FirstName,
//             LastName = registerDto.LastName,
//             Email = registerDto.Email,
//             DateOfBirth = registerDto.DateOfBirth ?? DateTime.UtcNow.AddYears(-18),
//             UserId = user.Id,
//             Phone = registerDto.Phone ?? string.Empty,  // Add this line
//             Age = registerDto.Age ?? 18  // Add this line if Age is required
//         };
//         
//         await dbContext.Students.AddAsync(student);
//         await dbContext.SaveChangesAsync();
//
//         // Generate JWT token
//         var token = await GenerateJwtToken(user);
//
//         return new AuthResponseDto
//         {
//             Success = true,
//             Message = "Student registered successfully!",
//             Token = token,
//             Email = user.Email,
//             Role = "Student",
//             UserId = user.Id
//         };
//     }
//
//     public async Task<AuthResponseDto> RegisterInstructorAsync(RegisterDto registerDto)
//     {
//         // Check if user exists
//         var userExists = await userManager.FindByEmailAsync(registerDto.Email);
//         if (userExists != null)
//             return new AuthResponseDto 
//             { 
//                 Success = false, 
//                 Message = "User already exists!" 
//             };
//
//         // Create Identity user
//         var user = new IdentityUser<long>
//         {
//             UserName = registerDto.Email,
//             Email = registerDto.Email,
//             EmailConfirmed = true
//         };
//
//         var result = await userManager.CreateAsync(user, registerDto.Password);
//         if (!result.Succeeded)
//         {
//             var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//             return new AuthResponseDto 
//             { 
//                 Success = false, 
//                 Message = $"User creation failed: {errors}" 
//             };
//         }
//
//         // Add to Instructor role
//         if (await roleManager.RoleExistsAsync("Instructor"))
//             await userManager.AddToRoleAsync(user, "Instructor");
//
//         // Create Instructor entity
//         var instructor = new Instructor
//         {
//             FirstName = registerDto.FirstName,
//             LastName = registerDto.LastName,
//             Email = registerDto.Email,
//             HireDate = registerDto.HireDate ?? DateTime.UtcNow,
//             UserId = user.Id
//         };
//         
//         await dbContext.Instructors.AddAsync(instructor);
//         await dbContext.SaveChangesAsync();
//
//         // Generate JWT token
//         var token = await GenerateJwtToken(user);
//
//         return new AuthResponseDto
//         {
//             Success = true,
//             Message = "Instructor registered successfully!",
//             Token = token,
//             Email = user.Email,
//             Role = "Instructor",
//             UserId = user.Id
//         };
//     }
//
//     public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
//     {
//         var user = await userManager.FindByEmailAsync(loginDto.Email);
//         if (user == null)
//             return new AuthResponseDto 
//             { 
//                 Success = false, 
//                 Message = "Invalid email or password!" 
//             };
//
//         var isValidPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
//         if (!isValidPassword)
//             return new AuthResponseDto 
//             { 
//                 Success = false, 
//                 Message = "Invalid email or password!" 
//             };
//
//         var roles = await userManager.GetRolesAsync(user);
//         var token = await GenerateJwtToken(user, roles);
//
//         return new AuthResponseDto
//         {
//             Success = true,
//             Message = "Login successful!",
//             Token = token,
//             Email = user.Email,
//             Role = roles.FirstOrDefault() ?? "User",
//             UserId = user.Id
//         };
//     }
//
//     // use signin manager for login and use lock out as well 
//    
//     private async Task<string> GenerateJwtToken(IdentityUser<long> user, IList<string> roles = null)
//     {
//         if (roles == null)
//             roles = await userManager.GetRolesAsync(user);
//
//         var authClaims = new List<Claim>
//         {
//             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//             new Claim(ClaimTypes.Email, user.Email),
//             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//         };
//
//         foreach (var userRole in roles)
//         {
//             authClaims.Add(new Claim(ClaimTypes.Role, userRole));
//         }
//
//         var jwtKey = configuration["Jwt:Key"];
//         var jwtIssuer = configuration["Jwt:Issuer"];
//         var jwtAudience = configuration["Jwt:Audience"];
//         
//         // Add null checking to avoid errors
//         if (string.IsNullOrEmpty(jwtKey))
//             throw new InvalidOperationException("JWT Key is not configured");
//         
//         var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
//
//         // in token --> issuer="SMS",audience="localhost", signinCredentials = -, claims(data,roles), expires(DateTime)make a seperate class JwtService
//         var token = new JwtSecurityToken(
//             issuer: jwtIssuer,
//             audience: jwtAudience,
//             expires: DateTime.Now.AddHours(3),
//             claims: authClaims,
//             signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
//         );
//
//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }
// }


using CollegeManagementSystem.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;

namespace CollegeManagementSystem.Services;

public class AuthService
{
    private readonly UserManager<IdentityUser<long>> _userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public AuthService(
        UserManager<IdentityUser<long>> userManager,
        RoleManager<IdentityRole<long>> roleManager,
        IConfiguration configuration,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    // STUDENT REGISTRATION WITH TRANSACTION 
    public async Task<AuthResponseDto> RegisterStudentAsync(RegisterDto registerDto)
    {
        // Begin transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            // Check if user exists
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                await transaction.RollbackAsync();
                return new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "User already exists!" 
                };
            }

            // Create Identity user
            var user = new IdentityUser<long>
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto 
                { 
                    Success = false, 
                    Message = $"User creation failed: {errors}" 
                };
            }

            // Add to Student role
            if (await _roleManager.RoleExistsAsync("Student"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Student");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new AuthResponseDto 
                    { 
                        Success = false, 
                        Message = "Role assignment failed! Registration rolled back." 
                    };
                }
            }
            else
            {
                await _roleManager.CreateAsync(new IdentityRole<long>("Student"));
                await _userManager.AddToRoleAsync(user, "Student");
            }

            // Create Student entity
            var student = new Student
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                DateOfBirth = registerDto.DateOfBirth ?? DateTime.UtcNow.AddYears(-18),
                UserId = user.Id,
                Phone = registerDto.Phone ?? string.Empty,
                Age = registerDto.Age ?? 18
            };
            
            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            // Commit transaction
            await transaction.CommitAsync();

            // Generate JWT token
            var token = await GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Student registered successfully!",
                Token = token,
                Email = user.Email,
                Role = "Student",
                UserId = user.Id
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = $"Registration failed: {ex.Message}" 
            };
        }
    }

    // INSTRUCTOR REGISTRATION WITH TRANSACTION 
    public async Task<AuthResponseDto> RegisterInstructorAsync(RegisterDto registerDto)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                await transaction.RollbackAsync();
                return new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "User already exists!" 
                };
            }

            var user = new IdentityUser<long>
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto 
                { 
                    Success = false, 
                    Message = $"User creation failed: {errors}" 
                };
            }

            if (await _roleManager.RoleExistsAsync("Instructor"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Instructor");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new AuthResponseDto 
                    { 
                        Success = false, 
                        Message = "Role assignment failed! Registration rolled back." 
                    };
                }
            }
            else
            {
                await _roleManager.CreateAsync(new IdentityRole<long>("Instructor"));
                await _userManager.AddToRoleAsync(user, "Instructor");
            }

            var instructor = new Instructor
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                HireDate = registerDto.HireDate ?? DateTime.UtcNow,
                UserId = user.Id
            };
            
            await _dbContext.Instructors.AddAsync(instructor);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            var token = await GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Instructor registered successfully!",
                Token = token,
                Email = user.Email,
                Role = "Instructor",
                UserId = user.Id
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = $"Registration failed: {ex.Message}" 
            };
        }
    }

    //  LOGIN
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = "Invalid email or password!" 
            };

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isValidPassword)
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = "Invalid email or password!" 
            };

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful!",
            Token = token,
            Email = user.Email,
            Role = roles.FirstOrDefault() ?? "User",
            UserId = user.Id
        };
    }

    //  GENERATE JWT TOKEN 
    private async Task<string> GenerateJwtToken(IdentityUser<long> user, IList<string> roles = null)
    {
        if (roles == null)
            roles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("JWT Key is not configured");
        
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}