/*
 * @class Subscription Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to transfer subscription data
 */

// Namespace for Subscriptions dtos
namespace FeChat.Models.Dtos.Subscriptions {

    /// <summary>
    /// New Subscription Dto
    /// </summary>
    public class SubscriptionDto {

        /// <summary>
        /// Subscription ID
        /// </summary>
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Member ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Order ID
        /// </summary>
        public string? OrderId { get; set; }

        /// <summary>
        /// Subscription ID returned from gateway
        /// </summary>
        public string? NetId { get; set; }    

        /// <summary>
        /// Gateway Source
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Time When Expires the subscription
        /// </summary>
        public int Expiration { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        public int Created { get; set; } 

    }

}