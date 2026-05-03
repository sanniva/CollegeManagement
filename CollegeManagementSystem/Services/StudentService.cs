using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Interfaces;
using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _dbContext;

    public StudentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync()
    {
        return await _dbContext.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Age = s.Age,
                Email = s.Email,
                Phone = s.Phone,
                DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero),
                Enrollments = s.Enrollments.Select(e => new EnrollmentResponseDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
    {
        var student = await _dbContext.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return null;

        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Age = student.Age,
            Email = student.Email,
            Phone = student.Phone,
            DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero),
            Enrollments = student.Enrollments.Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = $"{student.FirstName} {student.LastName}",
                CourseId = e.CourseId,
                CourseName = e.Course.Name
            }).ToList()
        };
    }

    public async Task<IEnumerable<CourseListDto>> GetStudentCoursesAsync(int studentId)
    {
        var studentExists = await _dbContext.Students.AnyAsync(s => s.Id == studentId);
        if (!studentExists)
            return new List<CourseListDto>();

        return await _dbContext.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.Course)
            .Select(e => new CourseListDto
            {
                Id = e.Course.Id,
                Name = e.Course.Name,
                DurationYears = e.Course.DurationYears,
                ModuleCount = e.Course.Modules.Count,
                EnrollmentCount = e.Course.Enrollments.Count
            })
            .ToListAsync();
    }

    public async Task<StudentResponseDto> CreateStudentAsync(StudentCreateDto studentDto)
    {
        var existingStudent = await _dbContext.Students
            .AnyAsync(s => s.Email == studentDto.Email);

        if (existingStudent)
            throw new InvalidOperationException($"Student with email {studentDto.Email} already exists");

        var student = new Student
        {
            FirstName = studentDto.FirstName,
            LastName = studentDto.LastName,
            Age = studentDto.Age,
            Email = studentDto.Email,
            Phone = studentDto.Phone,
            DateOfBirth = studentDto.DateOfBirth.UtcDateTime,
            Enrollments = new List<Enrollment>()
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Age = student.Age,
            Email = student.Email,
            Phone = student.Phone,
            DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero)
        };
    }

    public async Task<bool> UpdateStudentAsync(int id, StudentUpdateDto studentDto)
    {
        var existingStudent = await _dbContext.Students.FindAsync(id);
        if (existingStudent == null)
            return false;

        if (existingStudent.Email != studentDto.Email)
        {
            var duplicateEmail = await _dbContext.Students
                .AnyAsync(s => s.Email == studentDto.Email && s.Id != id);

            if (duplicateEmail)
                throw new InvalidOperationException($"Email {studentDto.Email} is already in use");
        }

        existingStudent.FirstName = studentDto.FirstName;
        existingStudent.LastName = studentDto.LastName;
        existingStudent.Age = studentDto.Age;
        existingStudent.Email = studentDto.Email;
        existingStudent.Phone = studentDto.Phone;
        existingStudent.DateOfBirth = studentDto.DateOfBirth.Kind == DateTimeKind.Utc 
            ? studentDto.DateOfBirth 
            : DateTime.SpecifyKind(studentDto.DateOfBirth, DateTimeKind.Utc);

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _dbContext.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return false;

        if (student.Enrollments.Any())
            throw new InvalidOperationException($"Cannot delete student with {student.Enrollments.Count} active enrollments");

        _dbContext.Students.Remove(student);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<int> BulkInsertStudentsAsync(List<StudentCreateDto> studentDtos)
    {
        var students = studentDtos.Select(dto => new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Age = dto.Age,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth.UtcDateTime
        }).ToList();

        _dbContext.Students.AddRange(students);
        await _dbContext.SaveChangesAsync();
        return students.Count;
    }

    public async Task<IEnumerable<StudentResponseDto>> GetStudentsWithCoursesAsync()
    {
        return await _dbContext.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Age = s.Age,
                Email = s.Email,
                Phone = s.Phone,
                DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero),
                Enrollments = s.Enrollments.Select(e => new EnrollmentResponseDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<int> GetStudentsCountAsync()
    {
        return await _dbContext.Students.CountAsync();
    }

    public async Task<IEnumerable<object>> GetFullStudentDetailsAsync()
    {
        return await _dbContext.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c.Modules)
            .Select(s => new
            {
                s.Id,
                s.FirstName,
                s.LastName,
                s.Email,
                s.Age,
                Courses = s.Enrollments.Select(e => new
                {
                    e.Course.Id,
                    e.Course.Name,
                    e.Course.DurationYears,
                    Modules = e.Course.Modules.Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Credits
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();
    }
}