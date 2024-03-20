/*
 * @class Settings Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the settings for website and member
 */

// Namespace for Controllers
namespace FeChat.Controllers {

    // Use the Dynamic classes
    using System.Dynamic;

    // Use Antiforgery for csrf generation
    using Microsoft.AspNetCore.Antiforgery;

    // Use the Authentication feature to get the access token
    using Microsoft.AspNetCore.Authentication;

    // Use Mvc for Controllers
    using Microsoft.AspNetCore.Mvc;

    // Use the cors
    using Microsoft.AspNetCore.Cors;

    // Add api version in url
    using Asp.Versioning;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use the Dtos for plans
    using FeChat.Models.Dtos.Plans;    

    // Use the Dtos for subscriptions
    using FeChat.Models.Dtos.Subscriptions;

    // Use General classes for strings
    using FeChat.Utils.General;

    // Use the Settings interfaces
    using FeChat.Utils.Interfaces.Repositories.Settings;

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;
    
    // Use the Members repository
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the Subscriptions Repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Settings Reader
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/settings")]
    public class SettingsController: Controller {

        /// <summary>
        /// Gets the settings for website and member
        /// </summary>
        /// <param name="settingsRepository">An instance for the settings repository</param>
        /// <param name="membersRepository">Session for members repository</param>
        /// <param name="subscriptionsRepository">An instance to the Subscription repository</param>
        /// <param name="plansRepository">Contains a session to the plans repository</param>
        /// <returns>List with settings</returns>
        [HttpGet]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetSettings(ISettingsRepository settingsRepository, IMembersRepository membersRepository, ISubscriptionsRepository subscriptionsRepository, IPlansRepository plansRepository) {

            // Get the options saved in the database
            ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await settingsRepository.OptionsListAsync();

            // Verify if options exists
            if ( savedOptions.Result != null ) {

                // Allowed options
                List<string> allowedOptions = new() {
                    "WebsiteName",
                    "DashboardLogoSmall",
                    "DashboardLogoLarge",
                    "SignInPageLogo",
                    "HomePageLogo",
                    "AnalyticsCode",
                    "RegistrationEnabled",
                    "PrivacyPolicy",
                    "Cookies",
                    "TermsOfService",
                    "DemoVideo",
                    "Ip2LocationEnabled",
                    "Ip2LocationKey",
                    "GoogleMapsEnabled",
                    "GoogleMapsKey",
                    "GoogleAuthEnabled",
                    "GoogleClientId",
                    "GoogleClientSecret",
                    "ReCAPTCHAEnabled",
                    "ReCAPTCHAKey"
                };

                // Lets create a new dictionary list
                Dictionary<string, string> optionsList = new();

                // Get options length
                int optionsLength = savedOptions.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Check if the option is allowed
                    if ( !allowedOptions.Contains(savedOptions.Result[o].OptionName) ) {
                        continue;
                    }

                    // Add option to the dictionary
                    optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                }

                // Create a success response
                dynamic response = new ExpandoObject();

                // Set response status
                response.success = true;

                // Set options
                response.options = optionsList;

                // Retrieve the access token from the HttpContext
                string accessToken = HttpContext.GetTokenAsync("access_token").Result ?? string.Empty;

                // Check if access token exists
                if ( accessToken != string.Empty ) {

                    // Get the member's ID
                    string MemberId = new Tokens().GetTokenData(accessToken ?? string.Empty, "MemberId");

                    // Verify if MemberId has value
                    if (MemberId != "") {

                        // Member the member's data
                        ResponseDto<MemberDto> Member = await membersRepository.GetMemberAsync(int.Parse(MemberId));

                        // Verify if member exists
                        if ( Member != null ) {

                            // Create the member's information
                            Dictionary<string, string> member = new() {

                                // Add Member id
                                { "MemberId", MemberId },

                                // Add First Name
                                { "FirstName", Member.Result!.FirstName ?? string.Empty },

                                // Add Last Name
                                { "LastName", Member.Result!.LastName ?? string.Empty },  
                                
                                // Add Email
                                { "Email", Member.Result!.Email ?? string.Empty },                       

                                // Add Role
                                { "Role", Member.Result!.Role.ToString() ?? string.Empty }           

                            };

                            // Check if role is user
                            if ( Member.Result!.Role.ToString() == "1" ) {

                                // Get the user's subscription
                                ResponseDto<SubscriptionDto> subscriptionResponse = await subscriptionsRepository.GetSubscriptionByMemberIdAsync(int.Parse(MemberId));

                                // Verify if subscription response exists
                                if ( subscriptionResponse.Result != null ) {

                                    // Add subscription plan id
                                    member.Add("SubscriptionPlanId", subscriptionResponse.Result.PlanId.ToString());

                                    // Convert Unix timestamp to DateTimeOffset
                                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(subscriptionResponse.Result.Expiration);

                                    // Add subscription expiration
                                    member.Add("SubscriptionExpiration", dateTimeOffset.DateTime.Day.ToString() + "/" + dateTimeOffset.DateTime.Month.ToString() + "/" + dateTimeOffset.DateTime.Year.ToString());

                                    // Get the plan's data
                                    ResponseDto<PlanDto> planDto = await plansRepository.GetPlanAsync(subscriptionResponse.Result.PlanId);

                                    // Verify if plan exists
                                    if ( planDto.Result != null ) {

                                        // Set plan name
                                        member.Add("SubscriptionPlanName", planDto.Result.Name!);

                                        // Set plan price
                                        member.Add("SubscriptionPlanPrice", planDto.Result.Price!);                                        

                                    }

                                }

                            }

                            // Get the member's settings
                            ResponseDto<List<OptionDto>> memberOptionsList = await membersRepository.OptionsListAsync(int.Parse(MemberId));

                            // Verify if the options list exists
                            if ( memberOptionsList.Result != null ) {

                                // Get options length
                                int memberOptionsLength = memberOptionsList.Result.Count;

                                // List the options
                                for ( int o = 0; o < memberOptionsLength; o++ ) {

                                    // Add option
                                    member.Add(memberOptionsList.Result[o].OptionName, memberOptionsList.Result[o].OptionValue);

                                }

                            }

                            // Add member to response
                            response.memberOptions = member;

                        }

                    }

                }

                // Return the response
                return new JsonResult(response);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = savedOptions.Message
                };

                // Return a json
                return new JsonResult(response);  

            }

        }
        
    }

}