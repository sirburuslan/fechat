/*
 * @class Plans Restrictions Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the plans restrictions
 */

// Namespace for Plans Restrictions Repositories
namespace FeChat.Models.Repositories.Plans.Restrictions {

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;

    // Use the Plans Entities
    using FeChat.Models.Entities.Plans;

    // Use the Configuration for database connection
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Plans Restrictions Create Repository
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Plans Restrictions Create Repository Constructor
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
        /// Save bulk restrictions
        /// </summary>
        /// <param name="restrictions">Plans restrictions list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SaveRestrictionsAsync(List<PlansRestrictionsEntity> restrictions) {

            try {

                // Add range with restrictions
                await _context.PlansRestrictions.AddRangeAsync(restrictions);

                // Save the restrictions
                int save = await _context.SaveChangesAsync();

                // Create the cache key
                string cacheKey = "fc_plan_restrictions_" + restrictions.First().PlanId;

                // Delete the cache
                _memoryCache.Remove(cacheKey);

                return save > 0;

            } catch (InvalidOperationException) {

                return false;

            }

        }

    }

}