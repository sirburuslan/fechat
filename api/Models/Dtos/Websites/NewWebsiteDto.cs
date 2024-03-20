/*
 * @class New Website Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-13
 *
 * This class is used for website data sanitization and validation
 */

// Namespace for Websites Dtos
namespace FeChat.Models.Dtos.Websites {

    // Use the data annotations for validation
    using System.ComponentModel.DataAnnotations;

    // Use Web system for html sanitizing
    using System.Web;

    // Text Enconding for Javascript sanitizing
    using System.Text.Encodings.Web;

    // Use the General Utils for Error Messages
    using FeChat.Utils.General;

    // Use the custom validators
    using FeChat.Utils.Validations;

    /// <summary>
    /// Dto for New Websites
    /// </summary>
    public class NewWebsiteDto {

        /// <summary>
        /// Chat header container
        /// </summary>
        private string? _header;   

        /// <summary>
        /// Website name container
        /// </summary>
        private string? _name;   

        /// <summary>
        /// Website url container
        /// </summary>
        private string? _url;     
        
        /// <summary>
        /// Website domain container
        /// </summary>
        private string? _domain;               

        /// <summary>
        /// Website's ID field
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// Member's ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Chat status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int Enabled { get; set; }   

        /// <summary>
        /// Chat header field
        /// </summary>
        [StringLength(20, MinimumLength = 0, ErrorMessageResourceName = "ChatHeaderLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Header {
            get => _header;
            set => _header = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Website name field
        /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessageResourceName = "WebsiteNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Name {
            get => _name;
            set => _name = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Website url field
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(250, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Url {
            get => _url;
            set => _url = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();            
        }

        /// <summary>
        /// Website domain field
        /// </summary>
        [StringLength(250, MinimumLength = 0, ErrorMessageResourceName = "DomainLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Domain {
            get => _domain;
            set => _domain = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Website created field
        /// </summary>
        public int Created { get; set; }

    }

}