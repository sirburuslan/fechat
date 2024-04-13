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

// Namespace for User Websites Controllers
namespace FeChat.Controllers.User.Websites {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;

    // App Namespaces
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Websites;
    using Utils.General;
    using Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Websites Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/websites")]
    public class ReadController: Controller {

        /// <summary>
        /// Get the list with websites
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="websitesRepository">An instance to the websites repository</param>
        /// <returns>List with websites or error messages</returns>
        [Authorize]
        [HttpPost("list")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> List([FromBody] SearchDto searchDto, Member memberInfo, IWebsitesRepository websitesRepository) {

            // Get all websites
            ResponseDto<ElementsDto<NewWebsiteDto>> websitesList = await websitesRepository.GetWebsitesAsync(memberInfo.Info!.MemberId, searchDto);

            // Verify if websites exists
            if ( websitesList.Result != null ) {

                // Return websites response
                return new JsonResult(new {
                    success = true,
                    websitesList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = websitesList.Message
                });

            }

        }

        /// <summary>
        /// Gets the website information
        /// </summary>
        /// <param name="websiteId">Contains the website's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="websitesRepository">An instance to the websites repository</param>
        /// <returns>Website information or error message</returns>
        [Authorize]
        [HttpGet("{websiteId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Website(int websiteId, Member memberInfo, IWebsitesRepository websitesRepository) {

            // Get the website's data
            ResponseDto<WebsiteDto> websiteDto = await websitesRepository.GetWebsiteAsync(memberInfo.Info!.MemberId, websiteId);

            // Verify if website exists
            if ( websiteDto.Result != null ) {

                // Create the website's information
                Dictionary<string, object> website = new() {

                    // Add Website id
                    { "WebsiteId", websiteDto.Result.WebsiteId.ToString() },

                    // Add Website Name
                    { "Name", websiteDto.Result.Name ?? string.Empty },

                    // Add Website Url
                    { "Url", websiteDto.Result.Url ?? string.Empty },

                    // Add Chat enabled
                    { "Enabled", websiteDto.Result.Enabled },

                    // Add Chat header
                    { "Header", websiteDto.Result.Header ?? string.Empty }                           

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