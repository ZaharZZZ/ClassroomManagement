// Data/ApplicationDbContext.cs
using ClassroomManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet для каждой сущности
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomCondition> RoomConditions { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<ClassroomEquipment> ClassroomEquipments { get; set; }
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Составной первичный ключ для ClassroomEquipment
            modelBuilder.Entity<ClassroomEquipment>()
                .HasKey(ce => new { ce.ClassroomId, ce.EquipmentId });

            // Настройка связи ClassroomEquipment
            modelBuilder.Entity<ClassroomEquipment>()
                .HasOne(ce => ce.Classroom)
                .WithMany(c => c.ClassroomEquipments)
                .HasForeignKey(ce => ce.ClassroomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassroomEquipment>()
                .HasOne(ce => ce.Equipment)
                .WithMany(e => e.ClassroomEquipments)
                .HasForeignKey(ce => ce.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Предотвращение циклических каскадных удалений
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Head)
                .WithMany()
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Approver)
                .WithMany()
                .HasForeignKey(b => b.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ApprovedRoom)
                .WithMany()
                .HasForeignKey(b => b.ApprovedRoomId)
                .OnDelete(DeleteBehavior.SetNull);

            // Уникальный логин пользователя
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Уникальный ключ настройки
            modelBuilder.Entity<SystemSetting>()
                .HasIndex(s => s.SettingKey)
                .IsUnique();
        }
    }
}