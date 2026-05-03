using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/dashboard/statistics
    [HttpGet("statistics")]
    public async Task<ActionResult<DashboardStatisticsDto>> GetDashboardStatistics()
    {
        var statistics = new DashboardStatisticsDto
        {
            TotalStudents = await _context.Students.CountAsync(),
            TotalCourses = await _context.Courses.CountAsync(),
            TotalModules = await _context.Modules.CountAsync(),
            TotalInstructors = await _context.Instructors.CountAsync(),
            TotalEnrollments = await _context.Enrollments.CountAsync()
        };

        return Ok(statistics);
    }

    // GET: api/dashboard/recent-enrollments
    [HttpGet("recent-enrollments")]
    public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetRecentEnrollments([FromQuery] int count = 10)
    {
        var recentEnrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrollmentId)
            .Take(count)
            .Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                CourseId = e.CourseId,
                CourseName = e.Course.Name
            })
            .ToListAsync();

        return Ok(recentEnrollments);
    }

    // GET: api/dashboard/course-enrollment-stats
    [HttpGet("course-enrollment-stats")]
    public async Task<ActionResult<IEnumerable<CourseEnrollmentStatsDto>>> GetCourseEnrollmentStats()
    {
        var stats = await _context.Courses
            .Include(c => c.Enrollments)
            .Include(c => c.Modules)
            .Select(c => new CourseEnrollmentStatsDto
            {
                Id = c.Id,
                Name = c.Name,
                EnrollmentCount = c.Enrollments.Count,
                TotalCredits = c.Modules.Sum(m => m.Credits)
            })
            .OrderByDescending(c => c.EnrollmentCount)
            .ToListAsync();

        return Ok(stats);
    }

    // GET: api/dashboard/module-instructor-stats
    [HttpGet("module-instructor-stats")]
    public async Task<ActionResult<IEnumerable<ModuleInstructorStatsDto>>> GetModuleInstructorStats()
    {
        var stats = await _context.Instructors
            .Select(i => new ModuleInstructorStatsDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                ModulesTaught = _context.ModuleInstructors.Count(mi => mi.InstructorId == i.Id)
            })
            .OrderByDescending(i => i.ModulesTaught)
            .ToListAsync();

        return Ok(stats);
    }

    // GET: api/dashboard/student-age-distribution
    [HttpGet("student-age-distribution")]
    public async Task<ActionResult<IEnumerable<AgeDistributionDto>>> GetStudentAgeDistribution()
    {
        var distribution = await _context.Students
            .GroupBy(s => s.Age)
            .Select(g => new AgeDistributionDto
            {
                Age = g.Key,
                Count = g.Count()
            })
            .OrderBy(d => d.Age)
            .ToListAsync();

        return Ok(distribution);
    }

    // GET: api/dashboard/course-module-count
    [HttpGet("course-module-count")]
    public async Task<ActionResult<IEnumerable<object>>> GetCourseModuleCount()
    {
        var courseModules = await _context.Courses
            .Include(c => c.Modules)
            .Select(c => new
            {
                c.Id,
                c.Name,
                ModuleCount = c.Modules.Count,
                c.DurationYears
            })
            .OrderByDescending(c => c.ModuleCount)
            .ToListAsync();

        return Ok(courseModules);
    }

    // GET: api/dashboard/student-enrollment-history
    [HttpGet("student-enrollment-history")]
    public async Task<ActionResult<object>> GetStudentEnrollmentHistory()
    {
        var studentsWithEnrollments = await _context.Students
            .Include(s => s.Enrollments)
            .Select(s => new
            {
                s.Id,
                s.FirstName,
                s.LastName,
                EnrollmentCount = s.Enrollments.Count
            })
            .ToListAsync();

        var averageEnrollments = studentsWithEnrollments.Average(s => s.EnrollmentCount);
        var maxEnrollments = studentsWithEnrollments.Max(s => s.EnrollmentCount);
        var minEnrollments = studentsWithEnrollments.Min(s => s.EnrollmentCount);

        return Ok(new
        {
            Students = studentsWithEnrollments,
            Statistics = new
            {
                AverageEnrollmentsPerStudent = averageEnrollments,
                MaxEnrollments = maxEnrollments,
                MinEnrollments = minEnrollments,
                TotalStudents = studentsWithEnrollments.Count,
                StudentsWithNoEnrollments = studentsWithEnrollments.Count(s => s.EnrollmentCount == 0),
                StudentsWithMultipleEnrollments = studentsWithEnrollments.Count(s => s.EnrollmentCount > 1)
            }
        });
    }
}