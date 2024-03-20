/*
 * @class Plans Meta Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to create the plans meta
 */

// Namespace for Plans Meta Repositories
namespace FeChat.Models.Repositories.Plans.Meta {

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;

    // Use the Plans Entities
    using FeChat.Models.Entities.Plans;

    // Use the Configuration for database connection
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Plans Meta Create Repository
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
        /// Plans Meta Create Repository Constructor
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
        /// Save bulk plans meta
        /// </summary>
        /// <param name="metaEntities">Meta list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SavePlanMetaAsync(List<PlansMetaEntity> metaEntities) {

            try {

                // Add range with meta
                await _context.PlansMeta.AddRangeAsync(metaEntities);

                // Save the meta
                int save = _context.SaveChanges();

                // Create the cache key
                string cacheKey = "fc_plan_meta_" + metaEntities.First().PlanId;

                // Delete the cache
                _memoryCache.Remove(cacheKey);

                return save > 0;

            } catch (InvalidOperationException) {

                return false;

            }

        }

    }

}