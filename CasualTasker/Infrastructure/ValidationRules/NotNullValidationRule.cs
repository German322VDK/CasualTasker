using System.Globalization;
using System.Windows.Controls;

namespace CasualTasker.Infrastructure.ValidationRules
{
    /// <summary>
    /// Custom validation rule to ensure that a value is not null.
    /// </summary>
    public sealed class NotNullValidationRule : ValidationRule
    {
        /// <summary>
        /// Validates the provided value to ensure it is not null.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>A ValidationResult indicating whether the validation succeeded or failed.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(value != null, "value must not be null!");
        }
    }
}
