/*
 * @class Messages Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the messages for guests
 */

// Namespace for Public Messages Controllers
namespace FeChat.Controllers.Public.Messages {

    // Use the MVC features
    using Microsoft.AspNetCore.Mvc;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Authorization feature to allow guests access
    using Microsoft.AspNetCore.Authorization;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use general dtos for responses
    using FeChat.Models.Dtos;

    // Use the Messages Dtos
    using FeChat.Models.Dtos.Messages;

    // Use Websites Dtos
    using FeChat.Models.Dtos.Websites;

    // Use General utils for strings
    using FeChat.Utils.General;

    // Use the Messages Repositories
    using FeChat.Utils.Interfaces.Repositories.Messages;

    // Use the Repositories for Websites
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Create Messages Controller
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/messages")]
    public class ReadController: Controller {

        /// <summary>
        /// Gets the messages
        /// </summary>
        /// <param name="messagesListDto">Parameters to list the messages</param>
        /// <param name="websitesRepository">An instance for the websites repository</param>
        /// <param name="messagesRepository">An instance for the messages repository</param>
        /// <returns></returns>
        [HttpPost("list")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MessagesList([FromBody] MessagesListDto messagesListDto, IWebsitesRepository websitesRepository, IMessagesRepository messagesRepository) {

            // Check if website id exists
            if ( messagesListDto.WebsiteId == 0 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteIdMissing")
                });
                
            }

            // Check if Thread Secret is empty
            if ( (messagesListDto.ThreadSecret == null) || (messagesListDto.ThreadSecret == "") ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("NoMessagesFound")
                });

            }

            // Get website from the database
            ResponseDto<WebsiteDto> websiteResponse = await websitesRepository.GetWebsiteInfoAsync(messagesListDto.WebsiteId);

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
                WebsiteId = messagesListDto.WebsiteId,
                ThreadSecret = messagesListDto.ThreadSecret
            };

            // Get the thread from the database
            ResponseDto<ThreadDto> getThread = await messagesRepository.GetThreadByWebsiteIdAsync(thread);

            // Verify if thread exists
            if ( getThread.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("NoMessagesFound")
                });

            }

            // Set thread id as parameter for messages request
            messagesListDto.ThreadId = getThread.Result!.ThreadId;

            // Get the messages
            ResponseDto<ElementsDto<MessageDto>> messagesList = await messagesRepository.MessagesListAsync(messagesListDto);

            // Verify if the messages exists
            if ( messagesList.Result != null ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    messagesList.Result
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = messagesList.Message
                });

            }

        }

    }

}