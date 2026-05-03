using Microsoft.AspNetCore.Identity;
using CollegeManagementSystem.Data.Entities;

namespace CollegeManagementSystem.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole<long>> roleManager,
        UserManager<IdentityUser<long>> userManager)
    {
        // Seed roles
        string[] roleNames = { "Admin", "Instructor", "Student" };
        
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<long>
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
            }
        }
        
        // Seed admin user (create manually via code)
        var adminEmail = "admin@college.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new IdentityUser<long>
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}