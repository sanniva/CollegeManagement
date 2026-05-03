using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseListDto>> GetAllCoursesAsync();
        Task<CourseResponseDto?> GetCourseByIdAsync(int id);
        Task<IEnumerable<ModuleResponseDto>> GetCourseModulesAsync(int courseId);
        Task<IEnumerable<StudentResponseDto>> GetCourseStudentsAsync(int courseId);
        Task<CourseResponseDto> CreateCourseAsync(CourseCreateDto courseDto);
        Task<ModuleResponseDto> AddModuleToCourseAsync(int courseId, ModuleCreateDto moduleDto);
        Task<bool> UpdateCourseAsync(int id, CourseUpdateDto courseDto);
        Task<bool> DeleteCourseAsync(int id);
        Task<int> BulkInsertCoursesAsync(List<CourseCreateDto> courseDtos);
        Task<IEnumerable<object>> GetCoursesWithDetailsAsync();
        Task<int> GetCoursesCountAsync();
        Task<int> GetTotalCreditsAsync();
        Task<IEnumerable<object>> GetTopEnrolledCoursesAsync(int top);
        Task<IEnumerable<CourseListDto>> SearchCoursesAsync(string name);
        Task<IEnumerable<CourseListDto>> GetCoursesByDurationAsync(int years);
    }
}