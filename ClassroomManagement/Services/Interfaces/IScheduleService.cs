using ClassroomManagement.Models.ViewModels;

namespace ClassroomManagement.Services.Interfaces
{
    public interface IScheduleService
    {
        /// <summary>
        /// Получение данных для главной страницы расписания с возможностью фильтрации
        /// </summary>
        Task<HomeViewModel> GetScheduleViewModelAsync(
            DateTime date,
            int pairNumber,
            int? buildingId = null,
            int? floor = null,
            int? departmentId = null);

        /// <summary>
        /// Получение списка пар из настроек системы
        /// </summary>
        Task<List<PairInfo>> GetPairsAsync();

        /// <summary>
        /// Проверка занятости кабинета на конкретную пару и дату
        /// </summary>
        Task<CellStatus> GetCellStatusAsync(
            int classroomId, DateOnly date, TimeOnly startTime, TimeOnly endTime);

        /// <summary>
        /// Получение информации для всплывающей подсказки
        /// </summary>
        Task<ClassroomCellViewModel?> GetCellInfoAsync(
            int classroomId, DateOnly date, int pairNumber);
    }
}