// Models/Entities/BaseEntity.cs
namespace ClassroomManagement.Models.Entities
{
    /// <summary>
    /// Базовый класс для всех сущностей с первичным ключом
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}