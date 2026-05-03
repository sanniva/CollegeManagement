using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnrollmentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/enrollments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetEnrollments()
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                CourseId = e.CourseId,
                CourseName = e.Course.Name
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    // GET: api/enrollments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EnrollmentResponseDto>> GetEnrollment(int id)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null)
            return NotFound($"Enrollment with ID {id} not found");

        var response = new EnrollmentResponseDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}",
            CourseId = enrollment.CourseId,
            CourseName = enrollment.Course.Name
        };

        return Ok(response);
    }

    // GET: api/enrollments/student/{studentId}
    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetEnrollmentsByStudent(int studentId)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
        if (!studentExists)
            return NotFound($"Student with ID {studentId} not found");

        var enrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                CourseId = e.CourseId,
                CourseName = e.Course.Name
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    // GET: api/enrollments/course/{courseId}
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetEnrollmentsByCourse(int courseId)
    {
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return NotFound($"Course with ID {courseId} not found");

        var enrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                CourseId = e.CourseId,
                CourseName = e.Course.Name
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    // GET: api/enrollments/statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> GetEnrollmentStatistics()
    {
        var totalEnrollments = await _context.Enrollments.CountAsync();
        
        var enrollmentsByCourse = await _context.Enrollments
            .GroupBy(e => e.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                CourseName = _context.Courses.Where(c => c.Id == g.Key).Select(c => c.Name).FirstOrDefault(),
                Count = g.Count()
            })
            .ToListAsync();

        var enrollmentsByStudent = await _context.Enrollments
            .GroupBy(e => e.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                StudentName = _context.Students.Where(s => s.Id == g.Key).Select(s => $"{s.FirstName} {s.LastName}").FirstOrDefault(),
                Count = g.Count()
            })
            .ToListAsync();

        return Ok(new
        {
            TotalEnrollments = totalEnrollments,
            EnrollmentsByCourse = enrollmentsByCourse,
            EnrollmentsByStudent = enrollmentsByStudent
        });
    }

    // POST: api/enrollments
    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> Create(EnrollmentCreateDto enrollmentDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var student = await _context.Students.FindAsync(enrollmentDto.StudentId);
        if (student == null)
            return NotFound($"Student with ID {enrollmentDto.StudentId} not found");

        var course = await _context.Courses.FindAsync(enrollmentDto.CourseId);
        if (course == null)
            return NotFound($"Course with ID {enrollmentDto.CourseId} not found");

        var existingEnrollment = await _context.Enrollments
            .AnyAsync(e => e.StudentId == enrollmentDto.StudentId && e.CourseId == enrollmentDto.CourseId);

        if (existingEnrollment)
            return Conflict($"Student is already enrolled in this course");

        var enrollment = new Enrollment
        {
            StudentId = enrollmentDto.StudentId,
            CourseId = enrollmentDto.CourseId,
            Student = student,
            Course = course
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        var response = new EnrollmentResponseDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseId = enrollment.CourseId,
            CourseName = course.Name
        };

        return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.EnrollmentId }, response);
    }

    // DELETE: api/enrollments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null)
            return NotFound($"Enrollment with ID {id} not found");

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}