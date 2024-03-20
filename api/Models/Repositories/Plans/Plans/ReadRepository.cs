/*
 * @class Plans Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the plans
 */

// Namespace for Plans Repositories
namespace FeChat.Models.Repositories.Plans.Plans {

    // Use the Entity Framework
    using Microsoft.EntityFrameworkCore;

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;
    
    // Use General Dtos classes
    using FeChat.Models.Dtos;

    // Use the Plans Dtos classes
    using FeChat.Models.Dtos.Plans;

    // Use the Plans Entities
    using FeChat.Models.Entities.Plans;

    // Use the Configuration for database connection
    using FeChat.Utils.Configuration;
    
    // Use the General Utils
    using FeChat.Utils.General;

    /// <summary>
    /// Plans Repository
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
        /// Plans Read Repository Constructor
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
        /// Gets all plans
        /// </summary>
        /// <returns>List with plans</returns>
        public async Task<ResponseDto<List<PlanDto>>> GetAllPlansAsync() {

            try {

                // Create the cache key
                string cacheKey = "fc_all_plans";

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out List<PlanDto>? plansResponse ) ) {

                    // Request the plans
                    plansResponse = await _context.Plans
                    .Select(m => new PlanDto {
                            PlanId = m.PlanId,
                            Name = m.Name,
                            Price = m.Price,
                            Currency = m.Currency,
                            Created = m.Created,
                            Features = _context.PlansFeatures
                                .Where(pf => pf.PlanId == m.PlanId)
                                .Select(pf => new FeatureDto
                                {
                                    FeatureId = pf.FeatureId,
                                    PlanId = pf.PlanId,
                                    FeatureText = pf.FeatureText
                                })
                                .ToList()
                        }
                    )
                    .OrderBy(m => m.PlanId)
                    .ToListAsync();

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    //_memoryCache.Set(cacheKey, plansResponse, cacheOptions);

                }

                // Verify if plans exists
                if ( (plansResponse != null) && (plansResponse.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<List<PlanDto>> {
                        Result = plansResponse,
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<List<PlanDto>> {
                        Result = null,
                        Message = new Strings().Get("NoPlansFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<List<PlanDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Gets plans by page
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with plans</returns>
        public async Task<ResponseDto<ElementsDto<PlanDto>>> GetPlansByPageAsync(SearchDto searchDto) {

            try {

                // Prepare the page
                int page = (searchDto.Page > 0)?searchDto.Page:1;

                // Prepare the total results
                int total = 10;

                // Split the keys
                string[] searchKeys = searchDto.Search!.Split(' ');

                // Create the cache key
                string cacheKey = "fc_plans_" + string.Join("_", searchKeys) + '_' + searchDto.Page;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out Tuple<List<PlanDto>, int>? plansResponse ) ) {

                    // Request the plans without projecting to PlanDto yet
                    IQueryable<PlanEntity> query = _context.Plans.AsQueryable();

                    // Apply filtering based on searchKeys
                    foreach (string key in searchKeys) {

                        // To avoid closure issue
                        string tempKey = key;

                        // Set where parameters
                        query = query.Where(m =>
                            EF.Functions.Like(m.Name!.ToLower(), $"%{tempKey.ToLower()}%")
                        );

                    }

                    // Request the plans
                    List<PlanDto> plans = await query
                    .Select(m => new PlanDto {
                            PlanId = m.PlanId,
                            Name = m.Name,
                            Price = m.Price,
                            Currency = m.Currency,
                            Created = m.Created
                        }
                    )
                    .OrderByDescending(m => m.PlanId)
                    .Skip((page - 1) * total)
                    .Take(total)
                    .ToListAsync();

                    // Get the total count before pagination
                    int totalCount = await query.CountAsync();

                    // Add data to plan response
                    plansResponse = new Tuple<List<PlanDto>, int>(plans, totalCount);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, plansResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("plans", cacheKey);

                }

                // Verify if plans exists
                if ( (plansResponse != null) && (plansResponse.Item1.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<ElementsDto<PlanDto>> {
                        Result = new ElementsDto<PlanDto> {
                            Elements = plansResponse.Item1,
                            Total = plansResponse.Item2,
                            Page = searchDto.Page
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<ElementsDto<PlanDto>> {
                        Result = null,
                        Message = new Strings().Get("NoPlansFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<ElementsDto<PlanDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Get plan data
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>PlanDto with plan's data</returns>
        public async Task<ResponseDto<PlanDto>> GetPlanAsync(int planId) {

            try {

                // Cache key for plan
                string cacheKey = "fc_plan_" + planId;

                // Verify if the plan is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out PlanDto? planDto ) ) {

                    // Get the plan by id
                    planDto = await _context.Plans
                    .Select(p => new PlanDto {
                        PlanId = p.PlanId,
                        Name = p.Name,
                        Price = p.Price,
                        Currency = p.Currency,
                        Created = p.Created
                    })
                    .FirstAsync(u => u.PlanId == planId); 

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, planDto, cacheOptions);

                }

                // Verify if plan exists
                if ( planDto != null ) {

                    // Return the plan data
                    return new ResponseDto<PlanDto> {
                        Result = planDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<PlanDto> {
                        Result = null,
                        Message = new Strings().Get("PlanNotFound")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return the error message
                return new ResponseDto<PlanDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }

}