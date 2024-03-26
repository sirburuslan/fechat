/*
 * @class Websites Delete Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the websites
 */

// Namespace for the User Websites Controllers
namespace FeChat.Controllers.User.Websites {

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use the Websites Repositories
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Websites Delete Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/websites")]
    public class DeleteController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public DeleteController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Delete a website
        /// </summary>
        /// <param name="websiteId">Contains the website's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="websitesRepository">An instance to the websites repository</param>
        /// <returns>Message about the website status</returns>
        [HttpDelete("{websiteId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> DeleteWebsite(int websiteId, Member memberInfo, IWebsitesRepository websitesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Delete a member
            ResponseDto<bool> deleteWebsite = await websitesRepository.DeleteWebsiteAsync(websiteId, memberInfo.Info!.MemberId);

            // Check if the website was deleted
            if ( deleteWebsite.Result ) {

                // Return success message
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("WebsiteWasDeleted")
                });                

            } else if ( deleteWebsite.Message != null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = deleteWebsite.Message
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteWasNotDeleted")
                });

            }

        } 

    }

}