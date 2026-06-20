// Services/ClassroomService.cs
using ClassroomManagement.Data;
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;
using ClassroomManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public ClassroomService(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<List<Classroom>> GetAllClassroomsAsync()
        {
            return await _context.Classrooms
                .Include(c => c.Building)
                .Include(c => c.RoomType)
                .Include(c => c.Condition)
                .Include(c => c.Department)
                .Include(c => c.ClassroomEquipments).ThenInclude(ce => ce.Equipment)
                .OrderBy(c => c.Building.Name)
                .ThenBy(c => c.Floor)
                .ThenBy(c => c.Number)
                .ToListAsync();
        }

        public async Task<Classroom?> GetClassroomByIdAsync(int id)
        {
            return await _context.Classrooms
                .Include(c => c.Building)
                .Include(c => c.RoomType)
                .Include(c => c.Condition)
                .Include(c => c.Department)
                .Include(c => c.ClassroomEquipments).ThenInclude(ce => ce.Equipment)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(bool Success, string Message)> CreateClassroomAsync(
            ClassroomFormViewModel model)
        {
            var classroom = new Classroom
            {
                Number = model.Number,
                BuildingId = model.BuildingId,
                Floor = model.Floor,
                RoomTypeId = model.RoomTypeId,
                Capacity = model.Capacity,
                ConditionId = model.ConditionId,
                DepartmentId = model.DepartmentId,
                LongTermBookingBy = null,
                LongTermUntil = model.LongTermUntil,
                EquipmentNote = model.EquipmentNote
            };
            if (model.SelectedTeacherId.HasValue)
            {
                var teacher = await _context.Teachers.FindAsync(model.SelectedTeacherId.Value);
                if (teacher != null)
                    classroom.LongTermBookingBy = teacher.FullName;
            }

            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            // Добавляем оборудование
            foreach (var equipId in model.SelectedEquipmentIds)
            {
                _context.ClassroomEquipments.Add(new ClassroomEquipment
                {
                    ClassroomId = classroom.Id,
                    EquipmentId = equipId
                });
            }
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(null, "CreateClassroom",
                $"Создан кабинет {classroom.Number}");

            return (true, "Кабинет успешно добавлен");
        }

        public async Task<(bool Success, string Message)> UpdateClassroomAsync(ClassroomFormViewModel model)
        {
            var classroom = await _context.Classrooms.FindAsync(model.Id);
            if (classroom == null) return (false, "Кабинет не найден");

            if (model.SelectedTeacherId.HasValue)
            {
                var teacher = await _context.Teachers.FindAsync(model.SelectedTeacherId.Value);
                classroom.LongTermBookingBy = teacher?.FullName;
            }
            else
            {
                classroom.LongTermBookingBy = null;
            }


            if (classroom == null)
                return (false, "Кабинет не найден");

            classroom.Number = model.Number;
            classroom.BuildingId = model.BuildingId;
            classroom.Floor = model.Floor;
            classroom.RoomTypeId = model.RoomTypeId;
            classroom.Capacity = model.Capacity;
            classroom.ConditionId = model.ConditionId;
            classroom.DepartmentId = model.DepartmentId;
            classroom.LongTermBookingBy = model.LongTermBookingBy;
            classroom.LongTermUntil = model.LongTermUntil;
            classroom.EquipmentNote = model.EquipmentNote;

            // Обновляем оборудование
            _context.ClassroomEquipments.RemoveRange(classroom.ClassroomEquipments);
            foreach (var equipId in model.SelectedEquipmentIds)
            {
                _context.ClassroomEquipments.Add(new ClassroomEquipment
                {
                    ClassroomId = classroom.Id,
                    EquipmentId = equipId
                });
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(null, "UpdateClassroom",
                $"Обновлён кабинет {classroom.Number}");

            return (true, "Кабинет успешно обновлён");
        }

        public async Task<(bool Success, string Message)> DeleteClassroomAsync(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
                return (false, "Кабинет не найден");

            // Проверяем наличие связанных записей расписания
            var hasSchedule = await _context.ScheduleEntries
                .AnyAsync(s => s.ClassroomId == id);
            if (hasSchedule)
                return (false, "Нельзя удалить кабинет с записями в расписании");

            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(null, "DeleteClassroom",
                $"Удалён кабинет {classroom.Number}");

            return (true, "Кабинет удалён");
        }

        public async Task<ClassroomFormViewModel> GetClassroomFormViewModelAsync(int? id = null)
        {
            var model = new ClassroomFormViewModel();

            if (id.HasValue)
            {
                var classroom = await GetClassroomByIdAsync(id.Value);
                if (classroom != null)
                {

                    model.Id = classroom.Id;
                    model.Number = classroom.Number;
                    model.BuildingId = classroom.BuildingId;
                    model.Floor = classroom.Floor;
                    model.RoomTypeId = classroom.RoomTypeId;
                    model.Capacity = classroom.Capacity;
                    model.ConditionId = classroom.ConditionId;
                    model.DepartmentId = classroom.DepartmentId;
                    model.LongTermBookingBy = classroom.LongTermBookingBy;
                    model.LongTermUntil = classroom.LongTermUntil;
                    model.EquipmentNote = classroom.EquipmentNote;
                    model.SelectedEquipmentIds = classroom.ClassroomEquipments
                        .Select(ce => ce.EquipmentId).ToList();
                    if (!string.IsNullOrEmpty(classroom.LongTermBookingBy))
                    {
                        var teacher = await _context.Teachers
                            .FirstOrDefaultAsync(t => t.FullName == classroom.LongTermBookingBy);
                        model.SelectedTeacherId = teacher?.Id;
                    }
                }
            }

            // Заполняем выпадающие списки
            model.Buildings = new SelectList(
                await _context.Buildings.OrderBy(b => b.Name).ToListAsync(),
                "Id", "Name", model.BuildingId);

            model.RoomTypes = new SelectList(
                await _context.RoomTypes.OrderBy(r => r.Name).ToListAsync(),
                "Id", "Name", model.RoomTypeId);

            model.Conditions = new SelectList(
                await _context.RoomConditions.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name", model.ConditionId);

            model.Departments = new SelectList(
                await _context.Departments.OrderBy(d => d.Name).ToListAsync(),
                "Id", "Name", model.DepartmentId);

            var allEquipment = await _context.Equipment.OrderBy(e => e.Name).ToListAsync();
            model.EquipmentList = allEquipment.Select(e => new EquipmentCheckboxViewModel
            {
                Id = e.Id,
                Name = e.Name,
                IsSelected = model.SelectedEquipmentIds.Contains(e.Id)
            }).ToList();


            var teachers = await _context.Teachers
        .OrderBy(t => t.FullName)
        .Select(t => new { t.Id, t.FullName })
        .ToListAsync();

            model.TeachersList = new SelectList(teachers, "Id", "FullName", model.SelectedTeacherId);

            return model;
        }
    }
}