/*
 * @class Threads Delete Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the threads
 */

// Namespace for User Threads Controllers
namespace FeChat.Controllers.User.Threads {

    // System Namespaces
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Utils.General;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Threads Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/threads")]
    public class DeleteController: Controller {

        /// <summary>
        /// Delete a thread
        /// </summary>
        /// <param name="threadId">Contains the thread's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="messagesRepository">An instance to the messages repository</param>
        /// <returns>Message about the thread status</returns>
        [HttpDelete("{threadId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DeleteThread(int threadId, Member memberInfo, IMessagesRepository messagesRepository) {

            // Delete a member
            ResponseDto<bool> deleteThread = await messagesRepository.DeleteThreadAsync(threadId, memberInfo.Info!.MemberId);

            // Check if the thread was deleted
            if ( deleteThread.Result ) {

                // Return success message
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("ThreadWasDeleted")
                });                

            } else if ( deleteThread.Message != null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = deleteThread.Message
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadWasNotDeleted")
                });

            }

        }
        
    }

}