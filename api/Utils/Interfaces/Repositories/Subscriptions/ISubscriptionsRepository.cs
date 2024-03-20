/*
 * @interface Subscriptions Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-10
 *
 * This interface is implemented in SubscriptionsRepository
 */

// Namespace for Subscriptions Interfaces
namespace FeChat.Utils.Interfaces.Repositories.Subscriptions {

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Subscriptions Dtos
    using FeChat.Models.Dtos.Subscriptions;

    // Use Transactions Dtos
    using FeChat.Models.Dtos.Transactions;

    /// <summary>
    /// Repository for Subscriptions
    /// </summary>
    public interface ISubscriptionsRepository {

        /// <summary>
        /// Create a subscription
        /// </summary>
        /// <param name="subscriptionDto">Subscription information</param>
        /// <returns>Subscription id or error message</returns>
        Task<ResponseDto<SubscriptionDto>> CreateSubscriptionAsync(SubscriptionDto subscriptionDto);

        /// <summary>
        /// Create a transaction
        /// </summary>
        /// <param name="transactionDto">Transaction information</param>
        /// <returns>Transaction id or error message</returns>
        Task<ResponseDto<TransactionDto>> CreateTransactionAsync(TransactionDto transactionDto);

        /// <summary>
        /// Update a subscription
        /// </summary>
        /// <param name="subscriptionDto">Subscription information</param>
        /// <returns>Bool and error message</returns>
        Task<ResponseDto<bool>> UpdateSubscriptionAsync(SubscriptionDto subscriptionDto);

        /// <summary>
        /// Get a subscription for a user
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>SubscriptionDto with subscription's data</returns>
        Task<ResponseDto<SubscriptionDto>> GetSubscriptionByMemberIdAsync(int memberId);

        /// <summary>
        /// Get subscriptions by plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>SubscriptionDto with subscription's data</returns>
        Task<ResponseDto<List<SubscriptionDto>>> GetSubscriptionsByPlanIdAsync(int planId);

        /// <summary>
        /// Get subscriptions by net id
        /// </summary>
        /// <param name="netId">Net ID</param>
        /// <returns>Subscription data or error message</returns>
        Task<ResponseDto<SubscriptionDto>> GetSubscriptionByNetIdAsync(string netId);

        /// <summary>
        /// Delete member's subscriptions
        /// </summary>
        /// <param name="memberId">Member ID</param>
        Task DeleteMemberSubscriptionsAsync(int memberId);

        /// <summary>
        /// Gets transactions by page
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="planId">Plan Id</param>
        /// <returns>List with transactions</returns>
        Task<ResponseDto<ElementsDto<TransactionDetailsDto>>> GetTransactionsByPageAsync(SearchDto searchDto, int? planId);

        /// <summary>
        /// Gets transactions by id
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction details or error message</returns>
        Task<ResponseDto<TransactionDetailsDto>> GetTransactionAsync(int transactionId);

        /// <summary>
        /// Delete the Member transactions
        /// </summary>
        /// <param name="memberId">Member ID</param>
        Task DeleteMemberTransactionsAsync(int memberId);

    }

}