/*
 * @class Unseen Message Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-25
 *
 * This class is used to transfer unseen messages
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    /// <summary>
    /// Unseen Message Dto
    /// </summary>
    public class UnseenMessageDto {

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
        public string? ThreadSecret { get; set; }       

        /// <summary>
        /// Message field
        /// </summary>
        public string? Message { get; set; }

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

        /// <summary>
        /// Member's id field
        /// </summary>
        public int ThreadOwner { get; set; }        

        /// <summary>
        /// Member's first name field
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Member's last name field
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Member's email field
        /// </summary>
        public string? Email { get; set; }

    }

}