using CasualTasker.DTO.Base;

namespace CasualTasker.DTO
{
    /// <summary>
    /// Represents a Data Transfer Object for a category.
    /// </summary>
    public class CategoryDTO : NamedEntity
    {
        /// <summary>
        /// Gets or sets the color associated with the category.
        /// Default value is yellow (#FFFF00).
        /// </summary>
        public string Color { get; set; } = "#FFFF00";

        /// <summary>
        /// Creates a new copy (clone) of the current category instance.
        /// </summary>
        /// <returns>A new instance of <see cref="CategoryDTO"/> with the same properties.</returns>
        public override object Clone() =>
            new CategoryDTO
            {
                Id = Id,
                Name = Name,
                Color = Color
            };

        /// <summary>
        /// Updates the current category's properties based on another instance of <see cref="CategoryDTO"/>.
        /// </summary>
        /// <param name="entity">The source category from which to copy values.</param>
        /// <exception cref="ArgumentException">Thrown when the provided entity is not of type <see cref="CategoryDTO"/>.</exception>
        public override void UpdateFrom(NamedEntity entity)
        {
            base.UpdateFrom(entity);
            if(entity is CategoryDTO category)
            {
                Color = category.Color;
            }
            else
            {
                throw new ArgumentException($"entity.Type: {entity.GetType().Name} not equal to type:{nameof(CategoryDTO)}");
            }
        }
        /// <summary>
        /// Returns a string representation of the category, including its Id, Name, and Color.
        /// </summary>
        /// <returns>A string that represents the current category.</returns>
        public override string ToString() =>
            $"{{ Категория Id:{Id} Имя:{Name} Цвет:{Color} }}";
    }
}
