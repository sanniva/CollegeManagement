using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InstructorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InstructorsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/instructors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstructorResponseDto>>> GetInstructors()
    {
        var instructors = await _context.Instructors
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                HireDate = i.HireDate,
                ModulesTaught = _context.ModuleInstructors.Count(mi => mi.InstructorId == i.Id)
            })
            .ToListAsync();

        return Ok(instructors);
    }

    // GET: api/instructors/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InstructorResponseDto>> GetInstructor(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);

        if (instructor == null)
            return NotFound($"Instructor with ID {id} not found");

        var response = new InstructorResponseDto
        {
            Id = instructor.Id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            Email = instructor.Email,
            HireDate = instructor.HireDate,
            ModulesTaught = await _context.ModuleInstructors.CountAsync(mi => mi.InstructorId == id)
        };

        return Ok(response);
    }

    // GET: api/instructors/{id}/modules
    [HttpGet("{id:int}/modules")]
    public async Task<IActionResult> GetInstructorModules(int id)
    {
        var instructorExists = await _context.Instructors.AnyAsync(i => i.Id == id);
        if (!instructorExists)
            return NotFound($"Instructor with ID {id} not found");

        var modules = await _context.ModuleInstructors
            .Where(mi => mi.InstructorId == id)
            .Include(mi => mi.Module)
            .ThenInclude(m => m.Course)
            .Select(mi => new ModuleResponseDto
            {
                Id = mi.Module.Id,
                Title = mi.Module.Title,
                Credits = mi.Module.Credits,
                CourseId = mi.Module.CourseId,
                CourseName = mi.Module.Course.Name
            })
            .ToListAsync();

        return Ok(modules);
    }

    // GET: api/instructors/email/{email}
    [HttpGet("email/{email}")]
    public async Task<ActionResult<InstructorResponseDto>> GetInstructorByEmail(string email)
    {
        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.Email == email);

        if (instructor == null)
            return NotFound($"Instructor with email {email} not found");

        var response = new InstructorResponseDto
        {
            Id = instructor.Id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            Email = instructor.Email,
            HireDate = instructor.HireDate,
            ModulesTaught = await _context.ModuleInstructors.CountAsync(mi => mi.InstructorId == instructor.Id)
        };

        return Ok(response);
    }

    // GET: api/instructors/search
    [HttpGet("search")]
    public async Task<IActionResult> SearchInstructors([FromQuery] string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return BadRequest("Last name search term cannot be empty");

        var instructors = await _context.Instructors
            .Where(i => i.LastName.Contains(lastName))
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                HireDate = i.HireDate,
                ModulesTaught = _context.ModuleInstructors.Count(mi => mi.InstructorId == i.Id)
            })
            .ToListAsync();

        return Ok(instructors);
    }

    // GET: api/instructors/hired-after
    [HttpGet("hired-after")]
    public async Task<IActionResult> GetInstructorsHiredAfter([FromQuery] DateTime date)
    {
        var instructors = await _context.Instructors
            .Where(i => i.HireDate >= date)
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                HireDate = i.HireDate,
                ModulesTaught = _context.ModuleInstructors.Count(mi => mi.InstructorId == i.Id)
            })
            .ToListAsync();

        return Ok(instructors);
    }

    // POST: api/instructors
    [HttpPost]
    public async Task<ActionResult<InstructorResponseDto>> Create(InstructorCreateDto instructorDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingInstructor = await _context.Instructors
            .AnyAsync(i => i.Email == instructorDto.Email);

        if (existingInstructor)
            return Conflict($"Instructor with email {instructorDto.Email} already exists");

        var instructor = new Instructor
        {
            FirstName = instructorDto.FirstName,
            LastName = instructorDto.LastName,
            Email = instructorDto.Email,
            HireDate = instructorDto.HireDate
        };

        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();

        var response = new InstructorResponseDto
        {
            Id = instructor.Id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            Email = instructor.Email,
            HireDate = instructor.HireDate,
            ModulesTaught = 0
        };

        return CreatedAtAction(nameof(GetInstructor), new { id = instructor.Id }, response);
    }

    // PUT: api/instructors/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, InstructorUpdateDto instructorDto)
    {
        if (id != instructorDto.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingInstructor = await _context.Instructors.FindAsync(id);
        if (existingInstructor == null)
            return NotFound($"Instructor with ID {id} not found");

        if (existingInstructor.Email != instructorDto.Email)
        {
            var duplicateEmail = await _context.Instructors
                .AnyAsync(i => i.Email == instructorDto.Email && i.Id != id);

            if (duplicateEmail)
                return Conflict($"Email {instructorDto.Email} is already in use");
        }

        existingInstructor.FirstName = instructorDto.FirstName;
        existingInstructor.LastName = instructorDto.LastName;
        existingInstructor.Email = instructorDto.Email;
        existingInstructor.HireDate = instructorDto.HireDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/instructors/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var instructor = await _context.Instructors
            .Include(i => _context.ModuleInstructors.Where(mi => mi.InstructorId == id))
            .FirstOrDefaultAsync(i => i.Id == id);

        if (instructor == null)
            return NotFound($"Instructor with ID {id} not found");

        var moduleAssignments = await _context.ModuleInstructors
            .CountAsync(mi => mi.InstructorId == id);

        if (moduleAssignments > 0)
            return BadRequest($"Cannot delete instructor with {moduleAssignments} module assignments");

        _context.Instructors.Remove(instructor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}