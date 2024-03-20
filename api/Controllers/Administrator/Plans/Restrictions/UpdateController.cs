/*
 * @class Plans Restrictions Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the plans restrictions
 */

// Namespace for Administrator Plans Restrictions
namespace FeChat.Controllers.Administrator.Plans.Restrictions {

    // Use the classes to get dtos properties
    using System.Reflection;

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Authentication feature to get the access token
    using Microsoft.AspNetCore.Authentication;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use Plans dtos classes
    using FeChat.Models.Dtos.Plans;

    // Use the Dtos for Members
    using FeChat.Models.Dtos.Members;

    // Use the plans entity
    using FeChat.Models.Entities.Plans;

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Plans Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans/restrictions")]
    public class UpdateController: Controller {

        /// <summary>
        /// Save or update the plans restrictions
        /// </summary>
        /// <param name="restrictionsDto">Received restrictions for saving</param>
        /// <param name="PlanId">Plan ID</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="plansRepository">An instance for the plans repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("{PlanId}")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SaveRestrictions([FromBody] RestrictionsDto restrictionsDto, int PlanId, IMembersRepository membersRepository, IPlansRepository plansRepository) {

            // Get the plan's data
            ResponseDto<PlanDto> planData = await plansRepository.GetPlanAsync(PlanId);

            // Check if plan exists
            if ( planData.Result == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanNotFound")
                });
                
            }

            // Get the plan's restrictions
            ResponseDto<List<RestrictionDto>> savedRestrictions = await plansRepository.RestrictionsListAsync(PlanId);

            // Restrictions to update container
            List<PlansRestrictionsEntity> restrictionsUpdate = new();

            // Saved restrictions names
            List<string> restrictionsSaved = new();

            // Check if restrictions exists
            if ( savedRestrictions.Result != null ) {

                // Get restrictions length
                int restrictionsLength = savedRestrictions.Result!.Count;

                // List the saved restrictions
                for ( int r = 0; r < restrictionsLength; r++ ) {

                    // Get restriction's name
                    PropertyInfo? restrictionName = typeof(RestrictionsDto).GetProperty(savedRestrictions.Result[r].RestrictionName);

                    // Verify if restriction's name is not null
                    if ( restrictionName != null ) {

                        // Save the restriction's name
                        restrictionsSaved.Add(savedRestrictions.Result[r].RestrictionName);

                        // Get the restriction's value
                        string restrictionValue = restrictionName!.GetValue(restrictionsDto)!.ToString()!;

                        // Create the restriction's params
                        PlansRestrictionsEntity restrictionUpdate = new() {
                            RestrictionId = savedRestrictions.Result[r].RestrictionId,
                            PlanId = savedRestrictions.Result[r].PlanId,
                            RestrictionName = savedRestrictions.Result[r].RestrictionName,
                            RestrictionValue = int.Parse(restrictionValue)
                        };

                        // Add restriction in the update list
                        restrictionsUpdate.Add(restrictionUpdate);

                    }
                    
                }

            }

            // Restrictions to save container
            List<PlansRestrictionsEntity> restrictionsSave = new();

            // Get all restrictions dto properties
            PropertyInfo[] propertyInfos = typeof(RestrictionsDto).GetProperties();

            // Get properties length
            int propertiesLength = propertyInfos.Length;

            // List the properties
            for ( int p = 0; p < propertiesLength; p++ ) {

                // If is PlanId continue
                if ( propertyInfos[p].Name == "PlanId" ) {
                    continue;
                }

                // If value is null continue
                if ( propertyInfos[p].GetValue(restrictionsDto) == null ) {
                    continue;
                }

                // Check if the restriction is not saved already
                if ( !restrictionsSaved.Contains(propertyInfos[p].Name) ) {

                    // Create the restriction
                    PlansRestrictionsEntity restriction = new() {
                        PlanId = PlanId,
                        RestrictionName = propertyInfos[p].Name,
                        RestrictionValue = (int)(propertyInfos[p].GetValue(restrictionsDto) ?? 0)
                    };

                    // Add restriction to the save list
                    restrictionsSave!.Add(restriction);

                }

            }

            // Errors counter
            int errors = 0;

            // Verify if restrictions for updating exists
            if ( restrictionsUpdate.Count > 0 ) {

                // Update restrictions
                bool update_restrictions = await plansRepository.UpdateRestrictionsAsync(restrictionsUpdate);

                // Check if an error has been occurred when the restrictions were updated
                if ( !update_restrictions ) {
                    errors++;
                }
                
            } 

            // Verify if restrictions for saving exists
            if ( restrictionsSave.Count > 0 ) {

                // Save restrictions
                bool saveRestrictions = await plansRepository.SaveRestrictionsAsync(restrictionsSave);

                // Check if an error has been occurred when the restrictions were saved
                if ( !saveRestrictions ) {
                    errors++;
                }

            }

            // Verify if no errors occurred
            if ( errors == 0 ) {

                // Create a success response
                var response = new {
                    success = true,
                    message = new Strings().Get("PlanRestrictionsUpdated")
                };

                // Return a json
                return new JsonResult(response);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = new Strings().Get("PlanRestrictionsNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            }

        }

    }

}