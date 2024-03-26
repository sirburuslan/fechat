/*
 * @class Members Options Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the members options
 */

// Namespace for Administrator Members Options Controllers
namespace FeChat.Controllers.Administrator.Members.Options {

    // Use the .NET methods to get DTO properties
    using System.Reflection;

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use the entities
    using FeChat.Models.Entities.Members;

    // Use the general namespace fot strings
    using FeChat.Utils.General;    

    // Use the Repositories for database requests
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Members Options Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/members")]
    public class UpdateController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public UpdateController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Gets the member information
        /// </summary>
        /// <param name="optionsDto">Contains the member's options</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success message or error message for save changes</returns>
        [Authorize]
        [HttpPost("options")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Options([FromBody] OptionsDto optionsDto, Member memberInfo, IMembersRepository membersRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get all members options
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Options to update container
            List<MemberOptionsEntity> optionsUpdate = new();

            // Saved options names
            List<string> optionsSaved = new();

            // Check if options exists
            if ( optionsList.Result != null ) {

                // Get options length
                int optionsLength = optionsList.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Get option's name
                    var optionName = typeof(OptionsDto).GetProperty(optionsList.Result[o].OptionName);

                    // Verify if option's name is not null
                    if ( optionName != null ) {

                        // Verify if option name is plan id
                        if ( optionName.Name == "PlanId" ) {
                            continue;
                        }

                        // Get the option's value
                        var optionValue = optionName!.GetValue(optionsDto);

                        // Verify if optionValue is not null
                        if ( optionValue == null ) {
                            continue;
                        }

                        // Save the option's name
                        optionsSaved.Add(optionsList.Result[o].OptionName);

                        // Create the option's params
                        MemberOptionsEntity optionUpdate = new() {
                            OptionId = optionsList.Result[o].OptionId,
                            MemberId = optionsList.Result[o].MemberId,
                            OptionName = optionsList.Result[o].OptionName,
                            OptionValue = optionValue!.ToString() ?? string.Empty
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionUpdate);

                    }
                    
                }

            }

            // Options to save container
            List<MemberOptionsEntity> optionsSave = new();

            // Get all options dto properties
            PropertyInfo[] propertyInfos = typeof(OptionsDto).GetProperties();

            // Get properties length
            int propertiesLength = propertyInfos.Length;

            // List the properties
            for ( int p = 0; p < propertiesLength; p++ ) {

                // If is MemberId continue
                if ( propertyInfos[p].Name == "MemberId" ) {
                    continue;
                }

                // Verify if option name is plan id
                if ( propertyInfos[p].Name == "PlanId" ) {
                    continue;
                }

                // If value is null continue
                if ( propertyInfos[p].GetValue(optionsDto) == null ) {
                    continue;
                }

                // Check if the option is not saved already
                if ( !optionsSaved.Contains(propertyInfos[p].Name) ) {

                    // Create the option
                    MemberOptionsEntity option = new() {
                        MemberId = memberInfo.Info!.MemberId,
                        OptionName = propertyInfos[p].Name,
                        OptionValue = propertyInfos[p].GetValue(optionsDto)!.ToString()
                    };

                    // Add option to the save list
                    optionsSave!.Add(option);

                }

            }

            // Errors counter
            int errors = 0;

            // Verify if options for updating exists
            if ( optionsUpdate.Count > 0 ) {

                // Update options
                bool update_options = membersRepository.UpdateOptionsAsync(optionsUpdate);

                // Check if an error has been occurred when the options were updated
                if ( !update_options ) {
                    errors++;
                }
                
            } 

            // Verify if options for saving exists
            if ( optionsSave.Count > 0 ) {

                // Save options
                bool save_options = await membersRepository.SaveOptionsAsync(optionsSave);

                // Check if an error has been occurred when the options were saved
                if ( !save_options ) {
                    errors++;
                }

            }

            // Verify if no errors occurred
            if ( errors == 0 ) {

                // Create a success response
                var response = new {
                    success = true,
                    message = new Strings().Get("MembersSettingsUpdated")
                };

                // Return a json
                return new JsonResult(response);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = new Strings().Get("MembersSettingsNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            } 

        }

    }

}