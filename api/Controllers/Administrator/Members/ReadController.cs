/*
 * @class Members Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the members
 */

// Namespace for Administrator Members Read Controllers
namespace FeChat.Controllers.Administrator.Members {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;
    
    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Models.Dtos.Plans;
    using Models.Dtos.Subscriptions;
    using Utils.General;
    using Utils.Interfaces.Repositories.Members;
    using Utils.Interfaces.Repositories.Plans;
    using Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Members Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/members")]
    public class ReadController: Controller {

        /// <summary>
        /// Get the list with members
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="membersRepository">An instance to the members repository</param>
        /// <returns>List with members or error message</returns>
        [Authorize]
        [HttpPost("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> List([FromBody] SearchDto searchDto, IMembersRepository membersRepository) {

            // Get all members
            ResponseDto<ElementsDto<MemberDto>> membersList = await membersRepository.GetMembersAsync(searchDto);

            // Verify if members exists
            if ( membersList.Result != null ) {

                // Return members response
                return new JsonResult(new {
                    success = true,
                    membersList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = membersList.Message
                });

            }

        }

        /// <summary>
        /// Get the list with members to export
        /// </summary>
        /// <param name="membersRepository">An instance to the members repository</param>
        /// <returns>Exports the members list or error message</returns>
        [Authorize]
        [HttpPost("export")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Export(IMembersRepository membersRepository) {

            // Get all members
            ResponseDto<List<MemberDto>> membersList = await membersRepository.GetMembersForExportAsync();

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

        /// <summary>
        /// Gets the member information
        /// </summary>
        /// <param name="MemberId">Contains the member's ID</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="subscriptionsRepository">Contains an instance to the Subscriptions repository</param>
        /// <param name="plansRepository">Plans repository instance</param>
        /// <returns>Member information or error message</returns>
        [Authorize]
        [HttpGet("{MemberId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Member(int MemberId, IMembersRepository membersRepository, ISubscriptionsRepository subscriptionsRepository, IPlansRepository plansRepository) {

            // Get the member's data
            ResponseDto<MemberDto> Member = await membersRepository.GetMemberAsync(MemberId);

            // Verify if member exists
            if ( Member.Result != null ) {

                // Create the member's information
                Dictionary<string, string> member = new() {

                    // Add Member id
                    { "MemberId", MemberId.ToString() },

                    // Add First Name
                    { "FirstName", Member.Result!.FirstName ?? string.Empty },

                    // Add Last Name
                    { "LastName", Member.Result!.LastName ?? string.Empty },  
                    
                    // Add Email
                    { "Email", Member.Result!.Email ?? string.Empty },                       

                    // Add Role
                    { "Role", Member.Result!.Role.ToString() ?? string.Empty }           

                };

                // Get the member's settings
                ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(MemberId);

                // Verify if the options list exists
                if ( optionsList.Result != null ) {

                    // Get options length
                    int optionsLength = optionsList.Result.Count;

                    // List the options
                    for ( int o = 0; o < optionsLength; o++ ) {

                        // Plan ID is not allowed
                        if ( optionsList.Result[o].OptionName == "PlanId" ) {
                            continue;
                        }

                        // Add option
                        member.Add(optionsList.Result[o].OptionName, optionsList.Result[o].OptionValue);

                    }

                    // Get the user's subscription
                    ResponseDto<SubscriptionDto> subscriptionResponse = await subscriptionsRepository.GetSubscriptionByMemberIdAsync(MemberId);

                    // Verify if subscription exists
                    if ( subscriptionResponse.Result != null ) {

                        // Get the plan's data
                        ResponseDto<PlanDto> planDto = await plansRepository.GetPlanAsync(subscriptionResponse.Result.PlanId);

                        // Verify if plan exists
                        if ( planDto.Result != null ) {

                            // Add plan's id
                            member.Add("PlanId", subscriptionResponse.Result.PlanId.ToString());                            

                            // Add plan's Name
                            member.Add("PlanName", planDto.Result.Name ?? string.Empty);

                        }

                    }

                }

                // Return a json
                return new JsonResult(new {
                    success = true,
                    member
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("AccountNotFound")
                });

            }

        }

    }

}