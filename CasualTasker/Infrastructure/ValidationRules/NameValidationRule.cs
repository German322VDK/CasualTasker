using System.Globalization;
using System.Windows.Controls;

namespace CasualTasker.Infrastructure.ValidationRules
{
    /// <summary>
    /// Custom validation rule to ensure that a name is not empty or whitespace.
    /// </summary>
    public class NameValidationRule : ValidationRule
    {
        /// <summary>
        /// Validates the provided value to ensure it is a non-empty string.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>A ValidationResult indicating whether the validation succeeded or failed.</returns>
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
