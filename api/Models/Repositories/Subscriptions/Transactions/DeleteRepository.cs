/*
 * @class Subscriptions Transactions Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to delete the transactions
 */

// Namespace for Subscriptions Transactions Repositories
namespace FeChat.Models.Repositories.Subscriptions.Transactions {

    // Use the Entity Framework Core to extend the Linq features
    using Microsoft.EntityFrameworkCore;

    // Use Memory catching
    using Microsoft.Extensions.Caching.Memory;

    // Use Transactions Entities
    using FeChat.Models.Entities.Transactions;

    // Use the Configuration Utils
    using FeChat.Utils.Configuration;

    // Import the General Utils for Cache class
    using FeChat.Utils.General;

    /// <summary>
    /// Transactions Delete Repository
    /// </summary>
    public class DeleteRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Transactions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Transactions Delete Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public DeleteRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Delete the Member transactions
        /// </summary>
        /// <param name="memberId">Member ID</param>
        public async Task DeleteMemberTransactionsAsync(int memberId) {

            try {

                // Get all transactions by member id
                List<TransactionEntity>? transactionsList = await _context.Transactions.Where(t => t.MemberId == memberId).ToListAsync();

                // Verify if transactions exists
                if ( (transactionsList != null) && (transactionsList.Count > 0) ) {

                    // Remove the transactions
                    _context.Transactions.RemoveRange(transactionsList);

                    // Save Changes
                    int saveChanges = await _context.SaveChangesAsync();

                    // Verify if the changes were saved
                    if ( saveChanges > 0 ) {

                        // Remove the cache key in the group
                        new Cache(_memoryCache).Remove("transactions");

                    }

                }

            } catch ( Exception ex ) {

                // Display error message in console
                Console.WriteLine("TransactionDeletionError: " + ex.Message);

            }

        }

    }

}