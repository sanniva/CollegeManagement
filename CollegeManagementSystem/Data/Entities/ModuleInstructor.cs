using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CollegeManagementSystem.Data.Entities;

namespace CollegeManagementSystem.Data.Entities;

public class ModuleInstructor
{
    public int ModuleInstructorId { get; set; }

    public int ModuleId { get; set; }
    [ForeignKey("ModuleId")]
    public Module Module { get; set; }

    public int InstructorId { get; set; }
    [ForeignKey("InstructorId")]
    public Instructor Instructor { get; set; }
}