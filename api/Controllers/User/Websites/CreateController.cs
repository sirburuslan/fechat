/*
 * @class Websites Create Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the websites
 */

// Namespace for the User Websites Controllers
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
    
    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Plans dtos
    using FeChat.Models.Dtos.Plans;

    // Use Subscriptions Dtos
    using FeChat.Models.Dtos.Subscriptions;

    // Use Websites Dtos
    using FeChat.Models.Dtos.Websites;

    // Use the Repositories for Plans
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use Repositories for Subscriptions
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    // Use the Repositories for Websites
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Plans Create Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/websites")]
    public class CreateController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public CreateController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Save a website
        /// </summary>
        /// <param name="websiteDto">Data transfer object with website information</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="subscriptionsRepository">Contains an instance to the Subscriptions repository</param>
        /// <param name="plansRepository">Contains an instance to the Plans repository</param>
        /// <param name="websitesRepository">Instance for the website repository</param>
        [Authorize]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreateWebsite([FromBody] NewWebsiteDto websiteDto, Member memberInfo, ISubscriptionsRepository subscriptionsRepository, IPlansRepository plansRepository, IWebsitesRepository websitesRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Default websites limit number container
            int websitesLimit = 0;

            SearchDto searchDto = new() {
                Page = 1,
                Search = ""
            };

            // Get all websites
            ResponseDto<ElementsDto<NewWebsiteDto>> websitesList = await websitesRepository.GetWebsitesAsync(memberInfo.Info!.MemberId, searchDto);

            // Total websites number
            int totalSavedWebsites = ( websitesList.Result != null )?websitesList.Result.Total:0;

            // Get the user's subscription
            ResponseDto<SubscriptionDto> subscriptionResponse = await subscriptionsRepository.GetSubscriptionByMemberIdAsync(memberInfo.Info!.MemberId);

            // Verify if subscription response exists
            if ( subscriptionResponse.Result != null ) {

                // Get the plan's restrictions
                ResponseDto<List<RestrictionDto>> savedRestrictions = await plansRepository.RestrictionsListAsync(subscriptionResponse.Result.PlanId);

                // Check if restrictions exists
                if ( savedRestrictions.Result != null ) {

                    // Create the plan's information
                    Dictionary<string, int> plan = new();                        

                    // Get restrictions length
                    int restrictionsLength = savedRestrictions.Result!.Count;

                    // List the saved restrictions
                    for ( int r = 0; r < restrictionsLength; r++ ) {

                        // Set restriction name
                        string restrictionName = savedRestrictions.Result[r].RestrictionName;
                        
                        // Add Restriction
                        plan.Add(restrictionName, savedRestrictions.Result[r].RestrictionValue);
                        
                    }

                    // Get Websites limit
                    plan.TryGetValue("Websites", out int websites);

                    // Verify if the plan has Websites restriction
                    if ( websites > 0 ) {

                        // Replace the websites limit
                        websitesLimit = websites - totalSavedWebsites;

                    }

                }

            }

            // Verify if the user could create more websites
            if ( websitesLimit < 1 ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("MaximumAllowedWebsitesReached")
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

            // Create a Uri object from the URL string
            Uri uri = new(websiteDto.Url);
            
            // Get the domain from the Uri object
            websiteDto.Domain = uri.Host;

            // Get website by domain
            ResponseDto<NewWebsiteDto> getWebsite = await websitesRepository.GetWebsiteByDomainAsync(websiteDto);

            // Check if website exists
            if ( getWebsite.Result != null ) {

                // Verify if website is owned by same user
                if ( memberInfo.Info!.MemberId == getWebsite.Result.MemberId ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("WebsiteAlreadySaved")
                    });

                }

            }

            // Add member's id to website
            websiteDto.MemberId = memberInfo.Info!.MemberId;

            // Save a website
            ResponseDto<NewWebsiteDto> saveWebsite = await websitesRepository.SaveAsync(websiteDto);

            // Verify if the website was saved
            if ( saveWebsite.Result != null ) {

                // Create a success response
                return new JsonResult(new {
                    success = true,
                    message = saveWebsite.Message
                });                

            } else {

                // Create a error response
                return new JsonResult(new {
                    success = false,
                    message = saveWebsite.Message
                });       

            }

        }

    }

}