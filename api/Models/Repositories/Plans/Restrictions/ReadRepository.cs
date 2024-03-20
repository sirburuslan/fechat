/*
 * @class Plans Restrictions Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the plans restrictions
 */

// Namespace for Plans Restrictions Repositories
namespace FeChat.Models.Repositories.Plans.Restrictions {

    // Use the Entity Framework
    using Microsoft.EntityFrameworkCore;

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;
    
    // Use General Dtos classes
    using FeChat.Models.Dtos;

    // Use the Plans Dtos classes
    using FeChat.Models.Dtos.Plans;

    // Use the Configuration for database connection
    using FeChat.Utils.Configuration;
    
    // Use the General Utils
    using FeChat.Utils.General;

    /// <summary>
    /// Plans Restrictions Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Plans Restrictions Read Repository Constructor
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
        /// Get the plan restrictions
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>The restrictions list or null</returns>
        public async Task<ResponseDto<List<RestrictionDto>>> RestrictionsListAsync(int planId) {

            try {

                // Create the cache key for settings
                string cacheKey = "fc_plan_restrictions_" + planId;

                // Verify if the restrictions are saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out List<RestrictionDto>? restrictionsList) ) {

                    // Request the restrictions
                    restrictionsList = await _context.PlansRestrictions
                    .Select(r => new RestrictionDto {
                        RestrictionId = r.RestrictionId,
                        PlanId = r.PlanId,
                        RestrictionName = r.RestrictionName,
                        RestrictionValue = r.RestrictionValue
                    })
                    .Where(r => r.PlanId == planId)
                    .ToListAsync();

                    // Create the cache restrictions for storing
                    MemoryCacheEntryOptions cacheRestrictions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, restrictionsList, cacheRestrictions);

                }

                // Verify if restrictions exists
                if ( (restrictionsList != null) && (restrictionsList.Count > 0) ) {

                    // Return the restrictions
                    return new ResponseDto<List<RestrictionDto>> {
                        Result = restrictionsList,
                        Message = null
                    };

                } else {

                    // Return the missing restrictions message
                    return new ResponseDto<List<RestrictionDto>> {
                        Result = null,
                        Message = new Strings().Get("NoRestrictionsFound")
                    };                    

                }

            } catch ( InvalidOperationException e ) {

                // Return the error message
                return new ResponseDto<List<RestrictionDto>> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }

}