using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.Attributes
{
    /// <summary>
    /// Validates that a decimal value is a multiple of 1000
    /// </summary>
    public class MultipleOf1000Attribute : ValidationAttribute
    {
        public MultipleOf1000Attribute() : base("Please enter a multiple of 1000")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            if (value is decimal decimalValue)
            {
                // Check if the value is a multiple of 1000
                // Use modulo operation - if remainder is 0, it's a multiple
                if (decimalValue % 1000m != 0)
                {
                    return new ValidationResult(ErrorMessage ?? "Please enter a multiple of 1000");
                }
            }
            else if (value is double doubleValue)
            {
                // Convert to decimal for accurate comparison
                decimal decimalVal = (decimal)doubleValue;
                if (decimalVal % 1000m != 0)
                {
                    return new ValidationResult(ErrorMessage ?? "Please enter a multiple of 1000");
                }
            }
            else
            {
                // Try to convert to decimal
                if (decimal.TryParse(value.ToString(), out decimal parsedValue))
                {
                    if (parsedValue % 1000m != 0)
                    {
                        return new ValidationResult(ErrorMessage ?? "Please enter a multiple of 1000");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
