/*
 * @class Events Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This class is used to get read events
 */

// Namespace for Administrator Events Controllers
namespace FeChat.Controllers.Administrator.Events {

    // Use the MVC features
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Dtos
    using FeChat.Models.Dtos;    

    // Use the Events Dtos
    using FeChat.Models.Dtos.Events;

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;    

    // Use the Events repositories
    using FeChat.Utils.Interfaces.Repositories.Events;

    // Use Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Read Events Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/events")]
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
        /// Get the list with events
        /// </summary>
        /// <param name="eventsSearchDto">Events search parameters</param>
        /// <param name="membersRepository">An instance to the members repository</param>
        /// <param name="eventsRepository">An instance to the events repository</param>
        /// <returns>Events list or error message</returns>
        [Authorize]
        [HttpPost("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> List([FromBody] EventsSearchDto eventsSearchDto, IMembersRepository membersRepository, IEventsRepository eventsRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Prepare year
            int year = (eventsSearchDto.Year != null)?int.Parse(eventsSearchDto.Year):0;

            // Prepare month
            int month = (eventsSearchDto.Month != null)?int.Parse(eventsSearchDto.Month):0;
            
            // Prepare date
            int date = (eventsSearchDto.Date != null)?int.Parse(eventsSearchDto.Date):0;                        

            // Specify the date and time
            DateTime dateTime = new DateTime(year, month + 1, date);

            // Convert to Unix timestamp
            int unixTimestamp = (int)((DateTimeOffset)dateTime).ToUnixTimeSeconds();

            // Gets the events from the database by time
            ResponseDto<List<EventDto>> eventsList = await eventsRepository.GetEventsAsync(eventsSearchDto.MemberId, unixTimestamp);

            // Verify if events exists
            if ( eventsList.Result != null ) {

                // Return events list
                return new JsonResult(new {
                    success = true,
                    events = eventsList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });                

            }

            // Return error response
            return new JsonResult(new {
                success = false,
                message = eventsList.Message
            });

        }

    }

}