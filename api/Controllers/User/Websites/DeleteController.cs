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

    // System Namespaces
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Utils.General;
    using Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Websites Delete Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/websites")]
    public class DeleteController: Controller {

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