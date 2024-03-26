/*
 * @class Members Delete Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the members accounts
 */

// Namespace for Administrator Members Controllers
namespace FeChat.Controllers.Administrator.Members {

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use the general namespace fot strings
    using FeChat.Utils.General;    

    // Use Repository for Events
    using FeChat.Utils.Interfaces.Repositories.Events;

    // Use Repository for Members
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use Repositories for Subscriptions
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    // Use Repositories for Websites
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// This controller gets the member's basic information
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/members")]
    public class DeleteController: Controller {

        /// <summary>
        /// Members Repository container
        /// </summary>
        private readonly IMembersRepository _membersRepository;

        /// <summary>
        /// Events Repository container
        /// </summary>
        private readonly IEventsRepository _eventsRepository;

        /// <summary>
        /// Subscriptions Repository container
        /// </summary>
        private readonly ISubscriptionsRepository _subscriptionsRepository;

        /// <summary>
        /// Websites Repository container
        /// </summary>
        private readonly IWebsitesRepository _websitesRepository;

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="eventsRepository">Contains an instance to the Events repository</param>
        /// <param name="subscriptionsRepository">Contains an instance to the Subscriptions repository</param>
        /// <param name="websitesRepository">Contains an instance to the Websites repository</param>
        /// <param name="configuration">App configuration</param>
        public DeleteController(IMembersRepository membersRepository, IEventsRepository eventsRepository, ISubscriptionsRepository subscriptionsRepository, IWebsitesRepository websitesRepository, IConfiguration configuration) {

            // Save members repository
            _membersRepository = membersRepository;

            // Save events repository
            _eventsRepository = eventsRepository;

            // Save subscriptions repository
            _subscriptionsRepository = subscriptionsRepository;            

            // Save websites repository
            _websitesRepository = websitesRepository;

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Delete a member
        /// </summary>
        /// <param name="memberId">Contains the member's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <returns>Message if the member was deleted or error</returns>
        [HttpDelete("{memberId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DeleteMember(int memberId, Member memberInfo) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Check if the administrator tries to delete his account
            if ( memberId == memberInfo.Info!.MemberId ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("AdministratorCanNotDeleteAccount")
                });
                
            }

            // Delete a member
            ResponseDto<bool> deleteMember = await _membersRepository.DeleteMemberAsync(memberId);

            // Check if the member was deleted
            if ( deleteMember.Result ) {

                // Delete the member's events
                await _eventsRepository.DeleteMemberEventsAsync(memberId);

                // Delete the member's subscriptions
                await _subscriptionsRepository.DeleteMemberSubscriptionsAsync(memberId);

                // Delete the member's transactions
                await _subscriptionsRepository.DeleteMemberTransactionsAsync(memberId);                

                // Delete the member's websites
                await _websitesRepository.DeleteMemberWebsitesAsync(memberId);

                // Return success message
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("MemberWasDeleted")
                });                

            } else if ( deleteMember.Message != null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = deleteMember.Message
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("MemberWasNotDeleted")
                });

            }

        } 

    }

}