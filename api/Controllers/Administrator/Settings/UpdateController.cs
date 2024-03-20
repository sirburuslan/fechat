/*
 * @class Settings Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the settings
 */

// Namespace fot Administrator Settings Controllers
namespace FeChat.Controllers.Administrator.Settings {

    // Use the Generic NET classes for dictionary
    using System.Collections.Generic;

    // Use the Reflection classes to get the dto properties
    using System.Reflection;

    // Use the Mvc to get the controller
    using Microsoft.AspNetCore.Mvc;

    // Use the Authentication feature to get the access token
    using Microsoft.AspNetCore.Authentication;

    // Use Authorization for access restriction
    using Microsoft.AspNetCore.Authorization;

    // Use Cors libraries
    using Microsoft.AspNetCore.Cors;

    // Use Antiforgery for CSRF protection
    using Microsoft.AspNetCore.Antiforgery;

    // Use the Versioning library
    using Asp.Versioning;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use the Settings entities
    using FeChat.Models.Entities.Settings;

    // Use General Utils
    using FeChat.Utils.General;  
    
    // Use the Settings Repositories interfaces
    using FeChat.Utils.Interfaces.Repositories.Settings;
    
    // Use the Repositories for Members
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Plans Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/settings")]
    public class UpdateController: Controller {

        // Settings Repository container
        ISettingsRepository _settingsRepository;

        /// <summary>
        /// Settings controller constructor
        /// </summary>
        /// <param name="settingsRepository">An instance for the settings repository</param>
        public UpdateController(ISettingsRepository settingsRepository) {

            // Set injected settings repository
            _settingsRepository = settingsRepository;

        }

        /// <summary>
        /// Update the options
        /// </summary>
        /// <param name="optionsDto">Options to update</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Update response</returns>
        [Authorize]
        [HttpPost("update")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateOptions([FromBody] Models.Dtos.Settings.OptionsDto optionsDto, IMembersRepository membersRepository) {

            // Get all options saved in the database
            ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await _settingsRepository.OptionsListAsync();

            // Options to update container
            List<SettingsEntity> optionsUpdate = new();

            // Saved options names
            List<string> optionsSaved = new();

            // Check if options exists
            if ( savedOptions.Result != null ) {

                // Get options length
                int optionsLength = savedOptions.Result!.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Get option's name
                    PropertyInfo? optionName = typeof(Models.Dtos.Settings.OptionsDto).GetProperty(savedOptions.Result[o].OptionName);

                    // Verify if option's name is not null
                    if ( optionName != null ) {

                        // Save the option's name
                        optionsSaved.Add(savedOptions.Result[o].OptionName);

                        // Get the option's value
                        string optionValue = optionName!.GetValue(optionsDto)!.ToString() ?? string.Empty;

                        // Create the option's params
                        SettingsEntity optionUpdate = new() {
                            OptionId = savedOptions.Result[o].OptionId,
                            OptionName = savedOptions.Result[o].OptionName,
                            OptionValue = optionValue.Trim()
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionUpdate);

                    }
                    
                }

            }

            // Options to save container
            List<SettingsEntity> optionsSave = new();

            // Get all options dto properties
            PropertyInfo[] propertyInfos = typeof(Models.Dtos.Settings.OptionsDto).GetProperties();

            // Get properties length
            int propertiesLength = propertyInfos.Length;

            // List the properties
            for ( int p = 0; p < propertiesLength; p++ ) {

                // If is MemberId continue
                if ( propertyInfos[p].Name == "MemberId" ) {
                    continue;
                }

                // If value is null continue
                if ( propertyInfos[p].GetValue(optionsDto) == null ) {
                    continue;
                }

                // Check if the option is not saved already
                if ( !optionsSaved.Contains(propertyInfos[p].Name) ) {

                    // Create the option
                    SettingsEntity option = new() {
                        OptionName = propertyInfos[p].Name,
                        OptionValue = propertyInfos[p].GetValue(optionsDto)!.ToString()!.Trim()
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
                bool update_options = _settingsRepository.UpdateOptionsAsync(optionsUpdate);

                // Check if an error has been occurred when the options were updated
                if ( !update_options ) {
                    errors++;
                }
                
            } 

            // Verify if options for saving exists
            if ( optionsSave.Count > 0 ) {

                // Save options
                bool save_options = await _settingsRepository.SaveOptionsAsync(optionsSave);

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