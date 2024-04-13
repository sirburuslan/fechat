/*
 * @class Plans Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the plans
 */

// Namespace for Plans Repositories
namespace FeChat.Models.Repositories.Plans.Plans {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Plans;
    using Models.Entities.Plans;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Plans Repository
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
        /// Plans Create Repository Constructor
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
        /// Create a plan
        /// </summary>
        /// <param name="planDto">Contains the plan's data</param>
        /// <returns>Plan ID</returns>
        public async Task<ResponseDto<PlanDto>> CreatePlanAsync(PlanDto planDto) {

            try {

                // Create an entity
                PlanEntity planEntity = new() {
                    Name = planDto.Name,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add the entity
                _context.Plans.Add(planEntity);

                // Save the plan
                int PlanId = await _context.SaveChangesAsync();

                // Verify if plan's id exists
                if ( PlanId > 0 ) {

                    // Remove the caches for plans group
                    new Cache(_memoryCache).Remove("plans");

                    // Return success response
                    return new ResponseDto<PlanDto> {
                        Result = new PlanDto {
                            PlanId = planEntity.PlanId
                        },
                        Message = new Strings().Get("PlanCreated")
                    };

                } else {

                    // Return error response
                    return new ResponseDto<PlanDto> {
                        Result = null,
                        Message = new Strings().Get("PlanNotCreated")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return error message
                return new ResponseDto<PlanDto> {
                    Result = null,
                    Message = e.Message
                };
                
            }

        }

    }

}