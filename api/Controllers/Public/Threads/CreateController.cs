/*
 * @class Threads Create Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the threads for guests
 */

// Namespace for Public Threads Controllers
namespace FeChat.Controllers.Public.Threads {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Models.Dtos.Websites;
    using Utils.General;
    using Utils.Interfaces.Repositories.Messages;
    using Utils.Interfaces.Repositories.Settings;
    using Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Create Threads Controller
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/threads")]
    public class CreateController: Controller {

        /// <summary>
        /// Create a new thread
        /// </summary>
        /// <param name="newThreadDto">Message content</param>
        /// <param name="settingsRepository">An instance to the Settings repository</param>
        /// <param name="websitesRepository">An instance for the websites repository</param>
        /// <param name="messagesRepository">An instance for the messages repository</param>
        /// <returns>Success message or error message</returns>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreateThread([FromBody] NewThreadDto newThreadDto, ISettingsRepository settingsRepository, IWebsitesRepository websitesRepository, IMessagesRepository messagesRepository) {

            // Check if website id exists
            if ( newThreadDto.WebsiteId == 0 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteIdMissing")
                });
                
            }

            // Check if message has at least 3 characters
            if ( (newThreadDto.Message != null) && (newThreadDto.Message.Length < 2) ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("MessageTooShort")
                });

            }

            // Get website from the database
            ResponseDto<WebsiteDto> websiteResponse = await websitesRepository.GetWebsiteInfoAsync(newThreadDto.WebsiteId);

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

            // Get the options saved in the database
            ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await settingsRepository.OptionsListAsync();

            // Lets create a new dictionary list
            Dictionary<string, string> optionsList = new();            

            // Verify if options exists
            if ( savedOptions.Result != null ) {

                // Get options length
                int optionsLength = savedOptions.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Add option to the dictionary
                    optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                }

            }

            // Get the guest ip
            System.Net.IPAddress remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress!;

            // Request data from IP
            ResponseDto<IpDto> ipData = await new IpLookup().GetIpData(optionsList, remoteIpAddress.ToString());

            // Create the guest's data
            GuestDto guestDto = new() {
                Name = newThreadDto.Name,
                Email = newThreadDto.Email,
                Ip = remoteIpAddress.ToString(),
                Latitude = (ipData.Result != null)?ipData.Result.Latitude:"",
                Longitude = (ipData.Result != null)?ipData.Result.Longitude:""
            };

            // Save the guest's data
            ResponseDto<GuestDto> saveGuest = await messagesRepository.CreateGuestAsync(guestDto);

            // Check if the guest was saved
            if ( saveGuest.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("GuestNotCreated")
                });
                
            }

            // Create the thread information
            ThreadDto threadInfo = new() {
                MemberId = websiteResponse.Result.MemberId,
                WebsiteId = newThreadDto.WebsiteId,
                GuestId = saveGuest.Result.GuestId
            };

            // Create the thread
            ResponseDto<ThreadDto> createThread = await messagesRepository.CreateThreadAsync(threadInfo);

            // Check if error message exists
            if ( createThread.Result == null ) {

                // Return error message
                return new JsonResult(new {
                    success = false,
                    message = createThread.Message
                });   

            }

            // Create the message
            MessageDto messageDto = new() {
                ThreadId = createThread.Result.ThreadId,
                Message = newThreadDto.Message
            };

            // Save the message
            ResponseDto<MessageDto> responseDto = await messagesRepository.CreateMessageAsync(messageDto);

            // Verify if the messages was created
            if ( responseDto.Result != null ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    message = responseDto.Message,
                    thread = new {
                        createThread.Result.ThreadId,
                        createThread.Result.ThreadSecret
                    }
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = responseDto.Message
                });

            }

        }

    }

}