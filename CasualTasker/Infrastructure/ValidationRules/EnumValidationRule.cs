using System.Globalization;
using System.Windows.Controls;

namespace CasualTasker.Infrastructure.ValidationRules
{
    public class EnumValidationRule : ValidationRule
    {
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
