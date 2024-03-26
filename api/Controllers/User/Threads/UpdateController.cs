/*
 * @class Threads Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the threads for users
 */

// Namespace for User Threads Controllers
namespace FeChat.Controllers.User.Threads {

    // Use the MVC for the controller interface
    using Microsoft.AspNetCore.Mvc;

    // Use the authorization for access restriction
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use general dtos
    using FeChat.Models.Dtos;

    // Use dtos for messages
    using FeChat.Models.Dtos.Messages;

    // Use interfaces for Messages Repositories
    using FeChat.Utils.Interfaces.Repositories.Messages;
    
    // Use General utils for member role validation
    using FeChat.Utils.General;

    /// <summary>
    /// Threads Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/threads")]
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
        /// Update the typing status
        /// </summary>
        /// <param name="threadId">Contains the thread's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="messagesRepository">An instance to the Messages repository</param>
        /// <returns>Thread information or error message</returns>
        [Authorize]
        [HttpPost("{threadId}/typing")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> TypingActive(int threadId, Member memberInfo, IMessagesRepository messagesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get the typing data
            ResponseDto<TypingDto> typing = await messagesRepository.GetTypingAsync(threadId, memberInfo.Info!.MemberId);

            // Verify if typing data exists
            if ( typing.Result != null ) {

                // Update the typing data
                await messagesRepository.UpdateTypingAsync(typing.Result.Id);

            } else {

                // Save typing
                await messagesRepository.SaveTypingAsync(threadId, memberInfo.Info.MemberId);

            }

            // Return a json
            return new JsonResult(new {
                success = true
            });

        }
        
    }

}