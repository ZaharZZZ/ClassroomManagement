// Services/Interfaces/IAuditService.cs
namespace ClassroomManagement.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string action, string? details = null, string? ipAddress = null);
        Task<List<Models.Entities.AuditLog>> GetLogsAsync(int page = 1, int pageSize = 50);
    }
}