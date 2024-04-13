/*
 * @class Event Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This class is used build the Event entity
 */

// Namespace for Events Entities
namespace FeChat.Models.Entities.Events {

    // System Namespaces
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Event Entity
    /// </summary>
    public class EventEntity {

        /// <summary>
        /// Event Id Key
        /// </summary>
        [Key]
        [Required]
        public int EventId { get; set; }

        /// <summary>
        /// Member ID
        /// </summary>
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Type ID
        /// </summary>
        [Required]
        public int TypeId { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; } 

    }

}