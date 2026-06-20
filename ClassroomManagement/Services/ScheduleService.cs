using ClassroomManagement.Data;
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;
using ClassroomManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext _context;

        public ScheduleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PairInfo>> GetPairsAsync()
        {
            var settings = await _context.SystemSettings.ToListAsync();
            var pairs = new List<PairInfo>();

            for (int i = 1; i <= 5; i++)
            {
                var start = settings.FirstOrDefault(s => s.SettingKey == $"pair_{i}_start")?.SettingValue ?? "";
                var end = settings.FirstOrDefault(s => s.SettingKey == $"pair_{i}_end")?.SettingValue ?? "";
                pairs.Add(new PairInfo { Number = i, StartTime = start, EndTime = end });
            }
            return pairs;
        }

        public async Task<HomeViewModel> GetScheduleViewModelAsync(
            DateTime date,
            int pairNumber,
            int? buildingId = null,
            int? floor = null,
            int? departmentId = null)
        {
            var pairs = await GetPairsAsync();
            var selectedPair = pairs.FirstOrDefault(p => p.Number == pairNumber) ?? pairs.First();
            var startTime = TimeOnly.Parse(selectedPair.StartTime);
            var endTime = TimeOnly.Parse(selectedPair.EndTime);
            var dateOnly = DateOnly.FromDateTime(date);

            // --- Формируем списки для фильтров ---
            var buildings = await _context.Buildings.OrderBy(b => b.Name).ToListAsync();

            // Список этажей с учётом выбранного корпуса
            var floorsQuery = _context.Classrooms.AsQueryable();
            if (buildingId.HasValue)
                floorsQuery = floorsQuery.Where(c => c.BuildingId == buildingId.Value);
            var floors = await floorsQuery.Select(c => c.Floor).Distinct().OrderBy(f => f).ToListAsync();

            // Список отделений с учётом выбранных корпуса и этажа
            var departmentsQuery = _context.Departments.AsQueryable();
            if (buildingId.HasValue)
                departmentsQuery = departmentsQuery.Where(d => d.Classrooms.Any(c => c.BuildingId == buildingId.Value));
            if (floor.HasValue)
                departmentsQuery = departmentsQuery.Where(d => d.Classrooms.Any(c => c.Floor == floor.Value));
            var departments = await departmentsQuery.OrderBy(d => d.Name).ToListAsync();

            // --- Загружаем кабинеты с фильтрацией ---
            var classroomsQuery = _context.Classrooms
                .Include(c => c.Building)
                .Include(c => c.RoomType)
                .Include(c => c.Condition)
                .Include(c => c.Department)
                .AsQueryable();

            if (buildingId.HasValue)
                classroomsQuery = classroomsQuery.Where(c => c.BuildingId == buildingId.Value);
            if (floor.HasValue)
                classroomsQuery = classroomsQuery.Where(c => c.Floor == floor.Value);
            if (departmentId.HasValue)
                classroomsQuery = classroomsQuery.Where(c => c.DepartmentId == departmentId.Value);

            var classrooms = await classroomsQuery.ToListAsync();

            // --- Загружаем расписание и заявки на данную пару ---
            var scheduleEntries = await _context.ScheduleEntries
                .Include(s => s.Teacher)
                .Where(s => s.EventDate == dateOnly &&
                            s.StartTime < endTime &&
                            s.EndTime > startTime)
                .ToListAsync();

            var pendingBookings = await _context.Bookings
                .Include(b => b.Teacher)
                .Where(b => b.EventDate == dateOnly &&
                            b.StartTime < endTime &&
                            b.EndTime > startTime &&
                            b.Status.Name == "Pending")
                .ToListAsync();

            // --- Группировка по отделениям ---
            var departmentGroups = new List<DepartmentClassroomsViewModel>();

            // Если выбран конкретный отдел – показываем только его, иначе все
            var departmentsToGroup = departmentId.HasValue
                ? departments.Where(d => d.Id == departmentId.Value).ToList()
                : departments;

            foreach (var dept in departmentsToGroup)
            {
                var deptClassrooms = classrooms.Where(c => c.DepartmentId == dept.Id).OrderBy(c => c.Number).ToList();
                if (!deptClassrooms.Any())
                    continue;

                var classroomCells = new List<ClassroomCellViewModel>();
                foreach (var classroom in deptClassrooms)
                {
                    var cell = await BuildCellViewModelAsync(classroom, dateOnly, startTime, endTime, scheduleEntries, pendingBookings);
                    classroomCells.Add(cell);
                }

                var first = deptClassrooms.First();
                departmentGroups.Add(new DepartmentClassroomsViewModel
                {
                    DepartmentId = dept.Id,
                    DepartmentName = dept.Name,
                    DepartmentAbbreviation = dept.Abbreviation,
                    Floor = first.Floor,
                    BuildingName = first.Building?.Name ?? "",
                    Classrooms = classroomCells
                });
            }

            // --- Собираем модель ---
            return new HomeViewModel
            {
                SelectedDate = date,
                SelectedPair = pairNumber,
                Pairs = pairs,
                DepartmentGroups = departmentGroups,
                SelectedBuildingId = buildingId,
                SelectedFloor = floor,
                SelectedDepartmentId = departmentId,
                Buildings = new SelectList(buildings, "Id", "Name", buildingId),
                Floors = new SelectList(floors.Select(f => new { Value = f, Text = f.ToString() }), "Value", "Text", floor),
                Departments = new SelectList(departments, "Id", "Name", departmentId)
            };
        }

        private async Task<ClassroomCellViewModel> BuildCellViewModelAsync(
            Classroom classroom,
            DateOnly date,
            TimeOnly startTime,
            TimeOnly endTime,
            List<ScheduleEntry> scheduleEntries,
            List<Booking> pendingBookings)
        {
            var cell = new ClassroomCellViewModel
            {
                ClassroomId = classroom.Id,
                ClassroomNumber = classroom.Number,
                Capacity = classroom.Capacity,
                Status = CellStatus.Free
            };

            // Проверяем долгосрочное закрепление
            if (!string.IsNullOrEmpty(classroom.LongTermBookingBy) &&
                (classroom.LongTermUntil == null || classroom.LongTermUntil >= date.ToDateTime(TimeOnly.MinValue)))
            {
                cell.Status = CellStatus.LongTerm;
                cell.LongTermInfo = classroom.LongTermBookingBy;
                return cell;
            }

            // Проверяем расписание (утверждённые)
            var scheduleEntry = scheduleEntries.FirstOrDefault(s => s.ClassroomId == classroom.Id);
            if (scheduleEntry != null)
            {
                cell.Status = CellStatus.Booked;
                cell.TeacherName = scheduleEntry.Teacher.FullName;
                return cell;
            }

            // Проверяем заявки на рассмотрении
            var pendingBooking = pendingBookings.FirstOrDefault(b =>
                b.ClassroomId == classroom.Id || b.ApprovedRoomId == classroom.Id);
            if (pendingBooking != null)
            {
                cell.Status = CellStatus.Pending;
                cell.TeacherName = pendingBooking.Teacher.FullName;
                cell.Purpose = pendingBooking.Purpose;
                cell.IsUrgent = pendingBooking.IsUrgent;
                cell.BookingId = pendingBooking.Id;
            }

            return cell;
        }

        public async Task<CellStatus> GetCellStatusAsync(
            int classroomId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var classroom = await _context.Classrooms.FindAsync(classroomId);
            if (classroom == null) return CellStatus.Free;

            if (!string.IsNullOrEmpty(classroom.LongTermBookingBy) &&
                (classroom.LongTermUntil == null ||
                 classroom.LongTermUntil >= date.ToDateTime(TimeOnly.MinValue)))
                return CellStatus.LongTerm;

            var hasSchedule = await _context.ScheduleEntries.AnyAsync(s =>
                s.ClassroomId == classroomId &&
                s.EventDate == date &&
                s.StartTime < endTime &&
                s.EndTime > startTime);

            if (hasSchedule) return CellStatus.Booked;

            var hasPending = await _context.Bookings.AnyAsync(b =>
                (b.ClassroomId == classroomId || b.ApprovedRoomId == classroomId) &&
                b.EventDate == date &&
                b.StartTime < endTime &&
                b.EndTime > startTime &&
                b.Status.Name == "Pending");

            return hasPending ? CellStatus.Pending : CellStatus.Free;
        }

        public async Task<ClassroomCellViewModel?> GetCellInfoAsync(
            int classroomId, DateOnly date, int pairNumber)
        {
            var pairs = await GetPairsAsync();
            var pair = pairs.FirstOrDefault(p => p.Number == pairNumber);
            if (pair == null) return null;

            var startTime = TimeOnly.Parse(pair.StartTime);
            var endTime = TimeOnly.Parse(pair.EndTime);

            var classroom = await _context.Classrooms
                .Include(c => c.Building)
                .FirstOrDefaultAsync(c => c.Id == classroomId);
            if (classroom == null) return null;

            var scheduleEntry = await _context.ScheduleEntries
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s =>
                    s.ClassroomId == classroomId &&
                    s.EventDate == date &&
                    s.StartTime < endTime &&
                    s.EndTime > startTime);

            if (scheduleEntry != null)
            {
                return new ClassroomCellViewModel
                {
                    ClassroomId = classroomId,
                    ClassroomNumber = classroom.Number,
                    Capacity = classroom.Capacity,
                    Status = CellStatus.Booked,
                    TeacherName = scheduleEntry.Teacher.FullName
                };
            }

            var pendingBooking = await _context.Bookings
                .Include(b => b.Teacher)
                .FirstOrDefaultAsync(b =>
                    (b.ClassroomId == classroomId || b.ApprovedRoomId == classroomId) &&
                    b.EventDate == date &&
                    b.StartTime < endTime &&
                    b.EndTime > startTime &&
                    b.Status.Name == "Pending");

            if (pendingBooking != null)
            {
                return new ClassroomCellViewModel
                {
                    ClassroomId = classroomId,
                    ClassroomNumber = classroom.Number,
                    Capacity = classroom.Capacity,
                    Status = CellStatus.Pending,
                    TeacherName = pendingBooking.Teacher.FullName,
                    Purpose = pendingBooking.Purpose,
                    IsUrgent = pendingBooking.IsUrgent,
                    BookingId = pendingBooking.Id
                };
            }

            return new ClassroomCellViewModel
            {
                ClassroomId = classroomId,
                ClassroomNumber = classroom.Number,
                Capacity = classroom.Capacity,
                Status = CellStatus.Free
            };
        }
    }
}