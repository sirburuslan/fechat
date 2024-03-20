/*
 * @class UrlValidationAttribute
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-01
 *
 * This class is used to validate the urls attributes
 */

// Namespace for Validations
namespace FeChat.Utils.Validations {
    
    // Use the DataAnnotations to create a new validation
    using System.ComponentModel.DataAnnotations;

    // Use the General namespace for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Create a custom Validation for urls
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class UrlValidationAttribute : ValidationAttribute {

        /// <summary>
        /// Check if the value is valid
        /// </summary>
        /// <param name="value">Received value</param>
        /// <param name="validationContext">Describes the context in which a validation check is performed.</param>
        /// <returns>True if the value is correct or false</returns>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
           
            // Check if value exists
            if (value == null) {

                // Do nothing if the value is null
                return ValidationResult.Success!;

            } else if (value is string url) {

                // If value exists verify if value is an url
                if ((url.Length > 0) && !Uri.IsWellFormedUriString(url, UriKind.Absolute)) {

                    // Return an error message
                    return new ValidationResult(new Strings().Get("UrlNotValid"));

                }

                // Do nothing if the value is url
                return ValidationResult.Success!;

            } else {

                // Do nothing if the value is empty
                return ValidationResult.Success!;

            }

        }

    }

}