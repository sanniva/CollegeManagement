using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CollegeManagementSystem.Services;

public class JwtService
{
    // public string GenerateToken()
    // {
    //     var jwtOptions = config.GetSection("JWT");
    //     
    //     var secretKey = jwtOptions["SecretKey"];
    //     // var secretKey = "YourSuperSecretKeyHereThatIsAtLeast32CharactersLong123!";
    //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    //     var authSigningCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
    //     
    //     var token = new JwtSecurityToken(
    //         issuer: jwtOptions["Issuer"],
    //         audience: jwtOptions["Audience"],
    //         expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtOptions["expireInMinutes"])),
    //         claims: [],
    //         signingCredentials: authSigningCredentials
    //     );
    //     
    //     return new JwtSecurityTokenHandler().WriteToken(tokenObj);
    // }
}