/*
 * @interface Plans Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-14
 *
 * This interface is implemented in PlansRepository
 */

// Namespace for Plans Interfaces
namespace FeChat.Utils.Interfaces.Repositories.Plans {

    // Use General Dtos classes
    using FeChat.Models.Dtos;

    // Use the Plans Dtos classes
    using FeChat.Models.Dtos.Plans;

    // Use the Plans Entities
    using FeChat.Models.Entities.Plans;

    /// <summary>
    /// Repository for Plans
    /// </summary>
    public interface IPlansRepository {

        /// <summary>
        /// Create a plan
        /// </summary>
        /// <param name="planDto">Contains the plan's data</param>
        /// <returns>Plan ID</returns>
        Task<ResponseDto<PlanDto>> CreatePlanAsync(PlanDto planDto);

        /// <summary>
        /// Save bulk plans meta
        /// </summary>
        /// <param name="metaEntities">Meta list</param>
        /// <returns>Boolean response</returns>
        Task<bool> SavePlanMetaAsync(List<PlansMetaEntity> metaEntities);

        /// <summary>
        /// Update a plan
        /// </summary>
        /// <param name="planEntity">Plan entity with the plan's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        Task<ResponseDto<bool>> UpdatePlanAsync(PlanEntity planEntity);

        /// <summary>
        /// Update bulk meta
        /// </summary>
        /// <param name="meta">Plans meta list</param>
        /// <returns>Boolean response</returns>
        bool UpdateMetaAsync(List<PlansMetaEntity> meta);

        /// <summary>
        /// Gets all plans
        /// </summary>
        /// <returns>List with plans</returns>
        Task<ResponseDto<List<PlanDto>>> GetAllPlansAsync();

        /// <summary>
        /// Gets plans by page
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with plans</returns>
        Task<ResponseDto<ElementsDto<PlanDto>>> GetPlansByPageAsync(SearchDto searchDto);

        /// <summary>
        /// Get plan data
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>PlanDto with plan's data</returns>
        Task<ResponseDto<PlanDto>> GetPlanAsync(int planId);

        /// <summary>
        /// Get the plans meta
        /// </summary>
        /// <param name="planId">Plan Id</param>
        /// <returns>The meta list or null</returns>
        Task<ResponseDto<List<PlanMetaDto>>> MetaListAsync(int planId);

        /// <summary>
        /// Delete plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>Bool true if the plan was deleted</returns>
        Task<ResponseDto<bool>> DeletePlanAsync(int planId);

        /// <summary>
        /// Save bulk restrictions
        /// </summary>
        /// <param name="restrictions">Members restrictions list</param>
        /// <returns>Boolean response</returns>
        Task<bool> SaveRestrictionsAsync(List<PlansRestrictionsEntity> restrictions);

        /// <summary>
        /// Update bulk restrictions
        /// </summary>
        /// <param name="restrictions">Settings restrictions list</param>
        /// <returns>Boolean response</returns>
        Task<bool> UpdateRestrictionsAsync(List<PlansRestrictionsEntity> restrictions);

        /// <summary>
        /// Get the plan restrictions
        /// </summary>
        /// <param name="PlanId">Plan ID</param>
        /// <returns>The restrictions list or null</returns>
        Task<ResponseDto<List<RestrictionDto>>> RestrictionsListAsync(int PlanId);

        /// <summary>
        /// Save bulk features
        /// </summary>
        /// <param name="features">Plan features list</param>
        /// <returns>Boolean response or error message</returns>
        Task<ResponseDto<bool>> SaveFeaturesAsync(List<PlansFeaturesEntity> features);

        /// <summary>
        /// Get the plan features
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>The features list or null</returns>
        Task<ResponseDto<List<FeatureDto>>> FeaturesListAsync(int planId);

        /// <summary>
        /// Delete the plan features
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>Bool or error message</returns>
        Task<ResponseDto<bool>> FeaturesDeleteAsync(int planId);

    }

}