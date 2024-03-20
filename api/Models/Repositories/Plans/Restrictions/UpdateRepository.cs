/*
 * @class Plans Restrictions Update Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the plans restrictions
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
    /// Plans Restrictions Update Repository
    /// </summary>
    public class UpdateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Plans Restrictions Update Repository Constructor
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
        /// Update bulk restrictions
        /// </summary>
        /// <param name="restrictions">Settings restrictions list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> UpdateRestrictionsAsync(List<PlansRestrictionsEntity> restrictions) {

            try {

                // Update the entities in the database
                _context.PlansRestrictions.UpdateRange(restrictions); // Right now UpdateRangeAsync is not available

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