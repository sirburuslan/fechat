/*
 * @class Settings Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the settings
 */

// Namespace fot Administrator Settings Controllers
namespace FeChat.Controllers.Administrator.Settings {

    // System Namespaces
    using System.Collections.Generic;
    using System.Dynamic;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Cors;    
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Utils.General;
    using Utils.Interfaces.Repositories.Settings;
    using Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Plans Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/settings")]
    public class ReadController: Controller {

        // Settings Repository container
        ISettingsRepository _settingsRepository;

        /// <summary>
        /// Settings controller constructor
        /// </summary>
        /// <param name="settingsRepository">An instance for the settings repository</param>
        public ReadController(ISettingsRepository settingsRepository) {

            // Set injected settings repository
            _settingsRepository = settingsRepository;

        }

        /// <summary>
        /// Get the list with options
        /// </summary>
        /// <param name="membersRepository">Session for members repository</param>
        /// <returns>Requested options</returns>
        [HttpGet("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> OptionsList(IMembersRepository membersRepository) {

            // Get the options saved in the database
            ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await _settingsRepository.OptionsListAsync();

            // Verify if options exists
            if ( savedOptions.Result != null ) {

                // Allowed options
                List<string> allowedOptions = new() {
                    "WebsiteName",
                    "DashboardLogoSmall",
                    "DashboardLogoLarge",
                    "SignInPageLogo"
                };

                // Lets create a new dictionary list
                Dictionary<string, string> optionsList = new();

                // Get options length
                int optionsLength = savedOptions.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Check if the option is allowed
                    if ( !allowedOptions.Contains(savedOptions.Result[o].OptionName) ) {
                        continue;
                    }

                    // Add option to the dictionary
                    optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                }

                // Create a success response
                dynamic response = new ExpandoObject();

                // Set response status
                response.success = true;

                // Set options
                response.options = optionsList;

                // Retrieve the access token from the HttpContext
                string accessToken = HttpContext.GetTokenAsync("access_token").Result ?? string.Empty;

                // Check if access token exists
                if ( accessToken != string.Empty ) {

                    // Get the member's ID
                    string MemberId = new Tokens().GetTokenData(accessToken ?? string.Empty, "MemberId");

                    // Verify if MemberId has value
                    if (MemberId != "") {

                        // Member the member's data
                        ResponseDto<MemberDto> Member = await membersRepository.GetMemberAsync(int.Parse(MemberId));

                        // Verify if member exists
                        if ( Member != null ) {

                            // Create the member's information
                            Dictionary<string, string> member = new() {

                                // Add Member id
                                { "MemberId", MemberId },

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
                            ResponseDto<List<OptionDto>> memberOptionsList = await membersRepository.OptionsListAsync(int.Parse(MemberId));

                            // Verify if the options list exists
                            if ( memberOptionsList.Result != null ) {

                                // Get options length
                                int memberOptionsLength = memberOptionsList.Result.Count;

                                // List the options
                                for ( int o = 0; o < memberOptionsLength; o++ ) {

                                    // Add option
                                    member.Add(memberOptionsList.Result[o].OptionName, memberOptionsList.Result[o].OptionValue);

                                }

                            }

                            // Add member to response
                            response.memberOptions = member;

                        }

                    }

                }

                // Return the response
                return new JsonResult(response);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = savedOptions.Message
                };

                // Return a json
                return new JsonResult(response);  

            }

        }

        /// <summary>
        /// Get the list with options
        /// </summary>
        /// <returns>Requested options</returns>
        [HttpPost("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> AllOptions() {

            // Get the options saved in the database
            ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await _settingsRepository.OptionsListAsync();

            // Verify if options exists
            if ( savedOptions.Result != null ) {

                // Lets create a new dictionary list
                Dictionary<string, string> optionsList = new();

                // Get options length
                int optionsLength = savedOptions.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Add option to the dictionary
                    optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                }

                // Create a success response
                var response = new {
                    success = true,
                    options = optionsList
                };

                // Return a json
                return new JsonResult(response);  

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = savedOptions.Message
                };

                // Return a json
                return new JsonResult(response);  

            }

        }

    }

}