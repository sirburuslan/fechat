/*
 * @class Message Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-26
 *
 * This class is used for messages data sanitization and validation
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
    /// Message Dto
    /// </summary>
    public class MessageDto {

        /// <summary>
        /// Secret container
        /// </summary>
        public string? _secret;        

        /// <summary>
        /// Message container
        /// </summary>
        public string? _message;

        /// <summary>
        /// Message ID field
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Website ID field
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// Thread ID field
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Member ID field
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Thread Secret field
        /// </summary>
        [StringLength(50, ErrorMessageResourceName = "ThreadSecretLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? ThreadSecret {
            get => _secret;
            set => _secret = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();            
        }        

        /// <summary>
        /// Message field
        /// </summary>
        [StringLength(2000, ErrorMessageResourceName = "MessageLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Message {
            get => _message;
            set => _message = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Seen field
        /// </summary>
        public int Seen { get; set; }         

        /// <summary>
        /// Created field
        /// </summary>
        public int Created { get; set; } 
        
        /// <summary>
        /// Attachments field
        /// </summary>
        public string[]? Attachments { get; set; }             

    }

}