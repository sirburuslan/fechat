/*
 * @class Attachment Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-26
 *
 * This class is used build the Attachment entity
 */

// Namespace for Messages Entities
namespace FeChat.Models.Entities.Messages {

    // Use the Adnnotations for attributes
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Attachment Entity
    /// </summary>
    public class AttachmentEntity {

        /// <summary>
        /// Attachment's ID
        /// </summary>
        [Key]
        [Required]
        public int AttachmentId { get; set; }

        /// <summary>
        /// Message ID
        /// </summary>
        [Required]
        public int MessageId { get; set; }

        /// <summary>
        /// Attachment link
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(500)]
        public string? Link { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }   

    }

}