/*
 * @class Plans Features Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the plans features
 */

// Namespace for the Administrator Plans Features
namespace FeChat.Controllers.Administrator.Plans.Features {

    // Use Web encoding for JavaScript sanitizing
    using System.Text.Encodings.Web;

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

    // Use Members dtos
    using FeChat.Models.Dtos.Members;

    // Use Plans dtos classes
    using FeChat.Models.Dtos.Plans;

    // Use the plans entity
    using FeChat.Models.Entities.Plans;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;    

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    /// <summary>
    /// Plans Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans/features")]
    public class UpdateController: Controller {

        /// <summary>
        /// Save or update the plans features
        /// </summary>
        /// <param name="featuresList">Received features for saving</param>
        /// <param name="planId">Plan ID</param>
        /// <param name="membersRepository">An instance for the members repository</param>
        /// <param name="plansRepository">An instance for the plans repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("{PlanId}")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SaveFeatures([FromBody] string[] featuresList, int planId, IMembersRepository membersRepository, IPlansRepository plansRepository) {

            // Get the plan's data
            ResponseDto<PlanDto> planData = await plansRepository.GetPlanAsync(planId);

            // Check if plan exists
            if ( planData.Result == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanNotFound")
                });
                
            }

            // Delete the plan's features
            ResponseDto<bool> deleteFeatures = await plansRepository.FeaturesDeleteAsync(planId);

            // Verify if deleteFeatures contains an error message
            if ( deleteFeatures.Message != null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = deleteFeatures.Message
                });
                
            }

            // Verify if features exists
            if ( featuresList.Length > 0 ) {

                // Valid features container
                List<PlansFeaturesEntity> validFeatures = new();

                // Total features
                int featuresTotal = featuresList.Length;

                // List the features list
                for ( int f = 0; f < featuresTotal; f++ ) {

                    // Create new feature
                    PlansFeaturesEntity feature = new() {
                        PlanId = planId,
                        FeatureText = System.Web.HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(featuresList[f] ?? string.Empty))
                    };

                    // Set feature to the container
                    validFeatures.Add(feature);

                }

                // Save features
                ResponseDto<bool> saveFeatures = await plansRepository.SaveFeaturesAsync(validFeatures);

                // Verify if saveFeatures contains an error message
                if ( saveFeatures.Message != null ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = saveFeatures.Message
                    });
                    
                }

            }

            // Create a success response
            var response = new {
                success = true,
                message = new Strings().Get("PlanFeaturesUpdated")
            };

            // Return a json
            return new JsonResult(response);

        }

    }

}