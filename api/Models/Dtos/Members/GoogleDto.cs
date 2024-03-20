/*
 * @class Code Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-19
 *
 * This class is used for google data sanitization and validation
 */

// Namespace for Members Dtos
namespace FeChat.Models.Dtos.Members {

    // Use the data annotations for validation
    using System.ComponentModel.DataAnnotations;

    // Use Web system for html sanitizing
    using System.Web;

    // Text Enconding for Javascript sanitizing
    using System.Text.Encodings.Web;

    // Use General utils for errors messages
    using FeChat.Utils.General;

    /// <summary>
    /// Google Dto
    /// </summary>
    public class GoogleDto {

        /// <summary>
        /// Code container
        /// </summary>
        private string? _code;

        /// <summary>
        /// Code field
        /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessageResourceName = "CodeLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Code {
            get => _code;
            set => _code = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

    }

}