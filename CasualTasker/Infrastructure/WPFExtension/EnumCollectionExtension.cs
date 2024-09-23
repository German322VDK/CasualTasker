using System.Windows.Markup;

namespace CasualTasker.Infrastructure.WPFExtension
{
    /// <summary>
    /// Provides a markup extension that generates a list of values from a specified enum type.
    /// This can be used in XAML to bind to enum values, enabling easier access to enum members.
    /// </summary>
    public sealed class EnumCollectionExtension : MarkupExtension
    {
        /// <summary>
        /// Gets the type of the enum from which values are generated.
        /// </summary>
        public Type EnumType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCollectionExtension"/> class.
        /// </summary>
        /// <param name="enumType">The type of the enum to generate values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enumType"/> is null or not an enum type.</exception>
        public EnumCollectionExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new ArgumentNullException(nameof(enumType), "Type must not be null and be enum");

            EnumType = enumType;
        }

        /// <summary>
        /// Provides the generated list of enum values for use in XAML.
        /// </summary>
        /// <param name="serviceProvider">The service provider for obtaining services.</param>
        /// <returns>A list of enum values.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType != null)
            {
                return CreateEnumValueList(EnumType);
            }
            return default;
        }

        /// <summary>
        /// Creates a list of enum values from the specified enum type.
        /// </summary>
        /// <param name="enumType">The type of the enum to create a value list from.</param>
        /// <returns>A list of enum values.</returns>
        private List<object> CreateEnumValueList(Type enumType)
        {
            return Enum.GetNames(enumType)
                .Select(name => Enum.Parse(enumType, name))
                .ToList();
        }
    }
}
