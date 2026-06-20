// Data/DbInitializer.cs
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            IAuthService authService)
        {
            try
            {
                // Убеждаемся что БД доступна
                await context.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Нет подключения к БД: {ex.Message}");
                throw;
            }

            // Проверяем наличие ролей — если нет, создаём базовые данные
            if (!await context.Roles.AnyAsync())
            {
                await SeedRolesAsync(context);
            }

            if (!await context.BookingStatuses.AnyAsync())
            {
                await SeedBookingStatusesAsync(context);
            }

            if (!await context.RoomConditions.AnyAsync())
            {
                await SeedRoomConditionsAsync(context);
            }

            if (!await context.RoomTypes.AnyAsync())
            {
                await SeedRoomTypesAsync(context);
            }

            if (!await context.Buildings.AnyAsync())
            {
                await SeedBuildingsAsync(context);
            }

            if (!await context.SystemSettings.AnyAsync())
            {
                await SeedSystemSettingsAsync(context);
            }

            if (!await context.Departments.AnyAsync())
            {
                await SeedDepartmentsAsync(context);
            }

            if (!await context.Teachers.AnyAsync())
            {
                await SeedTeachersAsync(context);
            }

            if (!await context.Classrooms.AnyAsync())
            {
                await SeedClassroomsAsync(context);
            }

            // Пользователи — ВСЕГДА проверяем и создаём если нет
            await SeedUsersAsync(context, authService);

            Console.WriteLine("✓ DbInitializer завершён успешно");
        }

        // ============================================
        // РОЛИ
        // ============================================
        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            context.Roles.AddRange(
                new Role { Name = "Admin", Description = "Администратор системы" },
                new Role { Name = "Dispatcher", Description = "Сотрудник учебной части" },
                new Role { Name = "Teacher", Description = "Преподаватель" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Роли созданы");
        }

        // ============================================
        // СТАТУСЫ ЗАЯВОК
        // ============================================
        private static async Task SeedBookingStatusesAsync(ApplicationDbContext context)
        {
            context.BookingStatuses.AddRange(
                new BookingStatus { Name = "Pending", Description = "На рассмотрении" },
                new BookingStatus { Name = "Approved", Description = "Утверждена" },
                new BookingStatus { Name = "Rejected", Description = "Отклонена" },
                new BookingStatus { Name = "Cancelled", Description = "Отменена" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Статусы заявок созданы");
        }

        // ============================================
        // СОСТОЯНИЯ АУДИТОРИЙ
        // ============================================
        private static async Task SeedRoomConditionsAsync(ApplicationDbContext context)
        {
            context.RoomConditions.AddRange(
                new RoomCondition { Name = "Исправна", Description = "Аудитория в рабочем состоянии" },
                new RoomCondition { Name = "Ремонт", Description = "Аудитория на ремонте" },
                new RoomCondition { Name = "Закрыта", Description = "Аудитория временно закрыта" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Состояния аудиторий созданы");
        }

        // ============================================
        // ТИПЫ АУДИТОРИЙ
        // ============================================
        private static async Task SeedRoomTypesAsync(ApplicationDbContext context)
        {
            context.RoomTypes.AddRange(
                new RoomType { Name = "Учебный кабинет", Description = "Стандартный учебный кабинет" },
                new RoomType { Name = "Лаборатория", Description = "Лабораторное помещение" },
                new RoomType { Name = "Компьютерный класс", Description = "Класс с компьютерами" },
                new RoomType { Name = "Лекционный зал", Description = "Большой лекционный зал" },
                new RoomType { Name = "Спортивный зал", Description = "Спортивное помещение" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Типы аудиторий созданы");
        }

        // ============================================
        // КОРПУСА
        // ============================================
        private static async Task SeedBuildingsAsync(ApplicationDbContext context)
        {
            context.Buildings.AddRange(
                new Building { Name = "Основной корпус", Address = "ул. Примерная, д. 1" },
                new Building { Name = "Учебный корпус №2", Address = "ул. Примерная, д. 2" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Корпуса созданы");
        }

        // ============================================
        // СИСТЕМНЫЕ НАСТРОЙКИ (расписание пар)
        // ============================================
        private static async Task SeedSystemSettingsAsync(ApplicationDbContext context)
        {
            context.SystemSettings.AddRange(
                new SystemSetting { SettingKey = "pair_1_start", SettingValue = "08:00", Description = "Начало 1-й пары" },
                new SystemSetting { SettingKey = "pair_1_end", SettingValue = "09:35", Description = "Конец 1-й пары" },
                new SystemSetting { SettingKey = "pair_2_start", SettingValue = "09:45", Description = "Начало 2-й пары" },
                new SystemSetting { SettingKey = "pair_2_end", SettingValue = "11:20", Description = "Конец 2-й пары" },
                new SystemSetting { SettingKey = "pair_3_start", SettingValue = "11:30", Description = "Начало 3-й пары" },
                new SystemSetting { SettingKey = "pair_3_end", SettingValue = "13:05", Description = "Конец 3-й пары" },
                new SystemSetting { SettingKey = "pair_4_start", SettingValue = "13:35", Description = "Начало 4-й пары" },
                new SystemSetting { SettingKey = "pair_4_end", SettingValue = "15:10", Description = "Конец 4-й пары" },
                new SystemSetting { SettingKey = "pair_5_start", SettingValue = "15:20", Description = "Начало 5-й пары" },
                new SystemSetting { SettingKey = "pair_5_end", SettingValue = "16:55", Description = "Конец 5-й пары" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Системные настройки созданы");
        }

        // ============================================
        // ОТДЕЛЕНИЯ
        // ============================================
        private static async Task SeedDepartmentsAsync(ApplicationDbContext context)
        {
            context.Departments.AddRange(
                new Department { Name = "Заочное отделение", Abbreviation = "ЗО" },
                new Department { Name = "Дневное отделение", Abbreviation = "ДО" },
                new Department { Name = "Вечернее отделение", Abbreviation = "ВО" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Отделения созданы");
        }

        // ============================================
        // ПРЕПОДАВАТЕЛИ
        // ============================================
        private static async Task SeedTeachersAsync(ApplicationDbContext context)
        {
            var dept1 = await context.Departments.FirstOrDefaultAsync(d => d.Abbreviation == "ЗО");
            var dept2 = await context.Departments.FirstOrDefaultAsync(d => d.Abbreviation == "ДО");
            var dept3 = await context.Departments.FirstOrDefaultAsync(d => d.Abbreviation == "ВО");

            context.Teachers.AddRange(
                new Teacher
                {
                    FullName = "Иванов Иван Иванович",
                    AcademicDegree = "Кандидат наук",
                    DepartmentId = dept1?.Id
                },
                new Teacher
                {
                    FullName = "Петрова Мария Сергеевна",
                    AcademicDegree = null,
                    DepartmentId = dept2?.Id
                },
                new Teacher
                {
                    FullName = "Сидоров Алексей Петрович",
                    AcademicDegree = "Доктор наук",
                    DepartmentId = dept3?.Id
                }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Преподаватели созданы");
        }

        // ============================================
        // АУДИТОРИИ
        // ============================================
        private static async Task SeedClassroomsAsync(ApplicationDbContext context)
        {
            var building1 = await context.Buildings.FirstOrDefaultAsync(b => b.Name == "Основной корпус");
            var typeKabinet = await context.RoomTypes.FirstOrDefaultAsync(t => t.Name == "Учебный кабинет");
            var typeComp = await context.RoomTypes.FirstOrDefaultAsync(t => t.Name == "Компьютерный класс");
            var typeLab = await context.RoomTypes.FirstOrDefaultAsync(t => t.Name == "Лаборатория");
            var typeLect = await context.RoomTypes.FirstOrDefaultAsync(t => t.Name == "Лекционный зал");
            var condOk = await context.RoomConditions.FirstOrDefaultAsync(c => c.Name == "Исправна");
            var dept1 = await context.Departments.FirstOrDefaultAsync(d => d.Abbreviation == "ЗО");
            var dept2 = await context.Departments.FirstOrDefaultAsync(d => d.Abbreviation == "ДО");
            var dept3 = await context.Departments.FirstOrDefaultAsync(d => d.Abbreviation == "ВО");

            if (building1 == null || typeKabinet == null || condOk == null) return;

            context.Classrooms.AddRange(
                new Classroom { Number = "101", BuildingId = building1.Id, Floor = 1, RoomTypeId = typeKabinet.Id, Capacity = 30, ConditionId = condOk.Id, DepartmentId = dept1?.Id },
                new Classroom { Number = "102", BuildingId = building1.Id, Floor = 1, RoomTypeId = typeKabinet.Id, Capacity = 25, ConditionId = condOk.Id, DepartmentId = dept1?.Id },
                new Classroom { Number = "103", BuildingId = building1.Id, Floor = 1, RoomTypeId = typeComp?.Id ?? typeKabinet.Id, Capacity = 20, ConditionId = condOk.Id, DepartmentId = dept1?.Id },
                new Classroom { Number = "104", BuildingId = building1.Id, Floor = 1, RoomTypeId = typeKabinet.Id, Capacity = 30, ConditionId = condOk.Id, DepartmentId = dept2?.Id },
                new Classroom { Number = "105", BuildingId = building1.Id, Floor = 1, RoomTypeId = typeKabinet.Id, Capacity = 25, ConditionId = condOk.Id, DepartmentId = dept2?.Id },
                new Classroom { Number = "201", BuildingId = building1.Id, Floor = 2, RoomTypeId = typeLab?.Id ?? typeKabinet.Id, Capacity = 15, ConditionId = condOk.Id, DepartmentId = dept2?.Id },
                new Classroom { Number = "202", BuildingId = building1.Id, Floor = 2, RoomTypeId = typeKabinet.Id, Capacity = 30, ConditionId = condOk.Id, DepartmentId = dept3?.Id },
                new Classroom { Number = "203", BuildingId = building1.Id, Floor = 2, RoomTypeId = typeLect?.Id ?? typeKabinet.Id, Capacity = 100, ConditionId = condOk.Id, DepartmentId = dept3?.Id }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Аудитории созданы");
        }

        // ============================================
        // ПОЛЬЗОВАТЕЛИ — главный метод
        // ============================================
        private static async Task SeedUsersAsync(
            ApplicationDbContext context,
            IAuthService authService)
        {
            // Проверяем каждого пользователя отдельно
            await CreateUserIfNotExistsAsync(context, authService,
                login: "admin",
                password: "Admin123!",
                roleName: "Admin",
                teacherFullName: null);

            await CreateUserIfNotExistsAsync(context, authService,
                login: "dispatcher",
                password: "Disp123!",
                roleName: "Dispatcher",
                teacherFullName: null);

            await CreateUserIfNotExistsAsync(context, authService,
                login: "teacher1",
                password: "Teacher123!",
                roleName: "Teacher",
                teacherFullName: "Иванов Иван Иванович");
        }

        private static async Task CreateUserIfNotExistsAsync(
            ApplicationDbContext context,
            IAuthService authService,
            string login,
            string password,
            string roleName,
            string? teacherFullName)
        {
            // Проверяем существует ли пользователь
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser != null)
            {
                // Проверяем валидность хеша
                bool hashValid = false;
                try
                {
                    hashValid = authService.VerifyPassword(password, existingUser.PasswordHash);
                }
                catch
                {
                    hashValid = false;
                }

                if (!hashValid)
                {
                    // Хеш невалидный (placeholder) — обновляем
                    existingUser.PasswordHash = authService.HashPassword(password);
                    existingUser.IsBlocked = false;
                    await context.SaveChangesAsync();
                    Console.WriteLine($"✓ Обновлён хеш пароля для пользователя: {login}");
                }
                else
                {
                    Console.WriteLine($"✓ Пользователь {login} уже существует с валидным хешем");
                }
                return;
            }

            // Получаем роль
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                Console.WriteLine($"✗ Роль {roleName} не найдена! Пользователь {login} не создан.");
                return;
            }

            // Получаем преподавателя если нужно
            int? teacherId = null;
            if (!string.IsNullOrEmpty(teacherFullName))
            {
                var teacher = await context.Teachers
                    .FirstOrDefaultAsync(t => t.FullName == teacherFullName);
                teacherId = teacher?.Id;
            }

            // Создаём пользователя
            var newUser = new User
            {
                Login = login,
                PasswordHash = authService.HashPassword(password),
                RoleId = role.Id,
                TeacherId = teacherId,
                IsBlocked = false,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Создан пользователь: {login} (роль: {roleName})");
        }
    }
}