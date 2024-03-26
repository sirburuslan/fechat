/*
 * @class Plans Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-15
 *
 * This class is used to read the plans
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

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use Plans dtos classes
    using FeChat.Models.Dtos.Plans;

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Plans Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/plans")]
    public class ReadController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public ReadController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Get the list with plans
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="plansRepository">An instance to the plans repository</param>
        /// <returns>Plans list or error message</returns>
        [Authorize]
        [HttpPost("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> PlansList([FromBody] SearchDto searchDto, IMembersRepository membersRepository, IPlansRepository plansRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get all plans
            ResponseDto<ElementsDto<PlanDto>> plansList = await plansRepository.GetPlansByPageAsync(searchDto);

            // Verify if plans exists
            if ( plansList.Result != null ) {

                // Return plans response
                return new JsonResult(new {
                    success = true,
                    plansList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = plansList.Message
                });

            }

        }

        /// <summary>
        /// Gets the plan information
        /// </summary>
        /// <param name="planId">Contains the plan's ID</param>
        /// <param name="members">Contains an instance to the Members repository</param>
        /// <param name="plansRepository">Contains a session to the plans repository</param>
        /// <returns>Plan data or error message</returns>
        [Authorize]
        [HttpGet("{planId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> PlanInfo(int planId, IMembersRepository members, IPlansRepository plansRepository) {

            // Get the plan's data
            ResponseDto<PlanDto> planDto = await plansRepository.GetPlanAsync(planId);

            // Verify if plan exists
            if ( planDto.Result != null ) {

                // Create the plan's information
                Dictionary<string, object> plan = new() {

                    // Add Plan id
                    { "PlanId", planDto.Result.PlanId.ToString() },

                    // Add Plan Name
                    { "Name", planDto.Result.Name ?? string.Empty },

                    // Add Plan Price
                    { "Price", planDto.Result.Price ?? string.Empty },

                    // Add Plan Currency
                    { "Currency", planDto.Result.Currency ?? string.Empty }                              

                };

                // Get the plan's restrictions
                ResponseDto<List<RestrictionDto>> savedRestrictions = await plansRepository.RestrictionsListAsync(planId);

                // Check if restrictions exists
                if ( savedRestrictions.Result != null ) {

                    // Get restrictions length
                    int restrictionsLength = savedRestrictions.Result!.Count;

                    // List the saved restrictions
                    for ( int r = 0; r < restrictionsLength; r++ ) {

                        // Set restriction name
                        string restrictionName = savedRestrictions.Result[r].RestrictionName;
                        
                        // Add Restriction
                        plan.Add(restrictionName, savedRestrictions.Result[r].RestrictionValue);
                        
                    }

                }

                // Get the plan's features
                ResponseDto<List<FeatureDto>> savedFeatures = await plansRepository.FeaturesListAsync(planId);

                // Check if features exists
                if ( savedFeatures.Result != null ) {

                    // Get features length
                    int featuresLength = savedFeatures.Result!.Count;

                    // Features list
                    List<string> featuresList = new();

                    // List the saved features
                    for ( int r = 0; r < featuresLength; r++ ) {
                        
                        // Add Feature
                        featuresList.Add(savedFeatures.Result[r].FeatureText);
                        
                    }

                    // Add Features
                    plan.Add("Features", featuresList);

                }                

                // Return a json
                return new JsonResult(new {
                    success = true,
                    plan
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanNotFound")
                });

            }

        }

    }

}