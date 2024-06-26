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

    // System Namespaces
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Settings;
    using Models.Entities.Settings;
    using Utils.General;
    using Utils.Interfaces.Repositories.Settings;

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
        /// <returns>Update response</returns>
        [Authorize]
        [HttpPost("update")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> UpdateOptions([FromBody] OptionsDto optionsDto) {

            // Get all options saved in the database
            ResponseDto<List<OptionDto>> savedOptions = await _settingsRepository.OptionsListAsync();

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
                    PropertyInfo? optionName = typeof(OptionsDto).GetProperty(savedOptions.Result[o].OptionName);

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
            PropertyInfo[] propertyInfos = typeof(OptionsDto).GetProperties();

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