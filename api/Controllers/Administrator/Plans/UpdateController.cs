/*
 * @class Plans Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-15
 *
 * This class is used to create the plans
 */

// Namespace for Administrator Plans Controllers
namespace FeChat.Controllers.Administrator.Plans {

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use Plans dtos classes
    using FeChat.Models.Dtos.Plans;
    
    // Use the General Utils classes for Strings
    using FeChat.Utils.General;

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
    [Route("api/v{version:apiVersion}/admin/plans")]
    public class UpdateController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public UpdateController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Update the plan basic informations
        /// </summary>
        /// <param name="planDto">Contains the received information</param>
        /// <param name="PlanId">Contains the plan's ID</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="plansRepository">Plans repository instance</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("{PlanId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> UpdatePlanBasic([FromBody] PlanDto planDto, int PlanId, IMembersRepository membersRepository, IPlansRepository plansRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

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

            // Verify if the price is valid
            if ( !int.TryParse(planDto.Price, out _) && !double.TryParse(planDto.Price, out _) ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanPriceNotValid")
                });

            }

            // Create the update
            PlanEntity planEntity = new() {
                PlanId = PlanId,
                Name = planDto.Name,
                Price = planDto.Price.Contains(',') || planDto.Price.Contains('.')?planDto.Price:planDto.Price + ".00",
                Currency = planDto.Currency,
                Created = planData.Result.Created ?? 0
            };

            // Update the plan's data
            ResponseDto<bool> UpdatePlan = await plansRepository.UpdatePlanAsync(planEntity);

            // Verify the plan was updated
            if ( UpdatePlan.Result ) {
                
                // Create a success response
                var updateResponse = new {
                    success = true,
                    message = new Strings().Get("PlanUpdated")
                };

                // Return a json
                return new JsonResult(updateResponse);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = UpdatePlan.Message ?? new Strings().Get("PlanNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            }

        }

    }

}