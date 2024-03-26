/*
 * @class Plans Delete Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the plans
 */

// Namespace for the Administrator Plans Controllers
namespace FeChat.Controllers.Administrator.Plans {

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use the Dtos for subscriptions
    using FeChat.Models.Dtos.Subscriptions;

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use the Subscriptions Repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Plans Delete Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans")]
    public class DeleteController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public DeleteController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Delete a plan
        /// </summary>
        /// <param name="planId">Contains the plan's ID</param>
        /// <param name="plansRepository">Contains a session to the Plans repository</param>
        /// <param name="subscriptionsRepository">Contains a session to the Subscriptions repository</param>
        /// <returns>Success or error message</returns>
        [HttpDelete("{planId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DeletePlan(int planId, IPlansRepository plansRepository, ISubscriptionsRepository subscriptionsRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get the subscriptions by plan id
            ResponseDto<List<SubscriptionDto>> subscriptions = await subscriptionsRepository.GetSubscriptionsByPlanIdAsync(planId);

            // Verify if the plan has subscriptions
            if ( subscriptions.Result != null ) {

                // Return error message
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanHasSubscriptions")
                });   

            }

            // Delete a plan
            ResponseDto<bool> deletePlan = await plansRepository.DeletePlanAsync(planId);

            // Check if the plan was deleted
            if ( deletePlan.Result ) {

                // Return success message
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("PlanWasDeleted")
                });                

            } else if ( deletePlan.Message != null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = deletePlan.Message
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanWasNotDeleted")
                });

            }

        } 

    }

}