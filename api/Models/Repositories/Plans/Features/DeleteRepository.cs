/*
 * @class Plans Features Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the plans features
 */

// Namespace for Plans Features Repositories
namespace FeChat.Models.Repositories.Plans.Features {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;
    
    // App Namespaces
    using Models.Dtos;
    using Utils.Configuration;

    /// <summary>
    /// Plans Features Delete Repository
    /// </summary>
    public class DeleteRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Plans Features Delete Repository Constructor
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
        /// Delete the plan features
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>Bool or error message</returns>
        public async Task<ResponseDto<bool>> FeaturesDeleteAsync(int planId) {

            try {

                // Select the plan by id
                var plan = _context.PlansFeatures.Where(m => m.PlanId == planId);

                // Delete the plan
                _context.PlansFeatures.RemoveRange(plan);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {

                    // Create the cache key
                    string cacheKey = "fc_plan_features_" + planId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);

                    // Return the success message
                    return new ResponseDto<bool> {
                        Result = true,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
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