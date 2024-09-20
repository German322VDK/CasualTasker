using System.Globalization;
using System.Windows.Controls;

namespace CasualTasker.Infrastructure.ValidationRules
{
    public class NotNullValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(value != null, "value must not be null!");
        }
    }
}
