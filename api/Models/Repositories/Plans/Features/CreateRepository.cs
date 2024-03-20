/*
 * @class Plans Features Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the plans features
 */

// Namespace for Plans Features Repositories
namespace FeChat.Models.Repositories.Plans.Features {

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;
    
    // Use the General Dtos classes
    using FeChat.Models.Dtos;

    // Use the Plans Entities
    using FeChat.Models.Entities.Plans;

    // Use the Configuration for database connection
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Plans Features Create Repository
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
        /// Plans Features Create Repository Constructor
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
        /// Save bulk features
        /// </summary>
        /// <param name="features">Plan features list</param>
        /// <returns>Boolean response or error message</returns>
        public async Task<ResponseDto<bool>> SaveFeaturesAsync(List<PlansFeaturesEntity> features) {

            try {

                // Add range with features
                await _context.PlansFeatures.AddRangeAsync(features);

                // Save the features
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {

                    // Create the cache key
                    string cacheKey = "fc_plan_features_" + features.First().PlanId;

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