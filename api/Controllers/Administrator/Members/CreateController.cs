/*
 * @class Members Create Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the members accounts
 */

// Namespace for Administrator Members Controllers
namespace FeChat.Controllers.Administrator.Members {

    // Use the Asp Net core MVC
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;
    
    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use General Utils
    using FeChat.Utils.General;

    // Use the Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Members Create Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/members")]
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
        /// Create a member
        /// </summary>
        /// <param name="newMemberDto">Data transfer object with member information</param>
        /// <param name="membersRepository">Contains a session to the Members repository</param>
        /// <returns>Message if the member was created or error</returns>
        [Authorize]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreateMember([FromBody] NewMemberDto newMemberDto, IMembersRepository membersRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Set user role
            newMemberDto.Role = 1;

            // Create member
            ResponseDto<MemberDto> createMember = await membersRepository.CreateMemberAsync(newMemberDto);

            // Verify if the account was created
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