/*
 * @class Plans Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the plans
 */

// Namespace for Public Plans Controllers
namespace FeChat.Controllers.Public.Plans {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Plans;
    using Utils.Interfaces.Repositories.Plans;

    /// <summary>
    /// Plans Read Controller
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/plans")]
    public class ReadController: Controller {

        /// <summary>
        /// Gets the Plans
        /// </summary>
        /// <param name="plansRepository">An instance for the plans repository</param>
        /// <returns>List with plans or error message</returns>
        [HttpGet("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> PlansList(IPlansRepository plansRepository) {

            // Get all plans
            ResponseDto<List<PlanDto>> plansList = await plansRepository.GetAllPlansAsync();

            // Verify if plans exists
            if ( plansList.Result != null ) {

                // Return plans response
                return new JsonResult(new {
                    success = true,
                    plans = plansList.Result
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = plansList.Message
                });

            }

        }

    }

}