/*
 * @class Plans Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to manage the plans
 */

// Namespace for Plans Repositories
namespace FeChat.Models.Repositories.Plans {

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

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    /// <summary>
    /// Plans Repository
    /// </summary>
    public class PlansRepository: IPlansRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Entity Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public PlansRepository(IMemoryCache memoryCache, Db db) {

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

            // Init Create Repository
            Plans.CreateRepository createRepository = new(_memoryCache, _context);

            // Create a plan and return the response
            return await createRepository.CreatePlanAsync(planDto);

        }

        /// <summary>
        /// Save bulk plans meta
        /// </summary>
        /// <param name="metaEntities">Meta list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SavePlanMetaAsync(List<PlansMetaEntity> metaEntities) {

            // Init Create Repository
            Meta.CreateRepository createRepository = new(_memoryCache, _context);

            // Create plan meta and return the response
            return await createRepository.SavePlanMetaAsync(metaEntities);

        }

        /// <summary>
        /// Update a plan
        /// </summary>
        /// <param name="planEntity">Plan entity with the plan's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdatePlanAsync(PlanEntity planEntity) {

            // Init Update Repository
            Plans.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update a plan and return the response
            return await updateRepository.UpdatePlanAsync(planEntity);

        }

        /// <summary>
        /// Update bulk meta
        /// </summary>
        /// <param name="meta">Plans meta list</param>
        /// <returns>Boolean response</returns>
        public bool UpdateMetaAsync(List<PlansMetaEntity> meta) {

            // Init Update Repository
            Meta.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update plan meta and return the response
            return updateRepository.UpdateMetaAsync(meta);

        }

        /// <summary>
        /// Gets all plans
        /// </summary>
        /// <returns>List with plans</returns>
        public async Task<ResponseDto<List<PlanDto>>> GetAllPlansAsync() {

            // Init Read Repository
            Plans.ReadRepository readRepository = new(_memoryCache, _context);

            // Read all plans and return the response
            return await readRepository.GetAllPlansAsync();

        }

        /// <summary>
        /// Gets plans by page
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with plans</returns>
        public async Task<ResponseDto<ElementsDto<PlanDto>>> GetPlansByPageAsync(SearchDto searchDto) {

            // Init Read Repository
            Plans.ReadRepository readRepository = new(_memoryCache, _context);

            // Get plans by page and return the response
            return await readRepository.GetPlansByPageAsync(searchDto);

        }

        /// <summary>
        /// Get plan data
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>PlanDto with plan's data</returns>
        public async Task<ResponseDto<PlanDto>> GetPlanAsync(int planId) {

            // Init Read Repository
            Plans.ReadRepository readRepository = new(_memoryCache, _context);

            // Get plan by id and return the response
            return await readRepository.GetPlanAsync(planId);

        }

        /// <summary>
        /// Get the plans meta
        /// </summary>
        /// <param name="planId">Plan Id</param>
        /// <returns>The meta list or null</returns>
        public async Task<ResponseDto<List<PlanMetaDto>>> MetaListAsync(int planId) {

            // Init Read Repository
            Meta.ReadRepository readRepository = new(_memoryCache, _context);

            // Read plan meta and return the response
            return await readRepository.MetaListAsync(planId);

        }

        /// <summary>
        /// Delete plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>Bool true if the plan was deleted</returns>
        public async Task<ResponseDto<bool>> DeletePlanAsync(int planId) {

            // Init Delete Repository
            Plans.DeleteRepository deleteRepository = new(_memoryCache, _context);

            // Delete plan by id and return the response
            return await deleteRepository.DeletePlanAsync(planId);

        }

        /// <summary>
        /// Save bulk restrictions
        /// </summary>
        /// <param name="restrictions">Plans restrictions list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SaveRestrictionsAsync(List<PlansRestrictionsEntity> restrictions) {

            // Init Create Repository
            Restrictions.CreateRepository createRepository = new(_memoryCache, _context);

            // Create plan restrictions and return the response
            return await createRepository.SaveRestrictionsAsync(restrictions);

        }

        /// <summary>
        /// Update bulk restrictions
        /// </summary>
        /// <param name="restrictions">Settings restrictions list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> UpdateRestrictionsAsync(List<PlansRestrictionsEntity> restrictions) {

            // Init Update Repository
            Restrictions.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update plan restrictions and return the response
            return await updateRepository.UpdateRestrictionsAsync(restrictions);

        }

        /// <summary>
        /// Get the plan restrictions
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>The restrictions list or null</returns>
        public async Task<ResponseDto<List<RestrictionDto>>> RestrictionsListAsync(int planId) {

            // Init Read Repository
            Restrictions.ReadRepository readRepository = new(_memoryCache, _context);

            // Read plan restrictions and return the response
            return await readRepository.RestrictionsListAsync(planId);

        }

        /// <summary>
        /// Save bulk features
        /// </summary>
        /// <param name="features">Plan features list</param>
        /// <returns>Boolean response or error message</returns>
        public async Task<ResponseDto<bool>> SaveFeaturesAsync(List<PlansFeaturesEntity> features) {

            // Init Create Repository
            Features.CreateRepository createRepository = new(_memoryCache, _context);

            // Create plan features and return the response
            return await createRepository.SaveFeaturesAsync(features);

        }

        /// <summary>
        /// Get the plan features
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>The features list or null</returns>
        public async Task<ResponseDto<List<FeatureDto>>> FeaturesListAsync(int planId) {

            // Init Read Repository
            Features.ReadRepository readRepository = new(_memoryCache, _context);

            // Read plan features and return the response
            return await readRepository.FeaturesListAsync(planId);

        }

        /// <summary>
        /// Delete the plan features
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>Bool or error message</returns>
        public async Task<ResponseDto<bool>> FeaturesDeleteAsync(int planId) {

            // Init Delete Repository
            Features.DeleteRepository deleteRepository = new(_memoryCache, _context);

            // Delete plan features and return the response
            return await deleteRepository.FeaturesDeleteAsync(planId);

        }

    }

}