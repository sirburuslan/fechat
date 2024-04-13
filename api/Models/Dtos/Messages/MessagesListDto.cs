/*
 * @class Messages List Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-27
 *
 * This class is used to request the messages from the database
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Web;

    // App Namespaces
    using Utils.General;

    /// <summary>
    /// Message List Dto
    /// </summary>
    public class MessagesListDto {

        /// <summary>
        /// Secret container
        /// </summary>
        public string? _secret;

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
        /// Page field
        /// </summary>
        public int Page { get; set; }        

    }

}