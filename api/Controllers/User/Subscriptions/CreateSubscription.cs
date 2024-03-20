/*
 * @class Subscriptions Create Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create subscriptions
 */

// Namespace for User Subsciptions Controllers
namespace FeChat.Controllers.User.Subscriptions {

    // Use the Mvc classes
    using Microsoft.AspNetCore.Mvc;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Authorization feature to allow guests access
    using Microsoft.AspNetCore.Authorization;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use Subscriptions Dtos
    using FeChat.Models.Dtos.Subscriptions;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Plans Dtos
    using FeChat.Models.Dtos.Plans;

    // Use Transactions Dtos
    using FeChat.Models.Dtos.Transactions;

    // Use General Utils
    using FeChat.Utils.General;

    // Use the Events Repositories
    using FeChat.Utils.Interfaces.Repositories.Events;

    // Use the Plans Repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Gets the Settings Repositories
    using FeChat.Utils.Interfaces.Repositories.Settings;

    // Use the Subscriptions Repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Create Subscription Controller
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/subscriptions")]
    public class CreateSubscription: Controller {

        /// <summary>
        /// Create Subscriptions
        /// </summary>
        /// <param name="newSubscriptionDto">Subscriptions Details</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="settingsRepository">An instance to the Settings repository</param>
        /// <param name="plansRepository">An instance to the Plans repository</param>
        /// <param name="subscriptionsRepository">An instance to the Subscription repository</param>
        /// <param name="eventsRepository">Contains a session to the Events repository</param>
        /// <returns>Success or error message</returns>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([FromBody] NewSubscriptionDto newSubscriptionDto, Member memberInfo, ISettingsRepository settingsRepository, IPlansRepository plansRepository, ISubscriptionsRepository subscriptionsRepository, IEventsRepository eventsRepository) {

            try {

                // Get the plan's data
                ResponseDto<PlanDto> planDto = await plansRepository.GetPlanAsync(newSubscriptionDto.PlanId);

                // Verify if the plan exists
                if ( planDto.Result == null ) {

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("PlanNotFound")
                    });

                }

                // Get the options saved in the database
                ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await settingsRepository.OptionsListAsync();

                // Lets create a new dictionary list
                Dictionary<string, string> optionsList = new();            

                // Verify if options exists
                if ( savedOptions.Result != null ) {

                    // Get options length
                    int optionsLength = savedOptions.Result.Count;

                    // List the saved options
                    for ( int o = 0; o < optionsLength; o++ ) {

                        // Add option to the dictionary
                        optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                    }

                }

                // Get the meta saved in the database
                ResponseDto<List<PlanMetaDto>> savedMeta = await plansRepository.MetaListAsync(newSubscriptionDto.PlanId);

                // Lets create a new dictionary list
                Dictionary<string, string> metaList = new();            

                // Verify if meta exists
                if ( savedMeta.Result != null ) {

                    // Get meta length
                    int metaLength = savedMeta.Result.Count;

                    // List the saved meta
                    for ( int m = 0; m < metaLength; m++ ) {

                        // Add meta to the dictionary
                        metaList.Add(savedMeta.Result[m].MetaName, savedMeta.Result[m].MetaValue!);

                    }

                }

                // Check if source is PayPal
                if ( newSubscriptionDto.Source == "PayPal" ) {

                    // Get PayPal status
                    optionsList.TryGetValue("PayPalEnabled", out string? paypalStatus);

                    // Check if PayPal is enabled
                    if ( paypalStatus != "1" ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("PayPalNotConfigured")
                        });

                    }

                    // Get the PayPal Client Id
                    optionsList.TryGetValue("PayPalClientId", out string? payPalClientId);

                    // Generate access token
                    RestResponseDto tokenResponse = await new Utils.Gateways.PayPal.Tokens().GenerateAsync(optionsList);

                    // Verify if the response failed
                    if ( tokenResponse.Success == false ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = tokenResponse.Message
                        });
                        
                    }

                    // Get Subscription details
                    RestResponseDto subscriptionResponse = await new Utils.Gateways.PayPal.Subscriptions.ReadSubscription().GetAsync(optionsList, tokenResponse.Data!, newSubscriptionDto.SubscriptionId);

                    // Verify if subscription exists
                    if ( subscriptionResponse.Success == false ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = subscriptionResponse.Message
                        });
                        
                    }

                    // Decode the Response
                    dynamic responsePlanDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(subscriptionResponse.Data!)!;

                    // Get start time
                    string startTime = responsePlanDecode.start_time;

                    // Split time by empty space
                    string[] timeSpace = startTime.Split(" ");

                    // Split time by slash
                    string[] timeSlash = timeSpace[0].Split("/");

                    // Split time by dots
                    string[] timeDots = timeSpace[1].Split(":");               

                    // Specify the date and time
                    DateTimeOffset dateTime = new (int.Parse(timeSlash[2]), int.Parse(timeSlash[0]), int.Parse(timeSlash[1]), int.Parse(timeDots[0]), int.Parse(timeDots[1]), int.Parse(timeDots[2]), TimeSpan.Zero);

                    // Convert to Unix timestamp
                    long unixTimestamp = dateTime.ToUnixTimeSeconds();

                    // Check if the subscription was created more than a day ago
                    if ( (unixTimestamp - (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()) / 1000 > 86400 ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("SubscriptionIdNotValid")
                        });
                        
                    }
                    
                    // Get the PayPal Plan Id
                    metaList.TryGetValue("PayPalPlanId", out string? payPalPlanId);

                    // Verify if the plan is correct
                    if ( responsePlanDecode.plan_id != payPalPlanId ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("SubscriptionIdNotValid")
                        });
                        
                    }

                    // Get billing time
                    string billingTime = responsePlanDecode.billing_info.next_billing_time;

                    // Split time by empty space
                    string[] billingSpace = billingTime.Split(" ");

                    // Split time by slash
                    string[] billingSlash = billingSpace[0].Split("/");

                    // Split time by dots
                    string[] billingDots = billingSpace[1].Split(":");               

                    // Specify the date and time
                    DateTimeOffset billingOffset = new (int.Parse(billingSlash[2]), int.Parse(billingSlash[0]), int.Parse(billingSlash[1]), int.Parse(billingDots[0]), int.Parse(billingDots[1]), int.Parse(billingDots[2]), TimeSpan.Zero);

                    // Convert to Unix timestamp
                    long nextBillingTime = billingOffset.ToUnixTimeSeconds();

                    // Get already existing subscription
                    ResponseDto<SubscriptionDto> memberSubscription = await subscriptionsRepository.GetSubscriptionByMemberIdAsync(memberInfo.Info!.MemberId);

                    // Save event
                    await eventsRepository.CreateEventAsync(memberInfo.Info.MemberId, 2);

                    // Verify if the member has no a subscription
                    if ( memberSubscription.Result == null ) {

                        // Create the subscription data for saving
                        SubscriptionDto subscriptionDto = new() {
                            MemberId = memberInfo.Info.MemberId,
                            PlanId = newSubscriptionDto.PlanId,
                            OrderId = newSubscriptionDto.OrderId,
                            NetId = newSubscriptionDto.SubscriptionId,
                            Source = "PayPal",
                            Expiration = (int)nextBillingTime
                        };

                        // Save the subscription
                        ResponseDto<SubscriptionDto> subscriptionCreate = await subscriptionsRepository.CreateSubscriptionAsync(subscriptionDto);

                        // Verify if has been occurred an error
                        if ( subscriptionCreate.Result == null ) {

                            // Return a json
                            return new JsonResult(new {
                                success = false,
                                message = subscriptionCreate.Message
                            });
                            
                        }

                        // Create a transaction
                        TransactionDto transactionDto = new() {
                            MemberId = memberInfo.Info.MemberId,
                            SubscriptionId = subscriptionCreate.Result.SubscriptionId,
                            PlanId = newSubscriptionDto.PlanId,
                            OrderId = newSubscriptionDto.OrderId,
                            NetId = newSubscriptionDto.SubscriptionId,
                            Source = "PayPal"
                        };

                        // Save transaction
                        await subscriptionsRepository.CreateTransactionAsync(transactionDto);

                    } else {
                        
                        // Verify if previous subscription was made with PayPal
                        if ( memberSubscription.Result.Source == "PayPal" ) {

                            // Cancel the subscription
                            await new Utils.Gateways.PayPal.Subscriptions.CancelSubscription().CancelAsync(optionsList, tokenResponse.Data!, memberSubscription.Result.NetId!);

                        }

                        // Create the subscription data for updating
                        SubscriptionDto subscriptionDto = new() {
                            SubscriptionId = memberSubscription.Result.SubscriptionId,
                            MemberId = memberSubscription.Result.MemberId,
                            PlanId = newSubscriptionDto.PlanId,
                            OrderId = newSubscriptionDto.OrderId,
                            NetId = newSubscriptionDto.SubscriptionId,
                            Source = "PayPal",
                            Expiration = (int)nextBillingTime,
                            Created = memberSubscription.Result.Created
                        };

                        // Update the subscription
                        ResponseDto<bool> subscriptionUpdate = await subscriptionsRepository.UpdateSubscriptionAsync(subscriptionDto);

                        // Verify if the subscription was updated
                        if ( !subscriptionUpdate.Result ) {

                            // Return a json
                            return new JsonResult(new {
                                success = false,
                                message = subscriptionUpdate.Message
                            });

                        }

                        // Create a transaction
                        TransactionDto transactionDto = new() {
                            MemberId = memberInfo.Info.MemberId,
                            SubscriptionId = memberSubscription.Result.SubscriptionId,
                            PlanId = newSubscriptionDto.PlanId,
                            OrderId = newSubscriptionDto.OrderId,
                            NetId = newSubscriptionDto.SubscriptionId,
                            Source = "PayPal"
                        };

                        // Save transaction
                        await subscriptionsRepository.CreateTransactionAsync(transactionDto);

                    }

                } else {

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("PaymentSourceNotValid")
                    });
                    
                }

                // Return a json
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("SubscriptionUpdated")
                });

            } catch ( Exception ex ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = ex.Message
                });

            }

        }

    }

}