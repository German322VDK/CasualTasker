using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CasualTasker.DTO.Base
{
    public abstract class NamedEntity : ICloneable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public abstract object Clone();

        public virtual void UpdateFrom(NamedEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Id = entity.Id;
            Name = entity.Name;
        }
    }
}
