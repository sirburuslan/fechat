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

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Members Create Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/members")]
    public class CreateController: Controller {

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