using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Interfaces
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleResponseDto>> GetAllModulesAsync();
        Task<ModuleResponseDto?> GetModuleByIdAsync(int id);
        Task<IEnumerable<ModuleResponseDto>> GetModulesByCourseAsync(int courseId);
        Task<IEnumerable<ModuleResponseDto>> GetModulesByCreditsAsync(int credits);
        Task<IEnumerable<InstructorResponseDto>> GetModuleInstructorsAsync(int moduleId);
        Task<ModuleResponseDto> CreateModuleAsync(ModuleCreateDto moduleDto);
        Task<bool> UpdateModuleAsync(int id, ModuleUpdateDto moduleDto);
        Task<bool> DeleteModuleAsync(int id);
    }
}