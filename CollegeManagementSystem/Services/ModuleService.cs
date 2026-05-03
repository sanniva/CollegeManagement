using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Interfaces;
using CollegeManagementSystem.Models.DTOs;
using Module = CollegeManagementSystem.Data.Entities.Module;

namespace CollegeManagementSystem.Services;

public class ModuleService : IModuleService
{
    private readonly AppDbContext _dbContext;

    public ModuleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ModuleResponseDto>> GetAllModulesAsync()
    {
        return await _dbContext.Modules
            .Include(m => m.Course)
            .Select(m => new ModuleResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Credits = m.Credits,
                CourseId = m.CourseId,
                CourseName = m.Course.Name
            })
            .ToListAsync();
    }

    public async Task<ModuleResponseDto?> GetModuleByIdAsync(int id)
    {
        var module = await _dbContext.Modules
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (module == null)
            return null;

        return new ModuleResponseDto
        {
            Id = module.Id,
            Title = module.Title,
            Credits = module.Credits,
            CourseId = module.CourseId,
            CourseName = module.Course.Name
        };
    }

    public async Task<IEnumerable<ModuleResponseDto>> GetModulesByCourseAsync(int courseId)
    {
        var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return new List<ModuleResponseDto>();

        return await _dbContext.Modules
            .Where(m => m.CourseId == courseId)
            .Select(m => new ModuleResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Credits = m.Credits,
                CourseId = m.CourseId,
                CourseName = m.Course.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ModuleResponseDto>> GetModulesByCreditsAsync(int credits)
    {
        return await _dbContext.Modules
            .Where(m => m.Credits == credits)
            .Select(m => new ModuleResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Credits = m.Credits,
                CourseId = m.CourseId,
                CourseName = m.Course.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<InstructorResponseDto>> GetModuleInstructorsAsync(int moduleId)
    {
        var moduleExists = await _dbContext.Modules.AnyAsync(m => m.Id == moduleId);
        if (!moduleExists)
            return new List<InstructorResponseDto>();

        return await _dbContext.ModuleInstructors
            .Where(mi => mi.ModuleId == moduleId)
            .Include(mi => mi.Instructor)
            .Select(mi => new InstructorResponseDto
            {
                Id = mi.Instructor.Id,
                FirstName = mi.Instructor.FirstName,
                LastName = mi.Instructor.LastName,
                Email = mi.Instructor.Email,
                HireDate = new DateTimeOffset(mi.Instructor.HireDate, TimeSpan.Zero),
                ModulesTaught = 0 // This will be calculated separately if needed
            })
            .ToListAsync();
    }

    public async Task<ModuleResponseDto> CreateModuleAsync(ModuleCreateDto moduleDto)
    {
        var course = await _dbContext.Courses.FindAsync(moduleDto.CourseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {moduleDto.CourseId} not found");

        var existingModule = await _dbContext.Modules
            .AnyAsync(m => m.CourseId == moduleDto.CourseId && m.Title == moduleDto.Title);

        if (existingModule)
            throw new InvalidOperationException($"Module '{moduleDto.Title}' already exists in this course");

        var module = new Module
        {
            Title = moduleDto.Title,
            Credits = moduleDto.Credits,
            CourseId = moduleDto.CourseId,
            Course = course
        };

        _dbContext.Modules.Add(module);
        await _dbContext.SaveChangesAsync();

        return new ModuleResponseDto
        {
            Id = module.Id,
            Title = module.Title,
            Credits = module.Credits,
            CourseId = module.CourseId,
            CourseName = course.Name
        };
    }

    public async Task<bool> UpdateModuleAsync(int id, ModuleUpdateDto moduleDto)
    {
        var existingModule = await _dbContext.Modules.FindAsync(id);
        if (existingModule == null)
            return false;

        existingModule.Title = moduleDto.Title;
        existingModule.Credits = moduleDto.Credits;
        existingModule.CourseId = moduleDto.CourseId;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteModuleAsync(int id)
    {
        var module = await _dbContext.Modules.FindAsync(id);
        if (module == null)
            return false;

        var hasAssignments = await _dbContext.ModuleInstructors
            .AnyAsync(mi => mi.ModuleId == id);

        if (hasAssignments)
            throw new InvalidOperationException($"Cannot delete module with active instructor assignments");

        _dbContext.Modules.Remove(module);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}