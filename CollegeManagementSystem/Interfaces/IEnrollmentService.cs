using CollegeManagementSystem.Models.DTO;
using CollegeManagementSystem.Models.DTOs;

namespace CollegeManagementSystem.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentResponseDto>> GetAllEnrollmentsAsync();
        Task<EnrollmentResponseDto?> GetEnrollmentByIdAsync(int id);
        Task<IEnumerable<EnrollmentResponseDto>> GetEnrollmentsByStudentAsync(int studentId);
        Task<IEnumerable<EnrollmentResponseDto>> GetEnrollmentsByCourseAsync(int courseId);
        Task<EnrollmentResponseDto> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentDto);
        Task<bool> DeleteEnrollmentAsync(int id);
        Task<object> GetEnrollmentStatisticsAsync();
    }
}