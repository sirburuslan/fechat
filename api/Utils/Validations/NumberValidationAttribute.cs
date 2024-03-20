/*
 * @class NumberValidationAttribute
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used to validate the numbers attributes
 */

// Namespace for Validations
namespace FeChat.Utils.Validations {
    
    // Use the DataAnnotations to create a new validation
    using System.ComponentModel.DataAnnotations;

    // Use the General namespace for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Create a custom Validation for numbers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NumberValidationAttribute : ValidationAttribute {

        /// <summary>
        /// Minimum allowed number
        /// </summary>
        public double Minimum { get; set; } = double.MinValue;

        /// <summary>
        /// Maximum allowed number
        /// </summary>
        public double Maximum { get; set; } = double.MaxValue;

        /// <summary>
        /// Error Message
        /// </summary>
        public required new string ErrorMessage { get; set; }

        /// <summary>
        /// Check if the value is valid
        /// </summary>
        /// <param name="value">Received value</param>
        /// <param name="validationContext">Describes the context in which a validation check is performed.</param>
        /// <returns>True if the value is correct or false</returns>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {

            // Check if value is null
            if (value == null) {

                // The value is valid
                return ValidationResult.Success!;

            }

            // Check if the value is convertible
            if (value is IConvertible convertibleValue) {
                
                // Check if the value can be converted to a number
                try {

                    // Convert to duble format
                    double? numberValue = Convert.ToDouble(convertibleValue);

                    // Check against minimum and maximum values
                    if (numberValue < Minimum || numberValue > Maximum) {

                        // Create error message
                        string errorMessage = new Strings().Get(ErrorMessage);

                        // Replace min
                        errorMessage = errorMessage.Replace("[min]", Minimum.ToString());

                        // Replace max
                        errorMessage = errorMessage.Replace("[max]", Maximum.ToString());

                        // Return the error message
                        return new ValidationResult(errorMessage);

                    }

                    // The value is valid
                    return ValidationResult.Success!;

                } catch (FormatException) {

                    // The value is invalid
                    return new ValidationResult(ErrorMessage);

                }

            }

            // Return the error message
            return new ValidationResult(new Strings().Get("NoValidNumber"));

        }

    }

}