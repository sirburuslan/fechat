/*
 * @class Subscriptions Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the subscriptions
 */

// Namespace for Subscriptions Repositories
namespace FeChat.Models.Repositories.Subscriptions.Subscriptions {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Subscriptions;
    using Utils.General;
    using Utils.Configuration;

    /// <summary>
    /// Subscriptions Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Subscriptions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Subscriptions Read Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public ReadRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Get a subscription for a user
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>SubscriptionDto with subscription's data</returns>
        public async Task<ResponseDto<SubscriptionDto>> GetSubscriptionByMemberIdAsync(int memberId) {

            try {

                // Cache key for subscription
                string cacheKey = "fc_subscription_member_" + memberId;

                // Verify if the subscription is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out SubscriptionDto? subscriptionDto ) ) {

                    // Get the subscription by id
                    subscriptionDto = await _context.Subscriptions
                    .Select(s => new SubscriptionDto {
                        SubscriptionId = s.SubscriptionId,
                        MemberId = s.MemberId,
                        PlanId = s.PlanId,
                        OrderId = s.OrderId,
                        NetId = s.NetId,
                        Source = s.Source,
                        Expiration = s.Expiration,
                        Created = s.Created
                    })
                    .FirstAsync(s => s.MemberId == memberId); 

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, subscriptionDto, cacheOptions);

                }

                // Verify if subscription exists
                if ( subscriptionDto != null ) {

                    // Return the subscription data
                    return new ResponseDto<SubscriptionDto> {
                        Result = subscriptionDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<SubscriptionDto> {
                        Result = null,
                        Message = new Strings().Get("SubscriptionNotFound")
                    };
                    
                }

            } catch ( Exception ex ) {

                // Return the error message
                return new ResponseDto<SubscriptionDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

        /// <summary>
        /// Get subscriptions by plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>List with subscriptions or error message</returns>
        public async Task<ResponseDto<List<SubscriptionDto>>> GetSubscriptionsByPlanIdAsync(int planId) {

            try {

                // Cache key for subscription
                string cacheKey = "fc_subscription_plan_" + planId;

                // Verify if the subscription is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out List<SubscriptionDto>? subscriptionsDto ) ) {

                    // Get the subscriptions by plan's id
                    subscriptionsDto = await _context.Subscriptions
                    .Select(s => new SubscriptionDto {
                        SubscriptionId = s.SubscriptionId,
                        MemberId = s.MemberId,
                        PlanId = s.PlanId,
                        OrderId = s.OrderId,
                        NetId = s.NetId,
                        Source = s.Source,
                        Expiration = s.Expiration,
                        Created = s.Created
                    })
                    .Where(s => s.PlanId == planId)
                    .ToListAsync(); 

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, subscriptionsDto, cacheOptions);

                }

                // Verify if subscriptions exists
                if ( (subscriptionsDto != null) && (subscriptionsDto.Count > 0) ) {

                    // Return the subscription data
                    return new ResponseDto<List<SubscriptionDto>> {
                        Result = subscriptionsDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<List<SubscriptionDto>> {
                        Result = null,
                        Message = new Strings().Get("NoSubscriptionsFound")
                    };
                    
                }

            } catch ( Exception ex ) {

                // Return the error message
                return new ResponseDto<List<SubscriptionDto>> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

        /// <summary>
        /// Get subscription by net id
        /// </summary>
        /// <param name="netId">Net ID</param>
        /// <returns>List with subscriptions or error message</returns>
        public async Task<ResponseDto<SubscriptionDto>> GetSubscriptionByNetIdAsync(string netId) {

            try {

                // Cache key for subscription
                string cacheKey = "fc_subscription_net_" + netId;

                // Verify if the subscription is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out SubscriptionDto? subscriptionDto ) ) {

                    // Get the subscriptions by plan's id
                    subscriptionDto = await _context.Subscriptions
                    .Select(s => new SubscriptionDto {
                        SubscriptionId = s.SubscriptionId,
                        MemberId = s.MemberId,
                        PlanId = s.PlanId,
                        OrderId = s.OrderId,
                        NetId = s.NetId,
                        Source = s.Source,
                        Expiration = s.Expiration,
                        Created = s.Created
                    })
                    .FirstAsync(s => s.NetId == netId); 

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, subscriptionDto, cacheOptions);

                }

                // Verify if subscriptions exists
                if ( subscriptionDto != null ) {

                    // Return the subscription data
                    return new ResponseDto<SubscriptionDto> {
                        Result = subscriptionDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<SubscriptionDto> {
                        Result = null,
                        Message = new Strings().Get("SubscriptionNotFound")
                    };
                    
                }

            } catch ( Exception ex ) {

                // Return the error message
                return new ResponseDto<SubscriptionDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }

}