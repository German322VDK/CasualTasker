using CasualTasker.DTO.Base;

namespace CasualTasker.DTO
{
    public class TaskDTO : NamedEntity
    {
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = DateTime.Now;
        public CasualTaskStatus Status { get; set; } = CasualTaskStatus.InProgress;

        public virtual CategoryDTO Category { get; set; }

        public override object Clone() =>
            new TaskDTO
            {
                Id = Id,
                Description = Description,
                Status = Status,
                Category = Category.Clone() as CategoryDTO,
                DueDate = DueDate,
                Name = Name,
            };

        public override void UpdateFrom(NamedEntity entity)
        {
            base.UpdateFrom(entity);
            if (entity is TaskDTO task)
            {
                Description = task.Description;
                Status = task.Status;
                DueDate = task.DueDate;
                Category = task.Category.Clone() as CategoryDTO;
            }
            else
            {
                throw new ArgumentException($"entity.Type: {entity.GetType().Name} not equal to type:{nameof(TaskDTO)}");
            }
        }
    }

    public enum CasualTaskStatus
    {
        InProgress = 1,
        Completed = 2,
        Postponed = 3,
    }
}
