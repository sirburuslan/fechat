/*
 * @class Plans Create Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the plans
 */

// Namespace for Administrator Plans Controllers
namespace FeChat.Controllers.Administrator.Plans {

    // System Namespaces
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Asp.Versioning;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Plans;
    using Utils.Interfaces.Repositories.Plans;
    
    /// <summary>
    /// Plans Create Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans")]
    public class CreateController: Controller {

        /// <summary>
        /// Create a plan
        /// </summary>
        /// <param name="planDto">Data transfer object with plan information</param>
        /// <param name="plansRepository">Instance for the plans repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreatePlan([FromBody] PlanDto planDto, IPlansRepository plansRepository) {

            // Create plan
            ResponseDto<PlanDto> createMember = await plansRepository.CreatePlanAsync(planDto);

            // Verify if the plan was created
            if ( createMember.Result != null ) {

                // Create a success response
                var response = new {
                    success = true,
                    message = createMember.Message
                };

                // Return a json
                return new JsonResult(response);                

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = createMember.Message
                };

                // Return a json
                return new JsonResult(response);       

            }

        }

    }

}