/*
 * @class Subscriptions Update Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-16
 *
 * This class is used to update the subscriptions
 */

// Namespace for Subscriptions Repositories
namespace FeChat.Models.Repositories.Subscriptions.Subscriptions {

    // Use Memory catching
    using Microsoft.Extensions.Caching.Memory;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Subscriptions Dtos
    using FeChat.Models.Dtos.Subscriptions;

    // Use Subscriptions Entities
    using FeChat.Models.Entities.Subscriptions;

    // Use General Utils to access the strings
    using FeChat.Utils.General;

    // Use the Configuration Utils
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Subscriptions Update Repository
    /// </summary>
    public class UpdateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Subscriptions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Subscriptions Update Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public UpdateRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Update a subscription
        /// </summary>
        /// <param name="subscriptionDto">Subscription information</param>
        /// <returns>Bool and error message</returns>
        public async Task<ResponseDto<bool>> UpdateSubscriptionAsync(SubscriptionDto subscriptionDto) {

            try {

                // Create entity for update
                SubscriptionEntity subscriptionEntity = new() {
                    SubscriptionId = subscriptionDto.SubscriptionId,
                    MemberId = subscriptionDto.MemberId,
                    PlanId = subscriptionDto.PlanId,
                    OrderId = subscriptionDto.OrderId,
                    NetId = subscriptionDto.NetId,
                    Source = subscriptionDto.Source,
                    Expiration = subscriptionDto.Expiration,
                    Created = subscriptionDto.Created
                };

                // Update the subscription
                _context.Subscriptions.Update(subscriptionEntity);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Verify if the changes were saved
                if ( saveChanges > 0 ) {

                    // Cache key for subscription
                    string cacheKey = "fc_subscription_" + subscriptionDto.MemberId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);

                    // Return success response
                    return new ResponseDto<bool> {
                        Result = true,
                        Message = new Strings().Get("SubscriptionUpdated")
                    };

                } else {

                    // Return error response
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = new Strings().Get("SubscriptionNotUpdated")
                    };

                }

            } catch ( Exception ex ) {

                return new ResponseDto<bool> {
                    Result = false,
                    Message = ex.Message
                };

            }

        }

    }

}