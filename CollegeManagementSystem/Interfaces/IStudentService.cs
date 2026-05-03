using CollegeManagementSystem.Models.DTO;

namespace CollegeManagementSystem.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync();
        Task<StudentResponseDto?> GetStudentByIdAsync(int id);
        Task<IEnumerable<CourseListDto>> GetStudentCoursesAsync(int studentId);
        Task<StudentResponseDto> CreateStudentAsync(StudentCreateDto studentDto);
        Task<bool> UpdateStudentAsync(int id, StudentUpdateDto studentDto);
        Task<bool> DeleteStudentAsync(int id);
        Task<int> BulkInsertStudentsAsync(List<StudentCreateDto> studentDtos);
        Task<IEnumerable<StudentResponseDto>> GetStudentsWithCoursesAsync();
        Task<int> GetStudentsCountAsync();
        Task<IEnumerable<object>> GetFullStudentDetailsAsync();
    }
}