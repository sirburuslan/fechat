/*
 * @class Threads Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the threads for users
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

    // Use dtos for members
    using FeChat.Models.Dtos.Members;

    // Use dtos for messages
    using FeChat.Models.Dtos.Messages;

    // Use interfaces for Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use interfaces for Messages Repositories
    using FeChat.Utils.Interfaces.Repositories.Messages;
    
    // Use General utils for member role validation
    using FeChat.Utils.General;

    /// <summary>
    /// Threads Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/threads")]
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
        /// Get the threads list
        /// </summary>
        /// <param name="searchDto">Search information</param>
        /// <param name="websiteId">Optionally Website Id</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="messagesRepository">Instance for Threads repository</param>
        /// <returns>The list with threads or text message</returns>
        [Authorize]
        [HttpPost("list/{websiteId?}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> List([FromBody] SearchDto searchDto, int? websiteId, Member memberInfo, IMessagesRepository messagesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get all threads
            ResponseDto<ElementsDto<ResponseThreadDto>> threadsList = await messagesRepository.GetThreadsAsync(searchDto, memberInfo.Info!.MemberId, websiteId);

            // Verify if threads exists
            if ( threadsList.Result != null ) {

                // Return threads response
                return new JsonResult(new {
                    success = true,
                    threadsList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = threadsList.Message
                });

            }

        }

        /// <summary>
        /// Get the last updated threads
        /// </summary>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="messagesRepository">Instance for Messages repository</param>
        /// <returns>The list with threads or text message</returns>
        [Authorize]
        [HttpGet("last")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Last(Member memberInfo, IMessagesRepository messagesRepository) {

            // Get last 5 updated threads
            ResponseDto<ElementsDto<ResponseThreadDto>> threadsList = await messagesRepository.GetHotThreadsAsync(memberInfo.Info!.MemberId);

            // Verify if threads exists
            if ( threadsList.Result != null ) {

                // Return threads response
                return new JsonResult(new {
                    success = true,
                    threadsList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = threadsList.Message
                });

            }

        }

        /// <summary>
        /// Gets the thread information
        /// </summary>
        /// <param name="threadId">Contains the thread's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="messagesRepository">An instance to the messages repository</param>
        /// <returns>Thread information or error message</returns>
        [Authorize]
        [HttpGet("{threadId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Thread(int threadId, Member memberInfo, IMembersRepository membersRepository, IMessagesRepository messagesRepository) {

            // Get the thread's data
            ResponseDto<ThreadDto> threadDto = await messagesRepository.GetThreadAsync(threadId, memberInfo.Info!.MemberId);

            // Verify if thread exists
            if ( threadDto.Result != null ) {

                // Create the thread's information
                Dictionary<string, object> thread = new() {

                    // Add Thread Id
                    { "threadId", threadDto.Result.ThreadId.ToString() },

                    // Add creation date
                    { "created", threadDto.Result.Created.ToString() }                

                };

                // Create the guest's information
                Dictionary<string, object> guest = new() {

                    // Add Guest Id
                    { "guestId", threadDto.Result.GuestId.ToString() },

                    // Add Guest Name
                    { "guestName", threadDto.Result.GuestName ?? string.Empty },

                    // Add Guest Email
                    { "guestEmail", threadDto.Result.GuestEmail ?? string.Empty }, 

                    // Add Guest Ip
                    { "guestIp", threadDto.Result.GuestIp ?? string.Empty },
                    
                    // Add Guest Latitude
                    { "guestLatitude", threadDto.Result.GuestLatitude ?? string.Empty },

                    // Add Guest Longitude
                    { "guestLongitude", threadDto.Result.GuestLongitude ?? string.Empty },                                       

                };

                // Create parameters for messages request
                MessagesListDto messagesListDto = new() {
                    ThreadId = threadDto.Result.ThreadId,
                    Page = 1,
                    MemberId = memberInfo.Info.MemberId
                };

                // Get the messages
                ResponseDto<ElementsDto<MessageDto>> messagesList = await messagesRepository.MessagesListAsync(messagesListDto);

                // Verify if the messages exists
                if ( messagesList.Result != null ) {

                    // Add messages to the thread
                    thread.Add("messages", messagesList.Result);

                }

                // Get the member's settings
                ResponseDto<List<OptionDto>> memberOptionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

                // Verify if the options list exists
                if ( memberOptionsList.Result != null ) {

                    // Get options length
                    int memberOptionsLength = memberOptionsList.Result.Count;

                    // List the options
                    for ( int o = 0; o < memberOptionsLength; o++ ) {

                        // Check if options name is ProfilePhoto
                        if ( memberOptionsList.Result[o].OptionName == "ProfilePhoto" ) {

                            // Set user photo to the thread
                            thread.Add("userPhoto", memberOptionsList.Result[o].OptionValue);

                        }

                    }

                }

                // Return a json
                return new JsonResult(new {
                    success = true,
                    thread,
                    guest
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadNotFound")
                });

            }

        }

        /// <summary>
        /// Gets the thread messages
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="threadId">Contains the thread's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="messagesRepository">An instance to the messages repository</param>
        /// <returns>Thread information or error message</returns>
        [Authorize]
        [HttpPost("{threadId}/messages")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> ThreadMessages([FromBody] SearchDto searchDto, int threadId, Member memberInfo, IMessagesRepository messagesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Create parameters for messages request
            MessagesListDto messagesListDto = new() {
                ThreadId = threadId,
                Page = searchDto.Page,
                    MemberId = memberInfo.Info!.MemberId
            };

            // Get the messages
            ResponseDto<ElementsDto<MessageDto>> messagesList = await messagesRepository.MessagesListAsync(messagesListDto);

            // Verify if the messages exists
            if ( messagesList.Result != null ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    messagesList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadNotFound")
                });

            }

        }
        
    }

}