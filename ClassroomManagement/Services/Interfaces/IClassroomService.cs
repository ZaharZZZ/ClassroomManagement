// Services/Interfaces/IClassroomService.cs
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;

namespace ClassroomManagement.Services.Interfaces
{
    public interface IClassroomService
    {
        Task<List<Classroom>> GetAllClassroomsAsync();
        Task<Classroom?> GetClassroomByIdAsync(int id);
        Task<(bool Success, string Message)> CreateClassroomAsync(ClassroomFormViewModel model);
        Task<(bool Success, string Message)> UpdateClassroomAsync(ClassroomFormViewModel model);
        Task<(bool Success, string Message)> DeleteClassroomAsync(int id);
        Task<ClassroomFormViewModel> GetClassroomFormViewModelAsync(int? id = null);
    }
}