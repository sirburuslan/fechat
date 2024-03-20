/*
 * @class Subscriptions Meta Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used build the Subscriptions meta entity for extra data
 */

// Namespace for Subscriptions Entities
namespace FeChat.Models.Entities.Subscriptions {

    // Use the net annotations for fields information
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Subscription Meta Entity
    /// </summary>
    public class SubscriptionsMetaEntity {
        
        /// <summary>
        /// Meta ID
        /// </summary>
        [Key]
        [Required]
        public int MetaId { get; set; }

        /// <summary>
        /// Subscription ID
        /// </summary>
        [Required]
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Meta Name
        /// </summary>
        [Required]
        [MaxLength(250)]
        public required string MetaName { get; set; }

        /// <summary>
        /// Meta Value
        /// </summary>
        public string? MetaValue { get; set; }

    }

}