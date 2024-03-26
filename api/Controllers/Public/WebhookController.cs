/*
 * @class Webhook Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-20
 *
 * This class is used to handle the paypal webhooks requests
 */

// Namespace for Public Controllers
namespace FeChat.Controllers.Public {

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the MVC support for Controllers feature
    using Microsoft.AspNetCore.Mvc;

    // Use the versioning support to add version in url
    using Asp.Versioning;

    // Use the General Dtos
    using FeChat.Models.Dtos;

    // Use the Subscriptions Dtos
    using FeChat.Models.Dtos.Subscriptions;

    // Use the Dtos for Subscriptions
    using FeChat.Models.Dtos.Transactions;

    // Gets the Settings Repositories
    using FeChat.Utils.Interfaces.Repositories.Settings;

    // Use the Repositories for Subscriptions
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Webhook Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/webhook")]
    public partial class WebhookController: Controller {

        /// <summary>
        /// Receive the webhook request
        /// </summary>
        /// <returns>Empty result</returns>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task WebhookRequest(ISettingsRepository settingsRepository, ISubscriptionsRepository subscriptionsRepository) {

            // Get the data from the stread
            using StreamReader reader = new (Request.Body);

            // Read the data from the stream
            string requestBody = await reader.ReadToEndAsync();

            // Decode the request but first remove any html tag
            dynamic? responseDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(MyRegex().Replace(requestBody, string.Empty));
                
            // Verify if response is not null
            if ( responseDecode != null ) {

                // Verify if event_type is PAYMENT.SALE.COMPLETED
                if ( responseDecode.event_type != null && responseDecode.event_type == "PAYMENT.SALE.COMPLETED" ) {

                    // Verify if subscription id exists
                    if ( responseDecode.resource != null && responseDecode.resource.billing_agreement_id != null ) {

                        // Get payment id
                        string paymentId = responseDecode.resource.id.ToString();

                        // Get subscription id
                        string subscriptionId = responseDecode.resource.billing_agreement_id.ToString();

                        // Get subscription by subscription id
                        ResponseDto<SubscriptionDto> subscriptionDto = await subscriptionsRepository.GetSubscriptionByNetIdAsync(subscriptionId);

                        // Verify if subscription exists
                        if ( subscriptionDto.Result != null ) {

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

                            // Get PayPal status
                            optionsList.TryGetValue("PayPalEnabled", out string? paypalStatus);

                            // Check if PayPal is enabled
                            if ( paypalStatus == "1" ) {

                                // Generate access token
                                RestResponseDto tokenResponse = await new Utils.Gateways.PayPal.Tokens().GenerateAsync(optionsList);

                                // Verify if the response failed
                                if ( tokenResponse.Success == true ) {

                                    // Get Subscription details
                                    RestResponseDto subscriptionResponse = await new Utils.Gateways.PayPal.Subscriptions.ReadSubscription().GetAsync(optionsList, tokenResponse.Data!, subscriptionId);

                                    // Verify if subscription exists
                                    if ( subscriptionResponse.Success == true ) {

                                        // Decode the Response
                                        dynamic responsePlanDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(subscriptionResponse.Data!)!;

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

                                        // Check if next billing time is not expired
                                        if ( billingOffset.ToUnixTimeSeconds() > DateTimeOffset.UtcNow.ToUnixTimeSeconds() ) {

                                            // Create the subscription data for updating
                                            SubscriptionDto subscriptionData = new() {
                                                SubscriptionId = subscriptionDto.Result.SubscriptionId,
                                                MemberId = subscriptionDto.Result.MemberId,
                                                PlanId = subscriptionDto.Result.PlanId,
                                                OrderId = paymentId,
                                                NetId = subscriptionId,
                                                Source = "PayPal",
                                                Expiration = (int)billingOffset.ToUnixTimeSeconds(),
                                                Created = subscriptionDto.Result.Created
                                            };

                                            // Update the subscription
                                            ResponseDto<bool> subscriptionUpdate = await subscriptionsRepository.UpdateSubscriptionAsync(subscriptionData);

                                            // Verify if the subscription was updated
                                            if ( subscriptionUpdate.Result == true ) {

                                                // Create a transaction
                                                TransactionDto transactionDto = new() {
                                                    MemberId = subscriptionDto.Result.MemberId,
                                                    SubscriptionId = subscriptionDto.Result.SubscriptionId,
                                                    PlanId = subscriptionDto.Result.PlanId,
                                                    OrderId = paymentId,
                                                    NetId = subscriptionId,
                                                    Source = "PayPal"
                                                };

                                                // Save transaction
                                                await subscriptionsRepository.CreateTransactionAsync(transactionDto);

                                            }
                                            
                                        }

                                    }
                                    
                                }

                            }

                        }

                    }

                }
                
            }

        }

        [System.Text.RegularExpressions.GeneratedRegex("<[^>]*>")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();

    }

}