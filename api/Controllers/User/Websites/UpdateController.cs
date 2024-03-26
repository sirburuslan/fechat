/*
 * @class Websites Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the websites
 */

// Namespace for User Websites Controllers
namespace FeChat.Controllers.User.Websites {

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use Websites dtos classes
    using FeChat.Models.Dtos.Websites;

    // Use the Websites Repositories
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Websites Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/websites")]
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
        /// Update the website informations
        /// </summary>
        /// <param name="websiteDto">Data transfer object with website information</param>
        /// <param name="websiteId">Contains the website's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="websitesRepository">Instance for the website repository</param>
        [Authorize]
        [HttpPost("{websiteId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> UpdateWebsite([FromBody] NewWebsiteDto websiteDto, int websiteId, Member memberInfo, IWebsitesRepository websitesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Verify if website name is required
            if ( websiteDto.Name == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteNameRequired")
                });
                
            }

            // Verify if website url is required
            if ( websiteDto.Url == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteUrlRequired")
                });
                
            }   

            // Set Website Id
            websiteDto.WebsiteId = websiteId;            

            // Set Member Id
            websiteDto.MemberId = memberInfo.Info!.MemberId;

            // Create a Uri object from the URL string
            Uri uri = new(websiteDto.Url);
            
            // Get the domain from the Uri object
            websiteDto.Domain = uri.Host;

            // Get website by domain
            ResponseDto<NewWebsiteDto> getWebsite = await websitesRepository.GetWebsiteByDomainAsync(websiteDto);

            // Check if website exists
            if ( getWebsite.Result != null ) {

                // Verify if website is owned by same user
                if ( (memberInfo.Info!.MemberId == getWebsite.Result.MemberId) && (websiteId != getWebsite.Result.WebsiteId) ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("WebsiteAlreadySaved")
                    });

                }

            }

            // Update the website's data
            ResponseDto<bool> UpdateWebsite = await websitesRepository.UpdateWebsiteAsync(websiteDto);

            // Verify the website was updated
            if ( UpdateWebsite.Result ) {
                
                // Create a success response
                var updateResponse = new {
                    success = true,
                    message = new Strings().Get("WebsiteWasUpdated")
                };

                // Return a json
                return new JsonResult(updateResponse);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = UpdateWebsite.Message ?? new Strings().Get("WebsiteWasNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            }

        }

        /// <summary>
        /// Update the website chat settings
        /// </summary>
        /// <param name="websiteDto">Data transfer object with website information</param>
        /// <param name="websiteId">Contains the website's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="websitesRepository">Instance for the website repository</param>
        [Authorize]
        [HttpPost("{websiteId}/chat")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> UpdateChat([FromBody] NewWebsiteDto websiteDto, int websiteId, Member memberInfo, IWebsitesRepository websitesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Set Website Id
            websiteDto.WebsiteId = websiteId;            

            // Set Member Id
            websiteDto.MemberId = memberInfo.Info!.MemberId;

            // Update the chat's data
            ResponseDto<bool> UpdateChat = await websitesRepository.UpdateChatAsync(websiteDto);

            // Verify the chat was updated
            if ( UpdateChat.Result ) {
                
                // Create a success response
                var updateResponse = new {
                    success = true,
                    message = new Strings().Get("ChatWasUpdated")
                };

                // Return a json
                return new JsonResult(updateResponse);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = UpdateChat.Message ?? new Strings().Get("ChatWasNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            }

        }

    }

}