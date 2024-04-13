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

    // System Namespaces
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Subscriptions;
    using Utils.General;
    using Utils.Interfaces.Repositories.Plans;
    using Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Plans Delete Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans")]
    public class DeleteController: Controller {

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