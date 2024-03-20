/*
 * @class Threads Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the threads for public
 */

// Namespace for Public Threads Controllers
namespace FeChat.Controllers.Public.Threads {

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

    // Use Websites Dtos
    using FeChat.Models.Dtos.Websites;

    // Use General utils for strings
    using FeChat.Utils.General;

    // Use interfaces for Messages Repositories
    using FeChat.Utils.Interfaces.Repositories.Messages;
    
    // Use Websites repositories
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Threads Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/threads")]
    public class UpdateController: Controller {

        /// <summary>
        /// Container for Websites Repository
        /// </summary>
        private readonly IWebsitesRepository _websitesRepository;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="websitesRepository">An instance to the websites repository</param>
        public UpdateController(IWebsitesRepository websitesRepository) {

            // Save website repository
            _websitesRepository = websitesRepository;

        }

        /// <summary>
        /// Update the typing status
        /// </summary>
        /// <param name="websiteId">Website Id</param>
        /// <param name="threadSecret">Thread Secret</param>
        /// <param name="messagesRepository">An instance to the messages repository</param>
        /// <returns>Thread information or error message</returns>
        [AllowAnonymous]
        [HttpPost("{websiteId}/{threadSecret}/typing")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TypingActive(int websiteId, string threadSecret, IMessagesRepository messagesRepository) {

            // Check if website id exists
            if ( websiteId == 0 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteIdMissing")
                });
                
            }

            // Check if Thread Secret is empty
            if ( (threadSecret == null) || (threadSecret == "") ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadSecretMissing")
                });

            }

            // Get website from the database
            ResponseDto<WebsiteDto> websiteResponse = await _websitesRepository.GetWebsiteInfoAsync(websiteId);

            // Check if website exists
            if ( websiteResponse.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteNotFound")
                });
                
            }

            // Verify if the chat is enabled
            if ( websiteResponse.Result.Enabled != 1 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ChatDisabled")
                });
                
            }

            // Create the thread's data
            ThreadDto thread = new() {
                WebsiteId = websiteId,
                ThreadSecret = threadSecret
            };

            // Get the thread from the database
            ResponseDto<ThreadDto> getThread = await messagesRepository.GetThreadByWebsiteIdAsync(thread);

            // Verify if thread exists
            if ( getThread.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadNotFound")
                });

            }

            // Get the typing data
            ResponseDto<TypingDto> typing = await messagesRepository.GetTypingAsync(getThread.Result.ThreadId, 0);

            // Verify if typing data exists
            if ( typing.Result != null ) {

                // Update the typing data
                await messagesRepository.UpdateTypingAsync(typing.Result.Id);

            } else {

                // Save typing
                await messagesRepository.SaveTypingAsync(getThread.Result.ThreadId, 0);

            }

            // Return a json
            return new JsonResult(new {
                success = true
            });

        }
        
    }

}