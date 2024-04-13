/*
 * @class Message Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-26
 *
 * This class is used build the Message entity
 */

// Namespace for Messages Entities
namespace FeChat.Models.Entities.Messages {

    // System Namespaces
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Message Entity
    /// </summary>
    public class MessageEntity {

        /// <summary>
        /// Message's ID
        /// </summary>
        [Key]
        [Required]
        public int MessageId { get; set; }

        /// <summary>
        /// Thread ID
        /// </summary>
        [Required]
        public int ThreadId { get; set; }

        /// <summary>
        /// Member Id
        /// </summary>
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Message Body
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(2000)]
        public string? Message { get; set; }

        /// <summary>
        /// Seen status
        /// </summary>
        [Required]
        public int Seen { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }   

    }

}