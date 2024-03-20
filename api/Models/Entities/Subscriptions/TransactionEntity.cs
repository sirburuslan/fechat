/*
 * @class Transaction Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-10
 *
 * This class is used build the Transaction entity
 */

// Namespace for Transaction entities
namespace FeChat.Models.Entities.Transactions {

    // Use the DataAnnotations
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Transaction Entity
    /// </summary>
    public class TransactionEntity {

        /// <summary>
        /// Transaction ID
        /// </summary>
        [Key]
        [Required]
        public int TransactionId { get; set; }

        /// <summary>
        /// Member ID
        /// </summary>
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Subscription ID
        /// </summary>
        [Required]
        public int SubscriptionId { get; set; }

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
        /// Transaction ID returned from gateway
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
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }  

    }

}