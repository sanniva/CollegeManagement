// Models/DTOs/DashboardDto.cs
namespace CollegeManagementSystem.Models.DTO;

public class DashboardStatisticsDto
{
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalModules { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalEnrollments { get; set; }
}

public class CourseEnrollmentStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int EnrollmentCount { get; set; }
    public int TotalCredits { get; set; }
}

public class ModuleInstructorStatsDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int ModulesTaught { get; set; }
}

public class AgeDistributionDto
{
    public int Age { get; set; }
    public int Count { get; set; }
}