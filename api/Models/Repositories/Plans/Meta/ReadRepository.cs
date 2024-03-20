/*
 * @class Plans Meta Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to read the plans meta
 */

// Namespace for Plans Meta Repositories
namespace FeChat.Models.Repositories.Plans.Meta {

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
    /// Plans Meta Read Repository
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
        /// Plans Meta Read Repository Constructor
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
        /// Get the plans meta
        /// </summary>
        /// <param name="planId">Plan Id</param>
        /// <returns>The meta list or null</returns>
        public async Task<ResponseDto<List<PlanMetaDto>>> MetaListAsync(int planId) {

            try {

                // Create the cache key for meta
                string cacheKey = "fc_plan_meta_" + planId;

                // Verify if the meta are saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out List<PlanMetaDto>? metaList) ) {

                    // Request the meta
                    metaList = await _context.PlansMeta
                    .Select(m => new PlanMetaDto {
                        MetaId = m.MetaId,
                        PlanId = m.PlanId,
                        MetaName = m.MetaName,
                        MetaValue = m.MetaValue
                    })
                    .Where(p => p.PlanId == planId)
                    .ToListAsync();

                    // Create the cache meta for storing
                    MemoryCacheEntryOptions cacheMeta = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, metaList, cacheMeta);

                }

                // Verify if meta exists
                if ( (metaList != null) && (metaList.Count > 0) ) {

                    // Return the meta
                    return new ResponseDto<List<PlanMetaDto>> {
                        Result = metaList,
                        Message = null
                    };

                } else {

                    // Return the missing meta message
                    return new ResponseDto<List<PlanMetaDto>> {
                        Result = null,
                        Message = new Strings().Get("NoMetaFound")
                    };                    

                }

            } catch ( InvalidOperationException e ) {

                // Return the error message
                return new ResponseDto<List<PlanMetaDto>> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }

}