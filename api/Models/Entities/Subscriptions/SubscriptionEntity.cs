/*
 * @class Subscription Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used build the Subscription entity
 */

// Namespace for Subscription entities
namespace FeChat.Models.Entities.Subscriptions {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Subscription Entity
    /// </summary>
    public class SubscriptionEntity {

        /// <summary>
        /// Subscription ID
        /// </summary>
        [Key]
        [Required]
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Member ID
        /// </summary>
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        [Required]
        public int PlanId { get; set; }

        /// <summary>
        /// Order ID
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string? OrderId { get; set; }

        /// <summary>
        /// Subscription ID returned from gateway
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string? NetId { get; set; }      

        /// <summary>
        /// Gateway Source
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string? Source { get; set; }

        /// <summary>
        /// Time When Expires the subscription
        /// </summary>
        [Required]
        public int Expiration { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }  

    }

}