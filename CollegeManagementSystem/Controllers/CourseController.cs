// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using CollegeManagementSystem.Data;
// using CollegeManagementSystem.Data.Entities;
// using CollegeManagementSystem.Models.DTO;
// using CollegeManagementSystem.Models.DTOs;
//
// namespace CollegeManagementSystem.Controllers;
//
// [Route("api/[controller]")]
// [ApiController]
// public class CoursesController : ControllerBase
// {
//     private readonly AppDbContext _context;
//
//     public CoursesController(AppDbContext context)
//     {
//         _context = context;
//     }
//
//     // GET: api/courses
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<CourseListDto>>> GetAll()
//     {
//         var courses = await _context.Courses
//             .Select(c => new CourseListDto
//             {
//                 Id = c.Id,
//                 Name = c.Name,
//                 DurationYears = c.DurationYears,
//                 ModuleCount = c.Modules.Count,
//                 EnrollmentCount = c.Enrollments.Count
//             })
//             .ToListAsync();
//
//         return Ok(courses);
//     }
//
//     // GET: api/courses/{id}
//     [HttpGet("{id:int}")]
//     public async Task<ActionResult<CourseResponseDto>> GetById(int id)
//     {
//         var course = await _context.Courses
//             .Include(c => c.Modules)
//             .Include(c => c.Enrollments)
//             .ThenInclude(e => e.Student)
//             .FirstOrDefaultAsync(c => c.Id == id);
//
//         if (course == null)
//             return NotFound($"Course with ID {id} not found");
//
//         var response = new CourseResponseDto
//         {
//             Id = course.Id,
//             Name = course.Name,
//             DurationYears = course.DurationYears,
//             ModuleCount = course.Modules.Count,
//             EnrollmentCount = course.Enrollments.Count,
//             Modules = course.Modules.Select(m => new ModuleResponseDto
//             {
//                 Id = m.Id,
//                 Title = m.Title,
//                 Credits = m.Credits,
//                 CourseId = m.CourseId,
//                 CourseName = course.Name
//             }).ToList(),
//             Enrollments = course.Enrollments.Select(e => new EnrollmentResponseDto
//             {
//                 EnrollmentId = e.EnrollmentId,
//                 StudentId = e.StudentId,
//                 StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
//                 CourseId = e.CourseId,
//                 CourseName = course.Name
//             }).ToList()
//         };
//
//         return Ok(response);
//     }
//
//     // GET: api/courses/{id}/modules
//     [HttpGet("{id:int}/modules")]
//     public async Task<IActionResult> GetModules(int id)
//     {
//         var courseExists = await _context.Courses.AnyAsync(c => c.Id == id);
//         if (!courseExists)
//             return NotFound($"Course with ID {id} not found");
//
//         var modules = await _context.Modules
//             .Where(m => m.CourseId == id)
//             .Select(m => new ModuleResponseDto
//             {
//                 Id = m.Id,
//                 Title = m.Title,
//                 Credits = m.Credits,
//                 CourseId = m.CourseId,
//                 CourseName = m.Course.Name
//             })
//             .ToListAsync();
//
//         return Ok(modules);
//     }
//
//     // GET: api/courses/{id}/students
//     [HttpGet("{id:int}/students")]
//     public async Task<IActionResult> GetStudents(int id)
//     {
//         var courseExists = await _context.Courses.AnyAsync(c => c.Id == id);
//         if (!courseExists)
//             return NotFound($"Course with ID {id} not found");
//
//         var students = await _context.Enrollments
//             .Where(e => e.CourseId == id)
//             .Include(e => e.Student)
//             .Select(e => new StudentResponseDto
//             {
//                 Id = e.Student.Id,
//                 FirstName = e.Student.FirstName,
//                 LastName = e.Student.LastName,
//                 Age = e.Student.Age,
//                 Email = e.Student.Email,
//                 Phone = e.Student.Phone,
//                 DateOfBirth = e.Student.DateOfBirth
//             })
//             .ToListAsync();
//
//         return Ok(students);
//     }
//
//     // POST: api/courses
//     [HttpPost]
//     public async Task<ActionResult<CourseResponseDto>> Create(CourseCreateDto courseDto)
//     {
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
//
//         var existingCourse = await _context.Courses
//             .AnyAsync(c => c.Name == courseDto.Name);
//
//         if (existingCourse)
//             return Conflict($"Course with name '{courseDto.Name}' already exists");
//
//         var course = new Course
//         {
//             Name = courseDto.Name,
//             DurationYears = courseDto.DurationYears,
//             Modules = new List<Module>(),
//             Enrollments = new List<Enrollment>()
//         };
//
//         _context.Courses.Add(course);
//         await _context.SaveChangesAsync();
//
//         var response = new CourseResponseDto
//         {
//             Id = course.Id,
//             Name = course.Name,
//             DurationYears = course.DurationYears,
//             ModuleCount = 0,
//             EnrollmentCount = 0
//         };
//
//         return CreatedAtAction(nameof(GetById), new { id = course.Id }, response);
//     }
//
//     // POST: api/courses/{id}/modules
//     [HttpPost("{id:int}/modules")]
//     public async Task<IActionResult> AddModule(int id, ModuleCreateDto moduleDto)
//     {
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
//
//         var course = await _context.Courses.FindAsync(id);
//         if (course == null)
//             return NotFound($"Course with ID {id} not found");
//
//         var existingModule = await _context.Modules
//             .AnyAsync(m => m.CourseId == id && m.Title == moduleDto.Title);
//
//         if (existingModule)
//             return Conflict($"Module '{moduleDto.Title}' already exists in this course");
//
//         var module = new Module
//         {
//             Title = moduleDto.Title,
//             Credits = moduleDto.Credits,
//             CourseId = id,
//             Course = course
//         };
//
//         _context.Modules.Add(module);
//         await _context.SaveChangesAsync();
//
//         var response = new ModuleResponseDto
//         {
//             Id = module.Id,
//             Title = module.Title,
//             Credits = module.Credits,
//             CourseId = module.CourseId,
//             CourseName = course.Name
//         };
//
//         return CreatedAtAction(nameof(GetModules), new { id = id }, response);
//     }
//
//     // PUT: api/courses/{id}
//     [HttpPut("{id:int}")]
//     public async Task<IActionResult> Update(int id, CourseUpdateDto courseDto)
//     {
//         if (id != courseDto.Id)
//             return BadRequest("ID mismatch");
//
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
//
//         var existingCourse = await _context.Courses.FindAsync(id);
//         if (existingCourse == null)
//             return NotFound($"Course with ID {id} not found");
//
//         if (existingCourse.Name != courseDto.Name)
//         {
//             var duplicateName = await _context.Courses
//                 .AnyAsync(c => c.Name == courseDto.Name && c.Id != id);
//
//             if (duplicateName)
//                 return Conflict($"Course name '{courseDto.Name}' is already in use");
//         }
//
//         existingCourse.Name = courseDto.Name;
//         existingCourse.DurationYears = courseDto.DurationYears;
//
//         await _context.SaveChangesAsync();
//         return NoContent();
//     }
//
//     // DELETE: api/courses/{id}
//     [HttpDelete("{id:int}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         var course = await _context.Courses
//             .Include(c => c.Enrollments)
//             .Include(c => c.Modules)
//             .FirstOrDefaultAsync(c => c.Id == id);
//
//         if (course == null)
//             return NotFound($"Course with ID {id} not found");
//
//         if (course.Enrollments.Any())
//             return BadRequest($"Cannot delete course '{course.Name}' because it has {course.Enrollments.Count} enrolled students");
//
//         if (course.Modules.Any())
//             return BadRequest($"Cannot delete course '{course.Name}' because it has {course.Modules.Count} modules");
//
//         _context.Courses.Remove(course);
//         await _context.SaveChangesAsync();
//
//         return NoContent();
//     }
//
//     // POST: api/courses/bulk
//     [HttpPost("bulk")]
//     public async Task<IActionResult> BulkInsert(List<CourseCreateDto> courseDtos)
//     {
//         if (courseDtos == null || !courseDtos.Any())
//             return BadRequest("Course list is empty");
//
//         var courses = courseDtos.Select(dto => new Course
//         {
//             Name = dto.Name,
//             DurationYears = dto.DurationYears
//         }).ToList();
//
//         _context.Courses.AddRange(courses);
//         await _context.SaveChangesAsync();
//
//         return Ok(new { Count = courses.Count });
//     }
//
//     // GET: api/courses/with-details
//     [HttpGet("with-details")]
//     public async Task<IActionResult> GetWithDetails()
//     {
//         var courses = await _context.Courses
//             .Include(c => c.Modules)
//             .ThenInclude(m => m.Course)
//             .Include(c => c.Enrollments)
//             .ThenInclude(e => e.Student)
//             .Select(c => new
//             {
//                 c.Id,
//                 c.Name,
//                 c.DurationYears,
//                 Modules = c.Modules.Select(m => new
//                 {
//                     m.Id,
//                     m.Title,
//                     m.Credits,
//                     Instructors = _context.ModuleInstructors
//                         .Where(mi => mi.ModuleId == m.Id)
//                         .Select(mi => new
//                         {
//                             mi.Instructor.Id,
//                             mi.Instructor.FirstName,
//                             mi.Instructor.LastName,
//                             mi.Instructor.Email
//                         }).ToList()
//                 }).ToList(),
//                 Students = c.Enrollments.Select(e => new
//                 {
//                     e.Student.Id,
//                     e.Student.FirstName,
//                     e.Student.LastName,
//                     e.Student.Email
//                 }).ToList()
//             })
//             .ToListAsync();
//
//         return Ok(courses);
//     }
//
//     // GET: api/courses/count
//     [HttpGet("count")]
//     public async Task<ActionResult<int>> GetCount() =>
//         await _context.Courses.CountAsync();
//
//     // GET: api/courses/total-credits
//     [HttpGet("total-credits")]
//     public async Task<IActionResult> GetTotalCredits()
//     {
//         var total = await _context.Modules.SumAsync(m => m.Credits);
//         return Ok(new { TotalCredits = total });
//     }
//
//     // GET: api/courses/top-enrolled
//     [HttpGet("top-enrolled")]
//     public async Task<IActionResult> GetTopEnrolled([FromQuery] int top = 5)
//     {
//         var result = await _context.Courses
//             .Select(c => new
//             {
//                 c.Id,
//                 c.Name,
//                 c.DurationYears,
//                 EnrollmentCount = c.Enrollments.Count,
//                 ModuleCount = c.Modules.Count
//             })
//             .OrderByDescending(c => c.EnrollmentCount)
//             .Take(top)
//             .ToListAsync();
//
//         return Ok(result);
//     }
//
//     // GET: api/courses/search
//     [HttpGet("search")]
//     public async Task<IActionResult> Search([FromQuery] string name)
//     {
//         if (string.IsNullOrWhiteSpace(name))
//             return BadRequest("Search term cannot be empty");
//
//         var courses = await _context.Courses
//             .Where(c => c.Name.Contains(name))
//             .Select(c => new CourseListDto
//             {
//                 Id = c.Id,
//                 Name = c.Name,
//                 DurationYears = c.DurationYears,
//                 ModuleCount = c.Modules.Count,
//                 EnrollmentCount = c.Enrollments.Count
//             })
//             .ToListAsync();
//
//         return Ok(courses);
//     }
//
//     // GET: api/courses/duration/{years}
//     [HttpGet("duration/{years:int}")]
//     public async Task<IActionResult> GetByDuration(int years)
//     {
//         var courses = await _context.Courses
//             .Where(c => c.DurationYears == years)
//             .Select(c => new CourseListDto
//             {
//                 Id = c.Id,
//                 Name = c.Name,
//                 DurationYears = c.DurationYears,
//                 ModuleCount = c.Modules.Count,
//                 EnrollmentCount = c.Enrollments.Count
//             })
//             .ToListAsync();
//
//         return Ok(courses);
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(AppDbContext context, IMemoryCache cache, ILogger<CoursesController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // TASK 2: GET: api/courses - WITH CACHING (5 minutes)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseListDto>>> GetAll()
    {
        const string cacheKey = "all_courses";
        
        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out List<CourseListDto> cachedCourses))
        {
            _logger.LogInformation($" CACHE HIT: Returning {cachedCourses.Count} courses from cache");
            return Ok(new 
            { 
                source = "Cache (5 min TTL)",
                    timestamp = DateTime.Now,
                    data = cachedCourses 
                });
        }
        
        _logger.LogInformation("CACHE MISS: Fetching courses from database");
        
        // Fetch from database
        var courses = await _context.Courses
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Name = c.Name,
                DurationYears = c.DurationYears,
                ModuleCount = c.Modules.Count,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync();
        
        // Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetPriority(CacheItemPriority.Normal);
        
        _cache.Set(cacheKey, courses, cacheOptions);
        
        return Ok(new 
        { 
            source = "Database",
            timestamp = DateTime.Now,
            data = courses 
        });
    }

    // ========== GET: api/courses/{id} - WITH INDIVIDUAL CACHING ==========
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseResponseDto>> GetById(int id)
    {
        string cacheKey = $"course_{id}";
        
        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out CourseResponseDto cachedCourse))
        {
            _logger.LogInformation($"CACHE HIT: Course {id} found in cache");
            return Ok(new { source = "Cache", data = cachedCourse });
        }
        
        _logger.LogInformation($"CACHE MISS: Fetching course {id} from database");
        
        var course = await _context.Courses
            .Include(c => c.Modules)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound($"Course with ID {id} not found");

        var response = new CourseResponseDto
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
        
        // Cache individual course for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));
        
        _cache.Set(cacheKey, response, cacheOptions);

        return Ok(new { source = "Database", data = response });
    }

    // GET: api/courses/{id}/modules
    [HttpGet("{id:int}/modules")]
    public async Task<IActionResult> GetModules(int id)
    {
        string cacheKey = $"course_{id}_modules";
        
        if (_cache.TryGetValue(cacheKey, out List<ModuleResponseDto> cachedModules))
        {
            _logger.LogInformation($"CACHE HIT: Modules for course {id}");
            return Ok(new { source = "Cache", data = cachedModules });
        }
        
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == id);
        if (!courseExists)
            return NotFound($"Course with ID {id} not found");

        var modules = await _context.Modules
            .Where(m => m.CourseId == id)
            .Select(m => new ModuleResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Credits = m.Credits,
                CourseId = m.CourseId,
                CourseName = m.Course.Name
            })
            .ToListAsync();
        
        // Cache modules for 5 minutes
        _cache.Set(cacheKey, modules, TimeSpan.FromMinutes(5));

        return Ok(new { source = "Database", data = modules });
    }

    // GET: api/courses/{id}/students
    [HttpGet("{id:int}/students")]
    public async Task<IActionResult> GetStudents(int id)
    {
        string cacheKey = $"course_{id}_students";
        
        if (_cache.TryGetValue(cacheKey, out List<StudentResponseDto> cachedStudents))
        {
            _logger.LogInformation($"CACHE HIT: Students for course {id}");
            return Ok(new { source = "Cache", data = cachedStudents });
        }
        
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == id);
        if (!courseExists)
            return NotFound($"Course with ID {id} not found");

        var students = await _context.Enrollments
            .Where(e => e.CourseId == id)
            .Include(e => e.Student)
            .Select(e => new StudentResponseDto
            {
                Id = e.Student.Id,
                FirstName = e.Student.FirstName,
                LastName = e.Student.LastName,
                Age = e.Student.Age,
                Email = e.Student.Email,
                Phone = e.Student.Phone,
                DateOfBirth = e.Student.DateOfBirth
            })
            .ToListAsync();
        
        _cache.Set(cacheKey, students, TimeSpan.FromMinutes(5));

        return Ok(new { source = "Database", data = students });
    }

    // ========== POST: api/courses - CLEARS CACHE ON CREATE ==========
    [HttpPost]
    public async Task<ActionResult<CourseResponseDto>> Create(CourseCreateDto courseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingCourse = await _context.Courses
            .AnyAsync(c => c.Name == courseDto.Name);

        if (existingCourse)
            return Conflict($"Course with name '{courseDto.Name}' already exists");

        var course = new Course
        {
            Name = courseDto.Name,
            DurationYears = courseDto.DurationYears,
            Modules = new List<Module>(),
            Enrollments = new List<Enrollment>()
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        
        // Clear cache because data has changed
        ClearCourseCache();
        _logger.LogInformation("Cache cleared after creating new course");

        var response = new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            DurationYears = course.DurationYears,
            ModuleCount = 0,
            EnrollmentCount = 0
        };

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, response);
    }

    // POST: api/courses/{id}/modules - CLEARS CACHE ON ADD MODULE
    [HttpPost("{id:int}/modules")]
    public async Task<IActionResult> AddModule(int id, ModuleCreateDto moduleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound($"Course with ID {id} not found");

        var existingModule = await _context.Modules
            .AnyAsync(m => m.CourseId == id && m.Title == moduleDto.Title);

        if (existingModule)
            return Conflict($"Module '{moduleDto.Title}' already exists in this course");

        var module = new Module
        {
            Title = moduleDto.Title,
            Credits = moduleDto.Credits,
            CourseId = id,
            Course = course
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
        
        // Clear course-related cache
        ClearCourseCache(id);
        _logger.LogInformation($"🗑️ Cache cleared for course {id} after adding module");

        var response = new ModuleResponseDto
        {
            Id = module.Id,
            Title = module.Title,
            Credits = module.Credits,
            CourseId = module.CourseId,
            CourseName = course.Name
        };

        return CreatedAtAction(nameof(GetModules), new { id = id }, response);
    }

    // PUT: api/courses/{id} - CLEARS CACHE ON UPDATE
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CourseUpdateDto courseDto)
    {
        if (id != courseDto.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingCourse = await _context.Courses.FindAsync(id);
        if (existingCourse == null)
            return NotFound($"Course with ID {id} not found");

        if (existingCourse.Name != courseDto.Name)
        {
            var duplicateName = await _context.Courses
                .AnyAsync(c => c.Name == courseDto.Name && c.Id != id);

            if (duplicateName)
                return Conflict($"Course name '{courseDto.Name}' is already in use");
        }

        existingCourse.Name = courseDto.Name;
        existingCourse.DurationYears = courseDto.DurationYears;

        await _context.SaveChangesAsync();
        
        // Clear cache for this specific course and all courses list
        ClearCourseCache(id);
        _logger.LogInformation($"🗑Cache cleared for course {id} after update");
        
        return NoContent();
    }

    // DELETE: api/courses/{id} - CLEARS CACHE ON DELETE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Enrollments)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound($"Course with ID {id} not found");

        if (course.Enrollments.Any())
            return BadRequest($"Cannot delete course '{course.Name}' because it has {course.Enrollments.Count} enrolled students");

        if (course.Modules.Any())
            return BadRequest($"Cannot delete course '{course.Name}' because it has {course.Modules.Count} modules");

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        
        // Clear all course-related cache
        ClearCourseCache(id);
        _logger.LogInformation($" Cache cleared after deleting course {id}");

        return NoContent();
    }

    // DELETE: api/courses/cache - CLEAR ALL CACHE (Utility endpoint)
    [HttpDelete("cache")]
    public IActionResult ClearAllCache()
    {
        // Note: IMemoryCache doesn't have a built-in Clear all method
        // This is a workaround - consider using IDistributedCache for production
        
        _cache.Remove("all_courses");
        _logger.LogInformation("All course cache cleared");
        
        return Ok(new { message = "Cache cleared successfully" });
    }

    // POST: api/courses/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkInsert(List<CourseCreateDto> courseDtos)
    {
        if (courseDtos == null || !courseDtos.Any())
            return BadRequest("Course list is empty");

        var courses = courseDtos.Select(dto => new Course
        {
            Name = dto.Name,
            DurationYears = dto.DurationYears
        }).ToList();

        _context.Courses.AddRange(courses);
        await _context.SaveChangesAsync();
        
        // Clear cache after bulk insert
        ClearCourseCache();

        return Ok(new { Count = courses.Count });
    }

    // GET: api/courses/with-details
    [HttpGet("with-details")]
    public async Task<IActionResult> GetWithDetails()
    {
        const string cacheKey = "courses_with_details";
        
        if (_cache.TryGetValue(cacheKey, out object cachedDetails))
        {
            return Ok(new { source = "Cache", data = cachedDetails });
        }
        
        var courses = await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Course)
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
                    Instructors = _context.ModuleInstructors
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
        
        _cache.Set(cacheKey, courses, TimeSpan.FromMinutes(5));

        return Ok(new { source = "Database", data = courses });
    }

    // GET: api/courses/count
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount() =>
        await _context.Courses.CountAsync();

    // GET: api/courses/total-credits
    [HttpGet("total-credits")]
    public async Task<IActionResult> GetTotalCredits()
    {
        var total = await _context.Modules.SumAsync(m => m.Credits);
        return Ok(new { TotalCredits = total });
    }

    // GET: api/courses/top-enrolled
    [HttpGet("top-enrolled")]
    public async Task<IActionResult> GetTopEnrolled([FromQuery] int top = 5)
    {
        var result = await _context.Courses
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

        return Ok(result);
    }

    // GET: api/courses/search
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search term cannot be empty");

        var courses = await _context.Courses
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

        return Ok(courses);
    }

    // GET: api/courses/duration/{years}
    [HttpGet("duration/{years:int}")]
    public async Task<IActionResult> GetByDuration(int years)
    {
        var courses = await _context.Courses
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

        return Ok(courses);
    }

    // ========== HELPER METHODS FOR CACHE MANAGEMENT ==========
    
    private void ClearCourseCache(int? courseId = null)
    {
        // Clear main courses list cache
        _cache.Remove("all_courses");
        _cache.Remove("courses_with_details");
        
        // Clear specific course cache if provided
        if (courseId.HasValue)
        {
            _cache.Remove($"course_{courseId}");
            _cache.Remove($"course_{courseId}_modules");
            _cache.Remove($"course_{courseId}_students");
        }
    }
}