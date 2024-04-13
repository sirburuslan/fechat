/*
 * @class Plans Update Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the plans
 */

// Namespace for Plans Repositories
namespace FeChat.Models.Repositories.Plans.Plans {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Entities.Plans;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Plans Repository
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
        /// Plans Update Repository Constructor
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
        /// Update a plan
        /// </summary>
        /// <param name="planEntity">Plan entity with the plan's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdatePlanAsync(PlanEntity planEntity) {

            try {

                // Update the entities in the database
                _context.Plans.Update(planEntity);

                // Save the changes
                int save = await _context.SaveChangesAsync();

                // Check if the plan's data was updated
                if ( save > 0 ) {

                    // Create the cache key
                    string cacheKey = "fc_plan_" + planEntity.PlanId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);  

                    // Remove the caches for plans group
                    new Cache(_memoryCache).Remove("plans");                  

                    // Return error response
                    return new ResponseDto<bool> {
                        Result = true,
                        Message = null
                    };

                } else {

                    // Return error response
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
                    };

                }

            } catch (InvalidOperationException e) {

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };                

            }

        }

    }

}