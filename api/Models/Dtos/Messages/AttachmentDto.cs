/*
 * @class Attachment Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-29
 *
 * This class is used for attachments transport
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    /// <summary>
    /// Attachment Dto
    /// </summary>
    public class AttachmentDto {

        /// <summary>
        /// Attachment ID field
        /// </summary>
        public int AttachmentId { get; set; }

        /// <summary>
        /// Message ID field
        /// </summary>
        public int MessageId { get; set; }    

        /// <summary>
        /// Link field
        /// </summary>
        public string? Link { get; set; }

        /// <summary>
        /// Created field
        /// </summary>
        public int Created { get; set; }     

    }

}