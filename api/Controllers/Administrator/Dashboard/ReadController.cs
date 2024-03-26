/*
 * @class Dashboard Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-10
 *
 * This class is used to get multiple db queries in one request for the administrator dashboard
 */

// Namespace for Administrator Dashboard Controllers
namespace FeChat.Controllers.Administrator.Dashboard {

    // Use LINQ
    using System.Linq;

    // Use the MVC to add controllers support
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Dtos classes
    using FeChat.Models.Dtos;

    // Use Events Dtos
    using FeChat.Models.Dtos.Events;

    // Use the Member Dtos classes
    using FeChat.Models.Dtos.Members;

    // Use the Transactions Dtos classes
    using FeChat.Models.Dtos.Transactions;

    // Use General utils
    using FeChat.Utils.General;

    // Use Events repositories
    using FeChat.Utils.Interfaces.Repositories.Events;
    
    // Use the Members repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the Subscriptions repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Read Controller for Dashboard
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/dashboard")]
    public class ReadController: Controller {

        /// <summary>
        /// Gets the data from database for the administrator dashboard
        /// </summary>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="subscriptionsRepository">An instance to the Subscriptions repository</param>
        /// <param name="eventsRepository">An instance to the Events repository</param>
        /// <returns>IActionResult with dashboard content</returns>
        [Authorize]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DashboardData(Member memberInfo, IMembersRepository membersRepository, ISubscriptionsRepository subscriptionsRepository, IEventsRepository eventsRepository) {

            // Create the dashboard list
            Dictionary<string, object> dashboard = new();

            // Get current time
            DateTime currentTime = DateTime.Now;                      

            // Specify the date and time
            DateTime dateTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day);

            // Convert to Unix timestamp
            int unixTimestamp = (int)((DateTimeOffset)dateTime).ToUnixTimeSeconds();

            // Gets the events from the database by time
            ResponseDto<List<EventDto>> eventsList = await eventsRepository.GetEventsAsync(0, unixTimestamp);

            // Verify if events exists
            if ( eventsList.Result != null ) {

                // Set events
                dashboard.Add("events", eventsList.Result);

            }

            // Default time
            int time = 30;

            // Get all members options
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Verify if options list is not null
            if ( optionsList.Result != null ) {

                // Get the chart time if exists
                string? chartTime = optionsList.Result.FirstOrDefault(o => o.OptionName == "MembersChartTime")?.OptionValue;

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

            // Get all members
            ResponseDto<object> membersList = await membersRepository.GetMembersByTimeAsync(time);

            // Verify if members exists
            if ( membersList.Result != null ) {

                // Set members to the dashboard list
                dashboard.Add("members", membersList.Result);

            }

            // Get the transactions
            ResponseDto<ElementsDto<TransactionDetailsDto>> transactionsList = await subscriptionsRepository.GetTransactionsByPageAsync(new SearchDto() {
                Page = 1
            }, null);

            // Verify if transactions exists
            if ( transactionsList.Result != null ) {

                // Set transactions to the dashboard list
                dashboard.Add("transactions", transactionsList.Result.Elements.Take(5));                

            }

            // Return dashboard
            return new JsonResult(new {
                success = true,
                dashboard,
                time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });

        }

        /// <summary>
        /// Gets the members from database for the administrator dashboard
        /// </summary>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>IActionResult with response</returns>
        [Authorize]
        [HttpGet("members")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DashboardMembers(Member memberInfo, IMembersRepository membersRepository) {

            // Default time
            int time = 30;

            // Get all members options
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Verify if options list is not null
            if ( optionsList.Result != null ) {

                // Get the chart time if exists
                string? chartTime = optionsList.Result.FirstOrDefault(o => o.OptionName == "MembersChartTime")?.OptionValue;

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

            // Get all members
            ResponseDto<object> membersList = await membersRepository.GetMembersByTimeAsync(time);          

            // Verify if members exists
            if ( membersList.Result != null ) {

                // Return members response
                return new JsonResult(new {
                    success = true,
                    members = membersList.Result
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = membersList.Message
                });

            }

        }

    }

}