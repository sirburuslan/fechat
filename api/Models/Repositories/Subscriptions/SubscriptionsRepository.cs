/*
 * @class Subscriptions Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-09
 *
 * This class is used manage the subscriptions
 */

// Namespace for Subscriptions Repositories
namespace FeChat.Models.Repositories.Subscriptions {

    // Use Memory catching
    using Microsoft.Extensions.Caching.Memory;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Subscriptions Dtos
    using FeChat.Models.Dtos.Subscriptions;

    // Use Transactions Dtos
    using FeChat.Models.Dtos.Transactions;

    // Use the Configuration Utils
    using FeChat.Utils.Configuration;

    // Use the interfaces for Subscriptions Repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Subscriptions Repository
    /// </summary>
    public class SubscriptionsRepository: ISubscriptionsRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Transactions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Entity Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public SubscriptionsRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Create a subscription
        /// </summary>
        /// <param name="subscriptionDto">Subscription information</param>
        /// <returns>Subscription id or error message</returns>
        public async Task<ResponseDto<SubscriptionDto>> CreateSubscriptionAsync(SubscriptionDto subscriptionDto) {

            // Init Create Repository
            Subscriptions.CreateRepository createRepository = new(_memoryCache, _context);

            // Create a subscription and return the response
            return await createRepository.CreateSubscriptionAsync(subscriptionDto);

        }

        /// <summary>
        /// Create a transaction
        /// </summary>
        /// <param name="transactionDto">Transaction information</param>
        /// <returns>Transaction id or error message</returns>
        public async Task<ResponseDto<TransactionDto>> CreateTransactionAsync(TransactionDto transactionDto) {

            // Init Create Repository
            Transactions.CreateRepository createRepository = new(_memoryCache, _context);

            // Create a transaction and return the response
            return await createRepository.CreateTransactionAsync(transactionDto);

        }

        /// <summary>
        /// Update a subscription
        /// </summary>
        /// <param name="subscriptionDto">Subscription information</param>
        /// <returns>Bool and error message</returns>
        public async Task<ResponseDto<bool>> UpdateSubscriptionAsync(SubscriptionDto subscriptionDto) {

            // Init Update Repository
            Subscriptions.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update a subscription and return the response
            return await updateRepository.UpdateSubscriptionAsync(subscriptionDto);

        }

        /// <summary>
        /// Get a subscription for a user
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>SubscriptionDto with subscription's data</returns>
        public async Task<ResponseDto<SubscriptionDto>> GetSubscriptionByMemberIdAsync(int memberId) {

            // Init Read Repository
            Subscriptions.ReadRepository readRepository = new(_memoryCache, _context);

            // Read a subscription and return the response
            return await readRepository.GetSubscriptionByMemberIdAsync(memberId);

        }

        /// <summary>
        /// Get subscriptions by plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>List with subscriptions or error message</returns>
        public async Task<ResponseDto<List<SubscriptionDto>>> GetSubscriptionsByPlanIdAsync(int planId) {

            // Init Read Repository
            Subscriptions.ReadRepository readRepository = new(_memoryCache, _context);

            // Read a subscription by plan id and return the response
            return await readRepository.GetSubscriptionsByPlanIdAsync(planId);

        }

        /// <summary>
        /// Get subscriptions by net id
        /// </summary>
        /// <param name="netId">Net ID</param>
        /// <returns>Subscription data or error message</returns>
        public async Task<ResponseDto<SubscriptionDto>> GetSubscriptionByNetIdAsync(string netId) {

            // Init Read Repository
            Subscriptions.ReadRepository readRepository = new(_memoryCache, _context);

            // Read a subscription by net id and return the response
            return await readRepository.GetSubscriptionByNetIdAsync(netId);

        }

        /// <summary>
        /// Delete member's subscriptions
        /// </summary>
        /// <param name="memberId">Member ID</param>
        public async Task DeleteMemberSubscriptionsAsync(int memberId) {

            // Init Delete Repository
            Subscriptions.DeleteRepository deleteRepository = new(_context);

            // Delete a subscriptions by member id
            await deleteRepository.DeleteMemberSubscriptionsAsync(memberId);

        }

        /// <summary>
        /// Gets transactions by page
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="planId">Plan Id</param>
        /// <returns>List with transactions</returns>
        public async Task<ResponseDto<ElementsDto<TransactionDetailsDto>>> GetTransactionsByPageAsync(SearchDto searchDto, int? planId) {

            // Init Read Repository
            Transactions.ReadRepository readRepository = new(_memoryCache, _context);

            // Read a transaction and return the response
            return await readRepository.GetTransactionsByPageAsync(searchDto, planId);

        }

        /// <summary>
        /// Gets transactions by id
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction details or error message</returns>
        public async Task<ResponseDto<TransactionDetailsDto>> GetTransactionAsync(int transactionId) {

            // Init Read Repository
            Transactions.ReadRepository readRepository = new(_memoryCache, _context);

            // Read a transaction by id and return the response
            return await readRepository.GetTransactionAsync(transactionId);

        }

        /// <summary>
        /// Delete the Member transactions
        /// </summary>
        /// <param name="memberId">Member ID</param>
        public async Task DeleteMemberTransactionsAsync(int memberId) {

            // Init Delete Repository
            Transactions.DeleteRepository deleteRepository = new(_memoryCache, _context);

            // Delete transactions by member id
            await deleteRepository.DeleteMemberTransactionsAsync(memberId);
            
        }

    }

}