using System.Windows.Markup;

namespace CasualTasker.Infrastructure.WPFExtension
{
    public class EnumCollectionExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumCollectionExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new ArgumentNullException(nameof(enumType), "Type must not be null and be enum");

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType != null)
            {
                return CreateEnumValueList(EnumType);
            }
            return default;
        }

        private List<object> CreateEnumValueList(Type enumType)
        {
            return Enum.GetNames(enumType)
                .Select(name => Enum.Parse(enumType, name))
                .ToList();
        }
    }
}
