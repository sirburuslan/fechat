/*
 * @class Subscriptions Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the subscriptions
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
    /// Subscriptions Create Repository
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Subscriptions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Subscriptions Create Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public CreateRepository(IMemoryCache memoryCache, Db db) {

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

            try {

                // Create the entity with subscription data
                SubscriptionEntity subscriptionEntity = new() {
                    MemberId = subscriptionDto.MemberId,
                    PlanId = subscriptionDto.PlanId,
                    OrderId = subscriptionDto.OrderId,
                    NetId = subscriptionDto.NetId,
                    Source = subscriptionDto.Source,
                    Expiration = subscriptionDto.Expiration,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add entity to the Subscriptions list
                _context.Subscriptions.Add(subscriptionEntity);

                // Save the changes
                int saveChanges = await _context.SaveChangesAsync();

                // Verify if the entity was saved
                if ( saveChanges > 0 ) {

                    // Return success response
                    return new ResponseDto<SubscriptionDto> {
                        Result = new SubscriptionDto {
                            SubscriptionId = subscriptionEntity.SubscriptionId
                        },
                        Message = new Strings().Get("SubscriptionCreated")
                    };

                } else {

                    // Return error response
                    return new ResponseDto<SubscriptionDto> {
                        Result = null,
                        Message = new Strings().Get("SubscriptionNotCreated")
                    };

                }

            } catch ( Exception ex ) {

                // Return error message
                return new ResponseDto<SubscriptionDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }

}