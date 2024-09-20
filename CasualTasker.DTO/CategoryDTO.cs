using CasualTasker.DTO.Base;

namespace CasualTasker.DTO
{
    public class CategoryDTO : NamedEntity
    {
        public string Color { get; set; } = "#FFFF00";

        public override object Clone() =>
            new CategoryDTO
            {
                Id = Id,
                Name = Name,
                Color = Color
            };

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

        public override string ToString() =>
            $"{{ Категория Id:{Id} Имя:{Name} Цвет:{Color} }}";
    }
}
