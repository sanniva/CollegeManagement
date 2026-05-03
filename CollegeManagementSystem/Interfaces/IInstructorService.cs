using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Interfaces
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorResponseDto>> GetAllInstructorsAsync();
        Task<InstructorResponseDto?> GetInstructorByIdAsync(int id);
        Task<IEnumerable<ModuleResponseDto>> GetInstructorModulesAsync(int instructorId);
        Task<InstructorResponseDto?> GetInstructorByEmailAsync(string email);
        Task<IEnumerable<InstructorResponseDto>> SearchInstructorsAsync(string lastName);
        Task<IEnumerable<InstructorResponseDto>> GetInstructorsHiredAfterAsync(DateTime date);
        Task<InstructorResponseDto> CreateInstructorAsync(InstructorCreateDto instructorDto);
        Task<bool> UpdateInstructorAsync(int id, InstructorUpdateDto instructorDto);
        Task<bool> DeleteInstructorAsync(int id);
    }
}