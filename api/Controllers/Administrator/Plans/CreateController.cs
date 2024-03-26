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

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;
    
    /// <summary>
    /// Plans Create Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans")]
    public class CreateController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public CreateController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Create a plan
        /// </summary>
        /// <param name="planDto">Data transfer object with plan information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="plansRepository">Instance for the plans repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreatePlan([FromBody] PlanDto planDto, IMembersRepository membersRepository, IPlansRepository plansRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

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