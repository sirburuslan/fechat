/*
 * @class Transaction Details Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-10
 *
 * This class is used to transfer transaction details withe member and plan's data
 */

// Namespace for Transactions dtos
namespace FeChat.Models.Dtos.Transactions {

    /// <summary>
    /// New Transaction Dto
    /// </summary>
    public class TransactionDetailsDto {

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

        /// <summary>
        /// Plan Name field
        /// </summary>
        public string? PlanName { get; set; }  
        
        /// <summary>
        /// Plan Price field
        /// </summary>
        public string? PlanPrice { get; set; }   
        
        /// <summary>
        /// Plan Currency field
        /// </summary>
        public string? PlanCurrency { get; set; }
        
        /// <summary>
        /// Profile Photo field
        /// </summary>
        public string? ProfilePhoto { get; set; }  

        /// <summary>
        /// First Name field
        /// </summary>
        public string? FirstName { get; set; }  

        /// <summary>
        /// Last Name field
        /// </summary>
        public string? LastName { get; set; }                              

    }

}