// using Microsoft.AspNetCore.Mvc;
// using CollegeManagementSystem.Models;
// using Microsoft.Extensions.Options;
//
// namespace CollegeManagementSystem.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class StudentController : ControllerBase
//     {
//         // In-memory student list (acts like database)
//         // private static List<Student> students = new List<Student>
//         // {
//         //     new Student { Id = "NP01MS7A240036", Name = "Sanjana Shakya", Age = 20, Course = "BSc Computing" },
//         //     new Student { Id = "NP01MS7A240037", Name = "Aarav Karki", Age = 22, Course = "BBA" },
//         //     new Student { Id = "NP01MS7A240038", Name = "Prisha Gurung", Age = 19, Course = "BIT" }
//         // };
//         
//         private static readonly List<Student> students= [];
//         
//         IConfiguration _configuration;
//
//         public StudentController(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }
//         
//         // a) GET: api/student/getall
//         [HttpGet("getall")]
//         public ActionResult<List<Student>> GetAllStudents()
//         {
//             return Ok(students);
//         }
//
//         // b) GET: api/student/{id}
//         [HttpGet("{id}")]
//         public ActionResult<Student> GetStudentById(string id)
//         {
//             var student = students.FirstOrDefault(s => s.Id == id);
//
//             if (student == null)
//                 return NotFound("Student not found.");
//
//             return Ok(student);
//         }
//
//         // c) POST: api/student/add
//         [HttpPost("add")]
//         public ActionResult AddStudent([FromBody] Student student)
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);
//
//             if (students.Any(s => s.Id == student.Id))
//                 return BadRequest("Student with this ID already exists.");
//
//             students.Add(student);
//             return Ok("Student added successfully.");
//         }
//
//         // d) PUT: api/student/update
//         [HttpPut("update")]
//         public ActionResult UpdateStudent([FromBody] Student updatedStudent)
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);
//
//             var existingStudent = students.FirstOrDefault(s => s.Id == updatedStudent.Id);
//
//             if (existingStudent == null)
//                 return NotFound("Student not found.");
//
//             existingStudent.Name = updatedStudent.Name;
//             existingStudent.Age = updatedStudent.Age;
//             existingStudent.Course = updatedStudent.Course;
//
//             return Ok("Student updated successfully.");
//         }
//
//         // e) DELETE: api/student/delete/{id}
//         [HttpDelete("delete/{id}")]
//         public ActionResult DeleteStudent(string id)
//         {
//             var student = students.FirstOrDefault(s => s.Id == id);
//
//             if (student == null)
//                 return NotFound("Student not found.");
//
//             students.Remove(student);
//             return Ok("Student deleted successfully.");
//         }
//         
//         // IOptionsSnapshot real time update
//         // IOptions doesn't update in real time
//         // IOptionMonitor euta matra obj change garcha ani changes haru monitor garirakhcha
//         
//         [HttpGet("/setting")]
//         public IActionResult GetStudentFromSetting(IOptionsMonitor<Student> option)
//         {
//             Student student = option.CurrentValue;
//             return Ok(student);
//             
//             // int id =Convert.ToInt32(_configuration["Student:Id"]);
//             // string name = _configuration["Student:Name"]!;
//             // int age = Convert.ToInt32(_configuration["Student:Age"]!);
//             // string email =_configuration["Student:Email"]!;
//             // string course =_configuration["Student:Course"]!;
//             //
//             // return Ok(
//             //     new
//             //     {
//             //         Id = id,
//             //         Name = name,
//             //         Age = age,
//             //         Email = email,
//             //         Course = course
//             //     }
//             // );
//         }
//     }
// }

// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using CollegeManagementSystem.Data;
// using CollegeManagementSystem.Data.Entities;
// using CollegeManagementSystem.Models.DTO;
//
// namespace CollegeManagementSystem.Controllers;
//
// [Route("api/[controller]")]
// [ApiController]
// public class StudentsController : ControllerBase
// {
//     private readonly AppDbContext _context;
//
//     public StudentsController(AppDbContext context)
//     {
//         _context = context;
//     }
//
//     // GET: api/students
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetStudents()
//     {
//         var students = await _context.Students
//             .Include(s => s.Enrollments)
//             .ThenInclude(e => e.Course)
//             .Select(s => new StudentResponseDto
//             {
//                 Id = s.Id,
//                 FirstName = s.FirstName,
//                 LastName = s.LastName,
//                 Age = s.Age,
//                 Email = s.Email,
//                 Phone = s.Phone,
//                 DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero), // Convert DateTime to DateTimeOffset
//                 Enrollments = s.Enrollments.Select(e => new EnrollmentResponseDto
//                 {
//                     EnrollmentId = e.EnrollmentId,
//                     StudentId = e.StudentId,
//                     StudentName = $"{s.FirstName} {s.LastName}",
//                     CourseId = e.CourseId,
//                     CourseName = e.Course.Name
//                 }).ToList()
//             })
//             .ToListAsync();
//
//         return Ok(students);
//     }
//
//     // GET: api/students/{id}
//     [HttpGet("{id:int}")]
//     public async Task<ActionResult<StudentResponseDto>> GetStudent(int id)
//     {
//         var student = await _context.Students
//             .Include(s => s.Enrollments)
//             .ThenInclude(e => e.Course)
//             .FirstOrDefaultAsync(s => s.Id == id);
//
//         if (student == null)
//             return NotFound($"Student with ID {id} not found");
//
//         var response = new StudentResponseDto
//         {
//             Id = student.Id,
//             FirstName = student.FirstName,
//             LastName = student.LastName,
//             Age = student.Age,
//             Email = student.Email,
//             Phone = student.Phone,
//             DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero), // Convert DateTime to DateTimeOffset
//             Enrollments = student.Enrollments.Select(e => new EnrollmentResponseDto
//             {
//                 EnrollmentId = e.EnrollmentId,
//                 StudentId = e.StudentId,
//                 StudentName = $"{student.FirstName} {student.LastName}",
//                 CourseId = e.CourseId,
//                 CourseName = e.Course.Name
//             }).ToList()
//         };
//
//         return Ok(response);
//     }
//
//     // GET: api/students/{id}/courses
//     [HttpGet("{id:int}/courses")]
//     public async Task<IActionResult> GetStudentCourses(int id)
//     {
//         var studentExists = await _context.Students.AnyAsync(s => s.Id == id);
//         if (!studentExists)
//             return NotFound($"Student with ID {id} not found");
//
//         var courses = await _context.Enrollments
//             .Where(e => e.StudentId == id)
//             .Include(e => e.Course)
//             .Select(e => new CourseListDto
//             {
//                 Id = e.Course.Id,
//                 Name = e.Course.Name,
//                 DurationYears = e.Course.DurationYears,
//                 ModuleCount = e.Course.Modules.Count,
//                 EnrollmentCount = e.Course.Enrollments.Count
//             })
//             .ToListAsync();
//
//         return Ok(courses);
//     }
//
//     // POST: api/students
//     [HttpPost]
//     public async Task<ActionResult<StudentResponseDto>> Create(StudentCreateDto studentDto)
//     {
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
//
//         var existingStudent = await _context.Students
//             .AnyAsync(s => s.Email == studentDto.Email);
//
//         if (existingStudent)
//             return Conflict($"Student with email {studentDto.Email} already exists");
//
//         var student = new Student
//         {
//             FirstName = studentDto.FirstName,
//             LastName = studentDto.LastName,
//             Age = studentDto.Age,
//             Email = studentDto.Email,
//             Phone = studentDto.Phone,
//             // Convert DateTimeOffset to DateTime (UTC)
//             DateOfBirth = studentDto.DateOfBirth.UtcDateTime,
//             Enrollments = new List<Enrollment>()
//         };
//
//         _context.Students.Add(student);
//         await _context.SaveChangesAsync();
//
//         var response = new StudentResponseDto
//         {
//             Id = student.Id,
//             FirstName = student.FirstName,
//             LastName = student.LastName,
//             Age = student.Age,
//             Email = student.Email,
//             Phone = student.Phone,
//             DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero)
//         };
//
//         return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, response);
//     }
//
//     // PUT: api/students/{id}
//     [HttpPut("{id:int}")]
//     public async Task<IActionResult> Update(int id, StudentUpdateDto studentDto)
//     {
//         if (id != studentDto.Id)
//             return BadRequest("ID mismatch");
//
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
//
//         var existingStudent = await _context.Students.FindAsync(id);
//         if (existingStudent == null)
//             return NotFound($"Student with ID {id} not found");
//
//         if (existingStudent.Email != studentDto.Email)
//         {
//             var duplicateEmail = await _context.Students
//                 .AnyAsync(s => s.Email == studentDto.Email && s.Id != id);
//
//             if (duplicateEmail)
//                 return Conflict($"Email {studentDto.Email} is already in use");
//         }
//
//         existingStudent.FirstName = studentDto.FirstName;
//         existingStudent.LastName = studentDto.LastName;
//         existingStudent.Age = studentDto.Age;
//         existingStudent.Email = studentDto.Email;
//         existingStudent.Phone = studentDto.Phone;
//         // Convert DateTime to UTC if needed
//         existingStudent.DateOfBirth = studentDto.DateOfBirth.Kind == DateTimeKind.Utc 
//             ? studentDto.DateOfBirth 
//             : DateTime.SpecifyKind(studentDto.DateOfBirth, DateTimeKind.Utc);
//
//         await _context.SaveChangesAsync();
//         return NoContent();
//     }
//
//     // DELETE: api/students/{id}
//     [HttpDelete("{id:int}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         var student = await _context.Students
//             .Include(s => s.Enrollments)
//             .FirstOrDefaultAsync(s => s.Id == id);
//
//         if (student == null)
//             return NotFound($"Student with ID {id} not found");
//
//         if (student.Enrollments.Any())
//             return BadRequest($"Cannot delete student with {student.Enrollments.Count} active enrollments");
//
//         _context.Students.Remove(student);
//         await _context.SaveChangesAsync();
//
//         return NoContent();
//     }
//
//     // POST: api/students/bulk
//     [HttpPost("bulk")]
//     public async Task<IActionResult> BulkInsert(List<StudentCreateDto> studentDtos)
//     {
//         if (studentDtos == null || !studentDtos.Any())
//             return BadRequest("Student list is empty");
//
//         var students = studentDtos.Select(dto => new Student
//         {
//             FirstName = dto.FirstName,
//             LastName = dto.LastName,
//             Age = dto.Age,
//             Email = dto.Email,
//             Phone = dto.Phone,
//             DateOfBirth = dto.DateOfBirth.UtcDateTime // Convert DateTimeOffset to DateTime UTC
//         }).ToList();
//
//         _context.Students.AddRange(students);
//         await _context.SaveChangesAsync();
//
//         return Ok(new { Count = students.Count });
//     }
//
//     // GET: api/students/with-courses
//     [HttpGet("with-courses")]
//     public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetWithCourses()
//     {
//         var students = await _context.Students
//             .Include(s => s.Enrollments)
//             .ThenInclude(e => e.Course)
//             .Select(s => new StudentResponseDto
//             {
//                 Id = s.Id,
//                 FirstName = s.FirstName,
//                 LastName = s.LastName,
//                 Age = s.Age,
//                 Email = s.Email,
//                 Phone = s.Phone,
//                 DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero),
//                 Enrollments = s.Enrollments.Select(e => new EnrollmentResponseDto
//                 {
//                     EnrollmentId = e.EnrollmentId,
//                     StudentId = e.StudentId,
//                     StudentName = $"{s.FirstName} {s.LastName}",
//                     CourseId = e.CourseId,
//                     CourseName = e.Course.Name
//                 }).ToList()
//             })
//             .ToListAsync();
//
//         return Ok(students);
//     }
//
//     // GET: api/students/count
//     [HttpGet("count")]
//     public async Task<ActionResult<int>> GetCount() =>
//         await _context.Students.CountAsync();
//
//     // GET: api/students/full-details
//     [HttpGet("full-details")]
//     public async Task<ActionResult<IEnumerable<object>>> GetFullDetails()
//     {
//         var students = await _context.Students
//             .Include(s => s.Enrollments)
//             .ThenInclude(e => e.Course)
//             .ThenInclude(c => c.Modules)
//             .Select(s => new
//             {
//                 s.Id,
//                 s.FirstName,
//                 s.LastName,
//                 s.Email,
//                 s.Age,
//                 DateOfBirth = s.DateOfBirth,
//                 Courses = s.Enrollments.Select(e => new
//                 {
//                     e.Course.Id,
//                     e.Course.Name,
//                     e.Course.DurationYears,
//                     Modules = e.Course.Modules.Select(m => new
//                     {
//                         m.Id,
//                         m.Title,
//                         m.Credits
//                     }).ToList()
//                 }).ToList()
//             })
//             .ToListAsync();
//
//         return Ok(students);
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(AppDbContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ========== TASK 1: IQueryable vs IEnumerable ==========
    
    // TASK 1A: IEnumerable Implementation (Less Performant)
    [HttpGet("filtered/enumerable")]
    public async Task<IActionResult> GetFilteredStudents_IEnumerable([FromQuery] StudentFilterDto filter)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // BAD: Fetches ALL records from database first
        IEnumerable<Student> students = await _context.Students.ToListAsync();
        
        // Then filters in memory (CPU intensive)
        if (filter.MinAge.HasValue)
            students = students.Where(s => s.Age >= filter.MinAge.Value);
        
        if (filter.MaxAge.HasValue)
            students = students.Where(s => s.Age <= filter.MaxAge.Value);
        
        // Filter by name contains
        if (!string.IsNullOrEmpty(filter.NameContains))
            students = students.Where(s => 
                (s.FirstName + " " + s.LastName).Contains(filter.NameContains, StringComparison.OrdinalIgnoreCase));
        
        // Filter by email domain
        if (!string.IsNullOrEmpty(filter.EmailDomain))
            students = students.Where(s => 
                s.Email.EndsWith(filter.EmailDomain, StringComparison.OrdinalIgnoreCase));
        
        var result = students.Select(s => new StudentResponseDto
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Age = s.Age,
            Email = s.Email,
            Phone = s.Phone,
            UserId = s.UserId,
            DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero)
        });
        
        stopwatch.Stop();
        var resultList = result.ToList();
        _logger.LogInformation($"IEnumerable Query Time: {stopwatch.ElapsedMilliseconds}ms, Records: {resultList.Count}");
        
        return Ok(new 
        { 
            method = "IEnumerable (Memory Filtering)", 
            executionTimeMs = stopwatch.ElapsedMilliseconds, 
            totalRecords = resultList.Count,
            data = resultList 
        });
    }
    
    // TASK 1B: IQueryable Implementation (Performant)
    [HttpGet("filtered/iqueryable")]
    public async Task<IActionResult> GetFilteredStudents_IQueryable([FromQuery] StudentFilterDto filter)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // GOOD: Builds query without executing immediately
        IQueryable<Student> query = _context.Students;
        
        // Filters applied as SQL WHERE clause (database side)
        if (filter.MinAge.HasValue)
            query = query.Where(s => s.Age >= filter.MinAge.Value);
        
        if (filter.MaxAge.HasValue)
            query = query.Where(s => s.Age <= filter.MaxAge.Value);
        
        // Filter by name contains
        if (!string.IsNullOrEmpty(filter.NameContains))
            query = query.Where(s => 
                (s.FirstName + " " + s.LastName).Contains(filter.NameContains));
        
        // Filter by email domain
        if (!string.IsNullOrEmpty(filter.EmailDomain))
            query = query.Where(s => s.Email.EndsWith(filter.EmailDomain));
        
        // Only fetch filtered data from database
        var result = await query
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Age = s.Age,
                Email = s.Email,
                Phone = s.Phone,
                UserId = s.UserId,
                DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero)
            })
            .ToListAsync();
        
        stopwatch.Stop();
        _logger.LogInformation($"IQueryable Query Time: {stopwatch.ElapsedMilliseconds}ms, Records: {result.Count}");
        
        return Ok(new 
        { 
            method = "IQueryable (Database Filtering)", 
            executionTimeMs = stopwatch.ElapsedMilliseconds, 
            totalRecords = result.Count,
            data = result 
        });
    }
    
    // TASK 1C: Performance Comparison
    [HttpGet("performance-compare")]
    public async Task<IActionResult> ComparePerformance()
    {
        var results = new List<object>();
        
        // Clear any cached data
        _context.ChangeTracker.Clear();
        
        // Test 1: IEnumerable (fetches all, then filters)
        var enumerableStopwatch = Stopwatch.StartNew();
        var allStudents = await _context.Students.ToListAsync();
        var enumerableResult = allStudents
            .Where(s => s.Age >= 20 && s.Age <= 25)
            .Take(50)
            .ToList();
        enumerableStopwatch.Stop();
        
        results.Add(new 
        { 
            method = "IEnumerable", 
            timeMs = enumerableStopwatch.ElapsedMilliseconds,
            totalStudentsInDb = allStudents.Count,
            recordsFiltered = enumerableResult.Count,
            explanation = "Fetched ALL students from DB first, then filtered in memory",
            performance = "Slow for large datasets - loads entire table"
        });
        
        // Test 2: IQueryable (filters at database level)
        var queryableStopwatch = Stopwatch.StartNew();
        var queryableResult = await _context.Students
            .Where(s => s.Age >= 20 && s.Age <= 25)
            .Take(50)
            .ToListAsync();
        queryableStopwatch.Stop();
        
        results.Add(new 
        { 
            method = "IQueryable", 
            timeMs = queryableStopwatch.ElapsedMilliseconds,
            totalStudentsInDb = allStudents.Count,
            recordsFiltered = queryableResult.Count,
            explanation = "Only fetched filtered students from DB (WHERE clause applied at SQL level)",
            performance = "Fast for large datasets - optimized SQL query"
        });
        
        var performanceAdvantage = enumerableStopwatch.ElapsedMilliseconds - queryableStopwatch.ElapsedMilliseconds;
        
        return Ok(new
        {
            title = "IQueryable vs IEnumerable Performance Analysis",
            message = performanceAdvantage > 0 
                ? $"IQueryable is {performanceAdvantage}ms faster! That's {(performanceAdvantage * 100 / Math.Max(enumerableStopwatch.ElapsedMilliseconds, 1))}% faster."
                : "Performance is similar for small datasets",
            recommendation = "Always use IQueryable when filtering database queries. It translates LINQ to SQL WHERE clauses.",
            fasterMethod = performanceAdvantage > 0 ? "IQueryable" : "Similar",
            comparison = results,
            whenToUse = new
            {
                IEnumerable = "Use when data is already in memory (List, Array) or for very small datasets (< 100 records)",
                IQueryable = "🗄Use when querying databases, especially with filtering, sorting, or paging"
            },
            sqlExample = new
            {
                IEnumerable = "SELECT * FROM Students (then filter in C# memory)",
                IQueryable = "SELECT * FROM Students WHERE Age BETWEEN 20 AND 25 LIMIT 50"
            }
        });
    }

    // GET: api/students/filter-by-age (Simple age filter)
    [HttpGet("filter-by-age")]
    public async Task<IActionResult> GetStudentsByAge([FromQuery] int minAge = 0, [FromQuery] int maxAge = 100)
    {
        var students = await _context.Students
            .Where(s => s.Age >= minAge && s.Age <= maxAge)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Age = s.Age,
                Email = s.Email,
                Phone = s.Phone,
                UserId = s.UserId,
                DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero)
            })
            .ToListAsync();
        
        return Ok(new { filter = $"Age between {minAge} and {maxAge}", count = students.Count, data = students });
    }

    // ========== EXISTING ENDPOINTS (Updated with UserId) ==========
    
    // GET: api/students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetStudents()
    {
        var students = await _context.Students
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
                UserId = s.UserId,
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

        return Ok(students);
    }

    // GET: api/students/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentResponseDto>> GetStudent(int id)
    {
        var student = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound($"Student with ID {id} not found");

        var response = new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Age = student.Age,
            Email = student.Email,
            Phone = student.Phone,
            UserId = student.UserId,
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

        return Ok(response);
    }

    // GET: api/students/{id}/courses
    [HttpGet("{id:int}/courses")]
    public async Task<IActionResult> GetStudentCourses(int id)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.Id == id);
        if (!studentExists)
            return NotFound($"Student with ID {id} not found");

        var courses = await _context.Enrollments
            .Where(e => e.StudentId == id)
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

        return Ok(courses);
    }

    // POST: api/students
    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> Create(StudentCreateDto studentDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingStudent = await _context.Students
            .AnyAsync(s => s.Email == studentDto.Email);

        if (existingStudent)
            return Conflict($"Student with email {studentDto.Email} already exists");

        var student = new Student
        {
            FirstName = studentDto.FirstName,
            LastName = studentDto.LastName,
            Age = studentDto.Age,
            Email = studentDto.Email,
            Phone = studentDto.Phone,
            DateOfBirth = studentDto.DateOfBirth.UtcDateTime,
            // UserId will be set from the authenticated user or leave as 0
            // You can get it from the JWT token if needed
            UserId = 0, // Placeholder - set from authentication
            Enrollments = new List<Enrollment>()
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var response = new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Age = student.Age,
            Email = student.Email,
            Phone = student.Phone,
            UserId = student.UserId,
            DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero)
        };

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, response);
    }

    // POST: api/students (Alternative with UserId from auth)
    [HttpPost("with-auth")]
    [Authorize]
    public async Task<ActionResult<StudentResponseDto>> CreateWithAuth(StudentCreateDto studentDto)
    {
        // Get UserId from the authenticated user
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userIdClaim, out long userId))
            return Unauthorized("User ID not found");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingStudent = await _context.Students
            .AnyAsync(s => s.Email == studentDto.Email);

        if (existingStudent)
            return Conflict($"Student with email {studentDto.Email} already exists");

        var student = new Student
        {
            FirstName = studentDto.FirstName,
            LastName = studentDto.LastName,
            Age = studentDto.Age,
            Email = studentDto.Email,
            Phone = studentDto.Phone,
            DateOfBirth = studentDto.DateOfBirth.UtcDateTime,
            UserId = userId, // Set from authenticated user
            Enrollments = new List<Enrollment>()
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var response = new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Age = student.Age,
            Email = student.Email,
            Phone = student.Phone,
            UserId = student.UserId,
            DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero)
        };

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, response);
    }

    // PUT: api/students/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, StudentUpdateDto studentDto)
    {
        if (id != studentDto.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingStudent = await _context.Students.FindAsync(id);
        if (existingStudent == null)
            return NotFound($"Student with ID {id} not found");

        if (existingStudent.Email != studentDto.Email)
        {
            var duplicateEmail = await _context.Students
                .AnyAsync(s => s.Email == studentDto.Email && s.Id != id);

            if (duplicateEmail)
                return Conflict($"Email {studentDto.Email} is already in use");
        }

        existingStudent.FirstName = studentDto.FirstName;
        existingStudent.LastName = studentDto.LastName;
        existingStudent.Age = studentDto.Age;
        existingStudent.Email = studentDto.Email;
        existingStudent.Phone = studentDto.Phone;
        existingStudent.DateOfBirth = studentDto.DateOfBirth.Kind == DateTimeKind.Utc 
            ? studentDto.DateOfBirth 
            : DateTime.SpecifyKind(studentDto.DateOfBirth, DateTimeKind.Utc);
        existingStudent.UserId = studentDto.UserId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/students/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound($"Student with ID {id} not found");

        if (student.Enrollments.Any())
            return BadRequest($"Cannot delete student with {student.Enrollments.Count} active enrollments");

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/students/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkInsert(List<StudentCreateDto> studentDtos)
    {
        if (studentDtos == null || !studentDtos.Any())
            return BadRequest("Student list is empty");

        var students = studentDtos.Select(dto => new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Age = dto.Age,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth.UtcDateTime,
            UserId = 0 // Placeholder
        }).ToList();

        _context.Students.AddRange(students);
        await _context.SaveChangesAsync();

        return Ok(new { Count = students.Count });
    }

    // GET: api/students/search-by-name
    [HttpGet("search-by-name")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        if (string.IsNullOrEmpty(name))
            return BadRequest("Name parameter is required");

        var students = await _context.Students
            .Where(s => s.FirstName.Contains(name) || s.LastName.Contains(name))
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Age = s.Age,
                Email = s.Email,
                Phone = s.Phone,
                UserId = s.UserId,
                DateOfBirth = new DateTimeOffset(s.DateOfBirth, TimeSpan.Zero)
            })
            .ToListAsync();

        return Ok(students);
    }

    // GET: api/students/count
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount() =>
        await _context.Students.CountAsync();

    // GET: api/students/with-courses
    [HttpGet("with-courses")]
    public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetWithCourses()
    {
        var students = await _context.Students
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
                UserId = s.UserId,
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

        return Ok(students);
    }

    // GET: api/students/full-details
    [HttpGet("full-details")]
    public async Task<ActionResult<IEnumerable<object>>> GetFullDetails()
    {
        var students = await _context.Students
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
                s.UserId,
                DateOfBirth = s.DateOfBirth,
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

        return Ok(students);
    }
}