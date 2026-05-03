using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeManagementSystem.Data;
using CollegeManagementSystem.Data.Entities;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModuleInstructorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ModuleInstructorsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/moduleinstructors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleInstructorResponseDto>>> GetModuleInstructors()
    {
        var moduleInstructors = await _context.ModuleInstructors
            .Include(mi => mi.Module)
            .Include(mi => mi.Instructor)
            .Select(mi => new ModuleInstructorResponseDto
            {
                ModuleInstructorId = mi.ModuleInstructorId,
                ModuleId = mi.ModuleId,
                ModuleTitle = mi.Module.Title,
                InstructorId = mi.InstructorId,
                InstructorName = $"{mi.Instructor.FirstName} {mi.Instructor.LastName}"
            })
            .ToListAsync();

        return Ok(moduleInstructors);
    }

    // GET: api/moduleinstructors/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ModuleInstructorResponseDto>> GetModuleInstructor(int id)
    {
        var moduleInstructor = await _context.ModuleInstructors
            .Include(mi => mi.Module)
            .Include(mi => mi.Instructor)
            .FirstOrDefaultAsync(mi => mi.ModuleInstructorId == id);

        if (moduleInstructor == null)
            return NotFound($"Module instructor assignment with ID {id} not found");

        var response = new ModuleInstructorResponseDto
        {
            ModuleInstructorId = moduleInstructor.ModuleInstructorId,
            ModuleId = moduleInstructor.ModuleId,
            ModuleTitle = moduleInstructor.Module.Title,
            InstructorId = moduleInstructor.InstructorId,
            InstructorName = $"{moduleInstructor.Instructor.FirstName} {moduleInstructor.Instructor.LastName}"
        };

        return Ok(response);
    }

    // GET: api/moduleinstructors/module/{moduleId}
    [HttpGet("module/{moduleId:int}")]
    public async Task<ActionResult<IEnumerable<ModuleInstructorResponseDto>>> GetModuleInstructorsByModule(int moduleId)
    {
        var moduleExists = await _context.Modules.AnyAsync(m => m.Id == moduleId);
        if (!moduleExists)
            return NotFound($"Module with ID {moduleId} not found");

        var moduleInstructors = await _context.ModuleInstructors
            .Include(mi => mi.Module)
            .Include(mi => mi.Instructor)
            .Where(mi => mi.ModuleId == moduleId)
            .Select(mi => new ModuleInstructorResponseDto
            {
                ModuleInstructorId = mi.ModuleInstructorId,
                ModuleId = mi.ModuleId,
                ModuleTitle = mi.Module.Title,
                InstructorId = mi.InstructorId,
                InstructorName = $"{mi.Instructor.FirstName} {mi.Instructor.LastName}"
            })
            .ToListAsync();

        return Ok(moduleInstructors);
    }

    // GET: api/moduleinstructors/instructor/{instructorId}
    [HttpGet("instructor/{instructorId:int}")]
    public async Task<ActionResult<IEnumerable<ModuleInstructorResponseDto>>> GetModuleInstructorsByInstructor(int instructorId)
    {
        var instructorExists = await _context.Instructors.AnyAsync(i => i.Id == instructorId);
        if (!instructorExists)
            return NotFound($"Instructor with ID {instructorId} not found");

        var moduleInstructors = await _context.ModuleInstructors
            .Include(mi => mi.Module)
            .Include(mi => mi.Instructor)
            .Where(mi => mi.InstructorId == instructorId)
            .Select(mi => new ModuleInstructorResponseDto
            {
                ModuleInstructorId = mi.ModuleInstructorId,
                ModuleId = mi.ModuleId,
                ModuleTitle = mi.Module.Title,
                InstructorId = mi.InstructorId,
                InstructorName = $"{mi.Instructor.FirstName} {mi.Instructor.LastName}"
            })
            .ToListAsync();

        return Ok(moduleInstructors);
    }

    // GET: api/moduleinstructors/course/{courseId}
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<IEnumerable<ModuleInstructorResponseDto>>> GetModuleInstructorsByCourse(int courseId)
    {
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return NotFound($"Course with ID {courseId} not found");

        var moduleInstructors = await _context.ModuleInstructors
            .Include(mi => mi.Module)
            .Include(mi => mi.Instructor)
            .Where(mi => mi.Module.CourseId == courseId)
            .Select(mi => new ModuleInstructorResponseDto
            {
                ModuleInstructorId = mi.ModuleInstructorId,
                ModuleId = mi.ModuleId,
                ModuleTitle = mi.Module.Title,
                InstructorId = mi.InstructorId,
                InstructorName = $"{mi.Instructor.FirstName} {mi.Instructor.LastName}"
            })
            .ToListAsync();

        return Ok(moduleInstructors);
    }

    // POST: api/moduleinstructors
    [HttpPost]
    public async Task<ActionResult<ModuleInstructorResponseDto>> Create(ModuleInstructorCreateDto assignmentDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var module = await _context.Modules.FindAsync(assignmentDto.ModuleId);
        if (module == null)
            return NotFound($"Module with ID {assignmentDto.ModuleId} not found");

        var instructor = await _context.Instructors.FindAsync(assignmentDto.InstructorId);
        if (instructor == null)
            return NotFound($"Instructor with ID {assignmentDto.InstructorId} not found");

        var existingAssignment = await _context.ModuleInstructors
            .AnyAsync(mi => mi.ModuleId == assignmentDto.ModuleId && mi.InstructorId == assignmentDto.InstructorId);

        if (existingAssignment)
            return Conflict("Instructor is already assigned to this module");

        var moduleInstructor = new ModuleInstructor
        {
            ModuleId = assignmentDto.ModuleId,
            InstructorId = assignmentDto.InstructorId,
            Module = module,
            Instructor = instructor
        };

        _context.ModuleInstructors.Add(moduleInstructor);
        await _context.SaveChangesAsync();

        var response = new ModuleInstructorResponseDto
        {
            ModuleInstructorId = moduleInstructor.ModuleInstructorId,
            ModuleId = moduleInstructor.ModuleId,
            ModuleTitle = module.Title,
            InstructorId = moduleInstructor.InstructorId,
            InstructorName = $"{instructor.FirstName} {instructor.LastName}"
        };

        return CreatedAtAction(nameof(GetModuleInstructor), new { id = moduleInstructor.ModuleInstructorId }, response);
    }

    // DELETE: api/moduleinstructors/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var moduleInstructor = await _context.ModuleInstructors.FindAsync(id);
        if (moduleInstructor == null)
            return NotFound($"Module instructor assignment with ID {id} not found");

        _context.ModuleInstructors.Remove(moduleInstructor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}