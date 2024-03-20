/*
 * @class Typing Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This class is used build the Typing entity which monitors when the members are typing
 */

// Namespace for Messages Entities
namespace FeChat.Models.Entities.Messages {

    // Use the Adnnotations for attributes
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Typing Entity
    /// </summary>
    public class TypingEntity {

        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

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
        /// Updated time field
        /// </summary>
        [Required]
        public int Updated { get; set; }
        
    }

}