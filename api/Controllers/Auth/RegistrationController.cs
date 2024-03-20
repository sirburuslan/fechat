/*
 * @class Registration Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-18
 *
 * This class is used for users registration
 */

// Namespace for Auth Controllers
namespace FeChat.Controllers.Auth {

    // Use the Mvc to get the controller
    using Microsoft.AspNetCore.Mvc;

    // Import the Versioning library
    using Asp.Versioning;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use Dtos for Members
    using FeChat.Models.Dtos.Members;

    // Use General Utils
    using FeChat.Utils.General;

    // Use the Events Repositories
    using FeChat.Utils.Interfaces.Repositories.Events;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the Settings Repositories
    using FeChat.Utils.Interfaces.Repositories.Settings;

    /// <summary>
    /// This controller is used to create new accounts
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth/[controller]")]
    public class RegistrationController : Controller {

        /// <summary>
        /// This method validates the member's data and creates an account
        /// </summary>
        /// <param name="newMemberDto">Data transfer object with member information</param>
        /// <param name="settingsRepository">An instance for the settings repository</param>
        /// <param name="membersRepository">Contains a session to the Members repository</param>
        /// <param name="eventsRepository">Contains a session to the Events repository</param>
        /// <returns>Success or error message</returns>
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Registration([FromBody] NewMemberDto newMemberDto, ISettingsRepository settingsRepository, IMembersRepository membersRepository, IEventsRepository eventsRepository) {

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

            // Get registration status
            optionsList.TryGetValue("RegistrationEnabled", out string? RegistrationEnabled);

            // Verify if the registration is enabled
            if ( RegistrationEnabled != "1" ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("RegistrationDisabled")
                });

            }

            // Set user role
            newMemberDto.Role = 1;

            // Create member
            ResponseDto<MemberDto> createMember = await membersRepository.CreateMemberAsync(newMemberDto);

            // Verify if the account was created
            if ( createMember.Result != null ) {

                // Save event
                await eventsRepository.CreateEventAsync(createMember.Result.MemberId, 1);

                // Create a success response
                var response = new {
                    success = true,
                    message = createMember.Message
                };

                // Return a json
                return new JsonResult(response);                

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = createMember.Message
                };

                // Return a json
                return new JsonResult(response);       

            }

        }

    }
    
}