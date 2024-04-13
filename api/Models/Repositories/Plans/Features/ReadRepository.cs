/*
 * @class Plans Features Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the plans features
 */

// Namespace for Plans Features Repositories
namespace FeChat.Models.Repositories.Plans.Features {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Plans;
    using Utils.General;
    using Utils.Configuration;

    /// <summary>
    /// Plans Features Read Repository
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
        /// Plans Features Read Repository Constructor
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
        /// Get the plan features
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>The features list or null</returns>
        public async Task<ResponseDto<List<FeatureDto>>> FeaturesListAsync(int planId) {

            try {

                // Create the cache key for features
                string cacheKey = "fc_plan_features_" + planId;

                // Verify if the features are saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out List<FeatureDto>? featuresList) ) {

                    // Request the features
                    featuresList = await _context.PlansFeatures
                    .Select(f => new FeatureDto {
                        FeatureId = f.FeatureId,
                        PlanId = f.PlanId,
                        FeatureText = f.FeatureText
                    })
                    .Where(f => f.PlanId == planId)
                    .ToListAsync();

                    // Create the cache features for storing
                    MemoryCacheEntryOptions cacheFeatures = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, featuresList, cacheFeatures);

                }

                // Verify if features exists
                if ( (featuresList != null) && (featuresList.Count > 0) ) {

                    // Return the features
                    return new ResponseDto<List<FeatureDto>> {
                        Result = featuresList,
                        Message = null
                    };

                } else {

                    // Return the missing features message
                    return new ResponseDto<List<FeatureDto>> {
                        Result = null,
                        Message = new Strings().Get("NoFeaturesFound")
                    };                    

                }

            } catch ( InvalidOperationException e ) {

                // Return the error message
                return new ResponseDto<List<FeatureDto>> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }

}