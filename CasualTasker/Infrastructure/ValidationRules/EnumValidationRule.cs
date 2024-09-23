using System.Globalization;
using System.Windows.Controls;

namespace CasualTasker.Infrastructure.ValidationRules
{
    /// <summary>
    /// Custom validation rule to ensure that an enum value is not the default (0).
    /// </summary>
    public class EnumValidationRule : ValidationRule
    {
        /// <summary>
        /// Validates the provided value against the rules for enums.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>A ValidationResult indicating whether the validation succeeded or failed.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Value cannot be null.");
            if (value.GetType().IsEnum)
            {
                int intValue = (int)value;

                return new ValidationResult(intValue != 0, "Enum value must not be the default (0)!");
            }
            else
            {
                return new ValidationResult(false, "value must be enum!");
            }

        }
    }
}
