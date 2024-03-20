/*
 * @class New Thread Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-04
 *
 * This class is used for thread data validation and transfer
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    // Use the data annotations for validation
    using System.ComponentModel.DataAnnotations;

    // Use Web system for html sanitizing
    using System.Web;

    // Text Enconding for Javascript sanitizing
    using System.Text.Encodings.Web;

    // Use General utils for errors messages
    using FeChat.Utils.General;

    /// <summary>
    /// NewThread Dto
    /// </summary>
    public class NewThreadDto {

        /// <summary>
        /// Guest Name container
        /// </summary>
        private string? _name;

        /// <summary>
        /// Guest Email container
        /// </summary>
        private string? _email; 

        /// <summary>
        /// Guest Message container
        /// </summary>
        private string? _message;      

        /// <summary>
        /// Website Id
        /// </summary>
        public int WebsiteId { get; set; }       

        /// <summary>
        /// Guest Name field
        /// </summary>
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "GuestNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Name {
            get => _name;
            set => _name = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();            
        }
        
        /// <summary>
        /// Guest Email field
        /// </summary>
        [StringLength(200, MinimumLength = 1, ErrorMessageResourceName = "EmailNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Email {
            get => _email;
            set => _email = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();            
        } 

        /// <summary>
        /// Message field
        /// </summary>
        [StringLength(2000, ErrorMessageResourceName = "MessageLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Message {
            get => _message;
            set => _message = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

    }

}