/*
 * @class Dashboard Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to get multiple db queries in one request for the user dashboard
 */

// Namespace for User Dashboard Controllers
namespace FeChat.Controllers.User.Dashboard {

    // System Namespaces
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Utils.General;
    using Utils.Interfaces.Repositories.Members;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Read Controller for Dashboard
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/dashboard")]
    public class ReadController: Controller {
        
        /// <summary>
        /// Gets the data from database for the user dashboard
        /// </summary>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="messagesRepository">Contains an instance to the Messages repository</param>
        /// <returns>IActionResult with response</returns>
        [Authorize]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DashboardData(Member memberInfo, IMembersRepository membersRepository, IMessagesRepository messagesRepository) {

            // Default time
            int time = 30;

            // Get all threads options
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Verify if options list is not null
            if ( optionsList.Result != null ) {

                // Get the chart time if exists
                string? chartTime = optionsList.Result.FirstOrDefault(o => o.OptionName == "ThreadsChartTime")?.OptionValue;

                // Check if chart time is not null
                if ( chartTime != null ) {

                    switch (chartTime) {

                        case "1":
                            time = 30;
                            break;
    
                        case "2":
                            time = 60;
                            break;

                        case "3":
                            time = 180;
                            break;

                        case "4":
                            time = 360;
                            break;

                    }

                }

            }

            // Get all threads
            ResponseDto<object> threadsList = await messagesRepository.GetThreadsByTimeAsync(memberInfo.Info.MemberId, time);          

            // Verify if threads exists
            if ( threadsList.Result != null ) {

                // Return threads response
                return new JsonResult(new {
                    success = true,
                    threads = threadsList.Result.GetType().GetProperty("Threads")!.GetValue(threadsList.Result, null)
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
        /// Gets the threads from database for the administrator dashboard
        /// </summary>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="messagesRepository">Contains an instance to the Messages repository</param>
        /// <returns>IActionResult with response</returns>
        [Authorize]
        [HttpGet("threads")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DashboardThreads(Member memberInfo, IMembersRepository membersRepository, IMessagesRepository messagesRepository) {

            // Default time
            int time = 30;

            // Get all members options
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Verify if options list is not null
            if ( optionsList.Result != null ) {

                // Get the chart time if exists
                string? chartTime = optionsList.Result.FirstOrDefault(o => o.OptionName == "ThreadsChartTime")?.OptionValue;

                // Check if chart time is not null
                if ( chartTime != null ) {

                    switch (chartTime) {

                        case "1":
                            time = 30;
                            break;
    
                        case "2":
                            time = 60;
                            break;

                        case "3":
                            time = 180;
                            break;

                        case "4":
                            time = 360;
                            break;

                    }

                }

            }

            // Get all threads
            ResponseDto<object> threadsList = await messagesRepository.GetThreadsByTimeAsync(memberInfo.Info.MemberId, time);          

            // Verify if threads exists
            if ( threadsList.Result != null ) {

                // Return threads response
                return new JsonResult(new {
                    success = true,
                    threads = threadsList.Result.GetType().GetProperty("Threads")!.GetValue(threadsList.Result, null)
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = threadsList.Message
                });

            }

        }

    }

}