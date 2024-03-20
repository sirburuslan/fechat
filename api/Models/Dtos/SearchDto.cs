/*
 * @class Search Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-11
 *
 * This class is used to validate and transfer the search parameters
 */

// Namespace for Dtos
namespace FeChat.Models.Dtos {

    // Use the annotations for validation
    using System.ComponentModel.DataAnnotations;

    // Web classes for html sanitize 
    using System.Web;

    // Use web encoding for javascript sanitizing
    using System.Text.Encodings.Web;
    
    // Use the general utils for error messages
    using FeChat.Utils.General;
    
    // Use custom validation
    using FeChat.Utils.Validations;

    /// <summary>
    /// Search Dto
    /// </summary>
    public class SearchDto {

        /// <summary>
        /// Search keys container
        /// </summary>
        private string? _search;

        /// <summary>
        /// Search field
        /// </summary>
        [StringLength(250, MinimumLength = 0, ErrorMessageResourceName = "FirstNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Search {
            get => _search;
            set => _search = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Page number field
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 5000, ErrorMessage = "SupportedValueShouldBe")]
        public int Page { get; set; }

    }

}