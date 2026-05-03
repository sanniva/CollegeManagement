using CollegeManagementSystem.Models.DTO;

namespace CollegeManagementSystem.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatisticsDto> GetStatisticsAsync();
        Task<IEnumerable<EnrollmentResponseDto>> GetRecentEnrollmentsAsync(int count);
        Task<IEnumerable<CourseEnrollmentStatsDto>> GetCourseEnrollmentStatsAsync();
        Task<IEnumerable<ModuleInstructorStatsDto>> GetModuleInstructorStatsAsync();
        Task<IEnumerable<AgeDistributionDto>> GetStudentAgeDistributionAsync();
        Task<IEnumerable<object>> GetCourseModuleCountAsync();
        Task<object> GetStudentEnrollmentHistoryAsync();
    }
}
