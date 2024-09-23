using CasualTasker.DTO.Base;
using System.Drawing;

namespace CasualTasker.DTO
{
    /// <summary>
    /// Represents a Data Transfer Object for a task.
    /// </summary>
    public class TaskDTO : NamedEntity
    {
        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the due date of the task.
        /// </summary>
        public DateTime DueDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the status of the task.
        /// </summary>
        public CasualTaskStatus Status { get; set; } = CasualTaskStatus.InProgress;

        /// <summary>
        /// Gets or sets the category associated with the task.
        /// </summary>
        public virtual CategoryDTO Category { get; set; }

        /// <summary>
        /// Creates a new copy (clone) of the current <see cref="TaskDTO"/> instance.
        /// </summary>
        /// <returns>A new instance of <see cref="TaskDTO"/> with the same properties.</returns>
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

        /// <summary>
        /// Updates the current task's properties based on another instance of <see cref="TaskDTO"/>.
        /// </summary>
        /// <param name="entity">The source task from which to copy values.</param>
        /// <exception cref="ArgumentException">thrown when the provided entity is not of type <see cref="TaskDTO"/>.</exception>
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

        /// <summary>
        /// Returns a string representation of the task
        /// </summary>
        /// <returns>A string that represents the current task.</returns>
        public override string ToString() =>
            $"{{ Задача Id:{Id}, Имя:{Name}, Описание:{Description},  Статус:{Status}, дата выполнения:{DueDate}, Категоря:{Category}}}";
    }

    /// <summary>
    /// Defines the various statuses that a task can have.
    /// </summary>
    public enum CasualTaskStatus
    {
        /// <summary>
        /// The task is currently in progress.
        /// </summary>
        InProgress = 1,
        /// <summary>
        /// The task has been completed.
        /// </summary>
        Completed = 2,
        /// <summary>
        /// The task has been postponed.
        /// </summary>
        Postponed = 3,
    }
}
