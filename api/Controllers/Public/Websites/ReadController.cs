/*
 * @class Websites Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the websites
 */

// Namespace for Public Websites Controllers
namespace FeChat.Controllers.Public.Websites {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Websites;
    using Utils.General;
    using Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Websites Read Controller
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/websites")]
    public class ReadController: Controller {

        /// <summary>
        /// Container for Websites Repository
        /// </summary>
        private readonly IWebsitesRepository _websitesRepository;

        /// <summary>
        /// Read Controller Constructor
        /// </summary>
        /// <param name="websitesRepository">An instance to the websites repository</param>
        public ReadController(IWebsitesRepository websitesRepository) {

            // Save website repository
            _websitesRepository = websitesRepository;

        }

        /// <summary>
        /// Gets the website information
        /// </summary>
        /// <param name="websiteId">Contains the website's ID</param>
        /// <returns>Website information or error message</returns>
        /// <returns>Website information or error message</returns>
        [AllowAnonymous]
        [HttpGet("{websiteId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> WebsiteInfo(int websiteId) {

            // Get the website's data
            ResponseDto<WebsiteDto> websiteDto = await _websitesRepository.GetWebsiteInfoAsync(websiteId);

            // Verify if website exists
            if ( websiteDto.Result != null ) {

                // Verify if the chat is disabled
                if ( websiteDto.Result.Enabled < 1 ) {

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("ChatDisabled")
                    });                

                }

                // Create the website's information
                Dictionary<string, object> website = new() {

                    // Add Website id
                    { "WebsiteId", websiteDto.Result.WebsiteId.ToString() },

                    // Add Website Name
                    { "Name", websiteDto.Result.Name ?? string.Empty },

                    // Add Website Url
                    { "Url", websiteDto.Result.Url ?? string.Empty },

                    // Add Website Domain
                    { "Domain", websiteDto.Result.Domain ?? string.Empty },                    

                    // Add Chat status
                    { "Enabled", websiteDto.Result.Enabled },

                    // Add Chat header
                    { "Header", websiteDto.Result.Header ?? string.Empty },

                    // Add Member First Name
                    { "FirstName", websiteDto.Result.FirstName ?? string.Empty },    
                    
                    // Add Member Last Name
                    { "LastName", websiteDto.Result.LastName ?? string.Empty }, 
                    
                    // Add Member Profile Photo
                    { "ProfilePhoto", websiteDto.Result.ProfilePhoto ?? string.Empty }                                                        

                };

                // Return a json
                return new JsonResult(new {
                    success = true,
                    website
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteNotFound")
                });

            }

        }

    }

}