/*
 * @class Plans Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the plans
 */

// Namespace for Plans Repositories
namespace FeChat.Models.Repositories.Plans.Plans {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;
    
    // App Namespaces
    using Models.Dtos;
    using Utils.Configuration;
    using Utils.General;
    using Models.Entities.Plans;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Plans Repository
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
        /// Plans Delete Repository Constructor
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
        /// Delete plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>Bool true if the plan was deleted</returns>
        public async Task<ResponseDto<bool>> DeletePlanAsync(int planId) {

            try {

                // Select the plan by id
                var plan = _context.Plans.Where(m => m.PlanId == planId);

                // Delete the plan
                _context.Plans.RemoveRange(plan);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {

                    // Get the plan's features
                    List<PlansFeaturesEntity>? plansFeaturesList = await _context.PlansFeatures.Where(f => f.PlanId == planId).ToListAsync();

                    // Verify if the plan has features
                    if ( (plansFeaturesList != null) && (plansFeaturesList.Count > 0) ) {

                        // Remove the features
                        _context.PlansFeatures.RemoveRange(plansFeaturesList);

                        // Save the changes
                        await _context.SaveChangesAsync();

                    }

                    // Get the plan's restrictions
                    List<PlansRestrictionsEntity>? plansRestrictionsList = await _context.PlansRestrictions.Where(r => r.PlanId == planId).ToListAsync();

                    // Verify if the plan has restrictions
                    if ( (plansRestrictionsList != null) && (plansRestrictionsList.Count > 0) ) {

                        // Remove the restrictions
                        _context.PlansRestrictions.RemoveRange(plansRestrictionsList);

                        // Save Changes
                        await _context.SaveChangesAsync();

                    }

                    // Get the plan's meta
                    List<PlansMetaEntity>? plansMetaList = await _context.PlansMeta.Where(m => m.PlanId == planId).ToListAsync();

                    // Verify if the plan has meta
                    if ( (plansMetaList != null) && (plansMetaList.Count > 0) ) {

                        // Remove the meta
                        _context.PlansMeta.RemoveRange(plansMetaList);

                        // Save changes
                        await _context.SaveChangesAsync();

                    }

                    // Create the cache key
                    string cacheKey = "fc_plan_" + planId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);

                    // Remove the caches for plans group
                    new Cache(_memoryCache).Remove("plans"); 

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

            } catch ( InvalidOperationException e ) {

                // Return the error message
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };

            }

        }

    }

}