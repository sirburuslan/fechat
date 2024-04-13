/*
 * @class Profile Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-09
 *
 * This class is used to read the user's profile
 */

// Namespace for User Profile Controllers
namespace FeChat.Controllers.User {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Utils.General;
    using Models.Dtos;
    using Models.Dtos.Members;
    using Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Members Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public class ReadController: Controller {

        /// <summary>
        /// Gets the member information
        /// </summary>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>IActionResult with member information</returns>
        [Authorize]
        [HttpGet("profile")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Profile(Member memberInfo, IMembersRepository membersRepository) { 

            // Get the member's data
            ResponseDto<MemberDto> Member = await membersRepository.GetMemberAsync(memberInfo.Info!.MemberId);

            // Verify if member exists
            if ( Member.Result != null ) {

                // Create the member's information
                Dictionary<string, string> member = new() {

                    // Add Member id
                    { "MemberId", memberInfo.Info!.MemberId.ToString() },

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
                ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

                // Verify if the options list exists
                if ( optionsList.Result != null ) {

                    // Get options length
                    int optionsLength = optionsList.Result.Count;

                    // List the options
                    for ( int o = 0; o < optionsLength; o++ ) {

                        // Add option
                        member.Add(optionsList.Result[o].OptionName, optionsList.Result[o].OptionValue);

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