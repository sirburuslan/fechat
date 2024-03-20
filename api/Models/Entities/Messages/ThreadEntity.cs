/*
 * @class Thread Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-26
 *
 * This class is used build the Thread entity
 */

// Namespace for Messages Entities
namespace FeChat.Models.Entities.Messages {

    // Use the Adnnotations for attributes
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Thread Entity
    /// </summary>
    public class ThreadEntity {

        /// <summary>
        /// Thread's ID
        /// </summary>
        [Key]
        [Required]
        public int ThreadId { get; set; }

        /// <summary>
        /// Thread Secret
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public required string ThreadSecret { get; set; }

        /// <summary>
        /// Member Id
        /// </summary>
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Website Id
        /// </summary>
        [Required]
        public int WebsiteId { get; set; }

        /// <summary>
        /// Guest ID
        /// </summary>
        [Required]
        public int GuestId { get; set; }        

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }
        
    }

}