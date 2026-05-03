using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Interfaces;
using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;
using Module = CollegeManagementSystem.Data.Entities.Module;

namespace CollegeManagementSystem.Services;

public class CourseService : ICourseService
{
    private readonly AppDbContext _dbContext;

    public CourseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CourseListDto>> GetAllCoursesAsync()
    {
        return await _dbContext.Courses
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Name = c.Name,
                DurationYears = c.DurationYears,
                ModuleCount = c.Modules.Count,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }

    public async Task<CourseResponseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Modules)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return null;

        return new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            DurationYears = course.DurationYears,
            ModuleCount = course.Modules.Count,
            EnrollmentCount = course.Enrollments.Count,
            Modules = course.Modules.Select(m => new ModuleResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Credits = m.Credits,
                CourseId = m.CourseId,
                CourseName = course.Name
            }).ToList(),
            Enrollments = course.Enrollments.Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                CourseId = e.CourseId,
                CourseName = course.Name
            }).ToList()
        };
    }

    public async Task<IEnumerable<ModuleResponseDto>> GetCourseModulesAsync(int courseId)
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

    public async Task<IEnumerable<StudentResponseDto>> GetCourseStudentsAsync(int courseId)
    {
        var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return new List<StudentResponseDto>();

        return await _dbContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Student)
            .Select(e => new StudentResponseDto
            {
                Id = e.Student.Id,
                FirstName = e.Student.FirstName,
                LastName = e.Student.LastName,
                Age = e.Student.Age,
                Email = e.Student.Email,
                Phone = e.Student.Phone,
                DateOfBirth = new DateTimeOffset(e.Student.DateOfBirth, TimeSpan.Zero)
            })
            .ToListAsync();
    }

    public async Task<CourseResponseDto> CreateCourseAsync(CourseCreateDto courseDto)
    {
        var existingCourse = await _dbContext.Courses
            .AnyAsync(c => c.Name == courseDto.Name);

        if (existingCourse)
            throw new InvalidOperationException($"Course with name '{courseDto.Name}' already exists");

        var course = new Course
        {
            Name = courseDto.Name,
            DurationYears = courseDto.DurationYears,
            Modules = new List<Module>(),
            Enrollments = new List<Enrollment>()
        };

        _dbContext.Courses.Add(course);
        await _dbContext.SaveChangesAsync();

        return new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            DurationYears = course.DurationYears,
            ModuleCount = 0,
            EnrollmentCount = 0
        };
    }

    public async Task<ModuleResponseDto> AddModuleToCourseAsync(int courseId, ModuleCreateDto moduleDto)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        var existingModule = await _dbContext.Modules
            .AnyAsync(m => m.CourseId == courseId && m.Title == moduleDto.Title);

        if (existingModule)
            throw new InvalidOperationException($"Module '{moduleDto.Title}' already exists in this course");

        var module = new Module
        {
            Title = moduleDto.Title,
            Credits = moduleDto.Credits,
            CourseId = courseId,
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

    public async Task<bool> UpdateCourseAsync(int id, CourseUpdateDto courseDto)
    {
        var existingCourse = await _dbContext.Courses.FindAsync(id);
        if (existingCourse == null)
            return false;

        if (existingCourse.Name != courseDto.Name)
        {
            var duplicateName = await _dbContext.Courses
                .AnyAsync(c => c.Name == courseDto.Name && c.Id != id);

            if (duplicateName)
                throw new InvalidOperationException($"Course name '{courseDto.Name}' is already in use");
        }

        existingCourse.Name = courseDto.Name;
        existingCourse.DurationYears = courseDto.DurationYears;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Enrollments)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return false;

        if (course.Enrollments.Any())
            throw new InvalidOperationException($"Cannot delete course '{course.Name}' because it has {course.Enrollments.Count} enrolled students");

        if (course.Modules.Any())
            throw new InvalidOperationException($"Cannot delete course '{course.Name}' because it has {course.Modules.Count} modules");

        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<int> BulkInsertCoursesAsync(List<CourseCreateDto> courseDtos)
    {
        var courses = courseDtos.Select(dto => new Course
        {
            Name = dto.Name,
            DurationYears = dto.DurationYears
        }).ToList();

        _dbContext.Courses.AddRange(courses);
        await _dbContext.SaveChangesAsync();
        return courses.Count;
    }

    public async Task<IEnumerable<object>> GetCoursesWithDetailsAsync()
    {
        return await _dbContext.Courses
            .Include(c => c.Modules)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.DurationYears,
                Modules = c.Modules.Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Credits,
                    Instructors = _dbContext.ModuleInstructors
                        .Where(mi => mi.ModuleId == m.Id)
                        .Select(mi => new
                        {
                            mi.Instructor.Id,
                            mi.Instructor.FirstName,
                            mi.Instructor.LastName,
                            mi.Instructor.Email
                        }).ToList()
                }).ToList(),
                Students = c.Enrollments.Select(e => new
                {
                    e.Student.Id,
                    e.Student.FirstName,
                    e.Student.LastName,
                    e.Student.Email
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<int> GetCoursesCountAsync()
    {
        return await _dbContext.Courses.CountAsync();
    }

    public async Task<int> GetTotalCreditsAsync()
    {
        return await _dbContext.Modules.SumAsync(m => m.Credits);
    }

    public async Task<IEnumerable<object>> GetTopEnrolledCoursesAsync(int top)
    {
        return await _dbContext.Courses
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.DurationYears,
                EnrollmentCount = c.Enrollments.Count,
                ModuleCount = c.Modules.Count
            })
            .OrderByDescending(c => c.EnrollmentCount)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IEnumerable<CourseListDto>> SearchCoursesAsync(string name)
    {
        return await _dbContext.Courses
            .Where(c => c.Name.Contains(name))
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Name = c.Name,
                DurationYears = c.DurationYears,
                ModuleCount = c.Modules.Count,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<CourseListDto>> GetCoursesByDurationAsync(int years)
    {
        return await _dbContext.Courses
            .Where(c => c.DurationYears == years)
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Name = c.Name,
                DurationYears = c.DurationYears,
                ModuleCount = c.Modules.Count,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }
}