using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Interfaces
{
    public interface IModuleInstructorService
    {
        Task<IEnumerable<ModuleInstructorResponseDto>> GetAllAssignmentsAsync();
        Task<ModuleInstructorResponseDto?> GetAssignmentByIdAsync(int id);
        Task<IEnumerable<ModuleInstructorResponseDto>> GetAssignmentsByModuleAsync(int moduleId);
        Task<IEnumerable<ModuleInstructorResponseDto>> GetAssignmentsByInstructorAsync(int instructorId);
        Task<IEnumerable<ModuleInstructorResponseDto>> GetAssignmentsByCourseAsync(int courseId);
        Task<ModuleInstructorResponseDto> CreateAssignmentAsync(ModuleInstructorCreateDto assignmentDto);
        Task<bool> DeleteAssignmentAsync(int id);
    }
}