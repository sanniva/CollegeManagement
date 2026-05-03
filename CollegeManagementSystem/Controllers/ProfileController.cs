// Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]  // All endpoints require authentication
public class ProfileController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ProfileController> _logger;
    private readonly string _profileFolder;

    public ProfileController(IWebHostEnvironment environment, ILogger<ProfileController> logger)
    {
        _environment = environment;
        _logger = logger;
        _profileFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "Profiles");
        
        // Create folder if it doesn't exist
        if (!Directory.Exists(_profileFolder))
            Directory.CreateDirectory(_profileFolder);
    }

    //  TASK 4A: UPLOAD PROFILE PICTURE =
    [HttpPost("upload")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        try
        {
            // Validate file exists
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });
            
            // Validate file type (only images)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Only image files (jpg, jpeg, png, gif, webp) are allowed" });
            
            // Validate file size (max 5MB = 5 * 1024 * 1024 bytes)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size must be less than 5MB" });
            
            // Get current user ID from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });
            
            // Delete old profile pictures for this user
            var oldFiles = Directory.GetFiles(_profileFolder, $"{userId}_*");
            foreach (var oldFile in oldFiles)
            {
                System.IO.File.Delete(oldFile);
            }
            
            // Generate unique filename
            var uniqueFileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
            var filePath = Path.Combine(_profileFolder, uniqueFileName);
            
            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var profileUrl = $"/api/profile/download/{uniqueFileName}";
            
            _logger.LogInformation($"Profile picture uploaded for user {userId}: {uniqueFileName}");
            
            return Ok(new 
            { 
                success = true, 
                message = "Profile picture uploaded successfully",
                fileName = uniqueFileName,
                fileUrl = profileUrl,
                size = file.Length,
                contentType = file.ContentType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed");
            return StatusCode(500, new { message = "Upload failed", error = ex.Message });
        }
    }

    // ========== TASK 4B: DOWNLOAD PROFILE PICTURE ==========
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadProfilePicture(string fileName)
    {
        try
        {
            // Get current user ID from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found" });
            
            // Security: Ensure user can only download their own profile
            if (!fileName.StartsWith($"{userId}_"))
                return Unauthorized(new { message = "You can only download your own profile picture" });
            
            var filePath = Path.Combine(_profileFolder, fileName);
            
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Profile picture not found" });
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(Path.GetExtension(fileName));
            
            _logger.LogInformation($"Profile picture downloaded for user {userId}: {fileName}");
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Download failed");
            return StatusCode(500, new { message = "Download failed", error = ex.Message });
        }
    }

    //  GET CURRENT USER'S PROFILE INFO 
    [HttpGet("my-profile")]
    public IActionResult GetMyProfileInfo()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        // Find profile picture
        var profilePicture = Directory.GetFiles(_profileFolder, $"{userId}_*")
            .Select(f => Path.GetFileName(f))
            .FirstOrDefault();
        
        return Ok(new
        {
            userId,
            email,
            roles,
            hasProfilePicture = profilePicture != null,
            profilePictureUrl = profilePicture != null ? $"/api/profile/download/{profilePicture}" : null
        });
    }

    // ========== DELETE PROFILE PICTURE ==========
    [HttpDelete("delete")]
    public IActionResult DeleteProfilePicture()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var files = Directory.GetFiles(_profileFolder, $"{userId}_*");
        
        foreach (var file in files)
        {
            System.IO.File.Delete(file);
        }
        
        return Ok(new { message = "Profile picture deleted successfully" });
    }

    //  HELPER: GET CONTENT TYPE 
    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}