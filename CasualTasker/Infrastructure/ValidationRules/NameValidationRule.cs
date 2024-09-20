using System.Globalization;
using System.Windows.Controls;

namespace CasualTasker.Infrastructure.ValidationRules
{
    public class NameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string name)
            {
                bool result = !string.IsNullOrWhiteSpace(name);
                return new ValidationResult(result, "Name не может быть пустым");
            }
            else
                return new ValidationResult(false, $"value is wrong typed: {value.GetType().Name}");
        }
    }
}
