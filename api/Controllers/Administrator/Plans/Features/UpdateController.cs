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

    // System Namespaces
    using System.Text.Encodings.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Plans;
    using Models.Entities.Plans;
    using Utils.General;
    using Utils.Interfaces.Repositories.Plans;

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
        /// <param name="plansRepository">An instance for the plans repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("{PlanId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> SaveFeatures([FromBody] string[] featuresList, int planId, IPlansRepository plansRepository) {

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