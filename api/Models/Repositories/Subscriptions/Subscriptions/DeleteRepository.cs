/*
 * @class Subscriptions Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to delete the subscriptions
 */

// Namespace for Subscriptions Repositories
namespace FeChat.Models.Repositories.Subscriptions.Subscriptions {

    // Use the Entity Framework Core to extend the Linq features
    using Microsoft.EntityFrameworkCore;

    // Use Memory catching
    using Microsoft.Extensions.Caching.Memory;

    // Use Subscriptions Entities
    using FeChat.Models.Entities.Subscriptions;

    // Use the Configuration Utils
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Subscriptions Delete Repository
    /// </summary>
    public class DeleteRepository {

        /// <summary>
        /// Subscriptions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Subscriptions Delete Repository Constructor
        /// </summary>
        /// <param name="db">Db connection instance</param>
        public DeleteRepository(Db db) {

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Delete member subscriptions
        /// </summary>
        /// <param name="memberId">Member ID</param>
        public async Task DeleteMemberSubscriptionsAsync(int memberId) {

            try {

                // Get all subscriptions by member ID
                List<SubscriptionEntity>? subscriptionsList = await _context.Subscriptions.Where(s => s.MemberId == memberId).ToListAsync();

                // Verify if subscriptions exists
                if ( (subscriptionsList != null) && (subscriptionsList.Count > 0) ) {

                    // Delete subscriptions
                    _context.Subscriptions.RemoveRange(subscriptionsList);

                    // Save changes
                    await _context.SaveChangesAsync();

                    // Extract SubscriptionIds from subscriptionsList
                    var subscriptionIds = subscriptionsList.Select(s => s.SubscriptionId).ToList();

                    // Get meta by subscriptions
                    List<SubscriptionsMetaEntity>? subscriptionsMetaList = await _context.SubscriptionsMeta.Where(m => subscriptionIds.Contains(m.SubscriptionId)).ToListAsync();

                    // Verify if meta exists
                    if ( (subscriptionsMetaList != null) && (subscriptionsMetaList.Count > 0) ) {

                        // Delete subscriptions meta
                        _context.SubscriptionsMeta.RemoveRange(subscriptionsMetaList);

                        // Save changes
                        await _context.SaveChangesAsync();

                    }

                }

            } catch ( Exception ex ) {

                // Show error message in console
                Console.WriteLine("SubscriptionDeletionError: " + ex.Message);

            }

        }

    }

}