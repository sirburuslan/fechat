/*
 * @class Transaction Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-10
 *
 * This class is used to transfer transaction data
 */

// Namespace for Transactions dtos
namespace FeChat.Models.Dtos.Transactions {

    /// <summary>
    /// New Transaction Dto
    /// </summary>
    public class TransactionDto {

        /// <summary>
        /// Transaction ID
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Member ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Subscription ID
        /// </summary>
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Order ID
        /// </summary>
        public string? OrderId { get; set; }

        /// <summary>
        /// Transaction ID returned from gateway
        /// </summary>
        public string? NetId { get; set; }       

        /// <summary>
        /// Gateway Source
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        public int Created { get; set; } 

    }

}