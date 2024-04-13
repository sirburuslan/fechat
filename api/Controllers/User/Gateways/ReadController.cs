/*
 * @class Gateways Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the gateways
 */

// Namespace for User Gateways Controllers
namespace FeChat.Controllers.User.Gateways {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Plans;
    using Models.Dtos.Subscriptions;
    using Models.Entities.Plans;
    using Utils.General;
    using Utils.Interfaces.Repositories.Settings;
    using Utils.Interfaces.Repositories.Plans;
    using Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Gateways Read Controller
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/gateways")]
    public class ReadController: Controller {

        /// <summary>
        /// Http Client container
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="httpClient">Http Client</param>
        public ReadController(HttpClient httpClient) {

            // Save http client
            _httpClient = httpClient;

        }

        /// <summary>
        /// Gets the Gateways List
        /// </summary>
        /// <param name="planId">Plan Id</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="settingsRepository">An instance to the Settings repository</param>
        /// <param name="plansRepository">An instance to the Plans repository</param>
        /// <param name="subscriptionsRepository">An instance to the Subscriptions repository</param>
        /// <returns>List with gateways or error message</returns>
        [HttpGet("{planId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> GatewaysList(int planId, Member memberInfo, ISettingsRepository settingsRepository, IPlansRepository plansRepository, ISubscriptionsRepository subscriptionsRepository) {

            // Get the plan's data
            ResponseDto<PlanDto> planDto = await plansRepository.GetPlanAsync(planId);

            // Verify if the plan exists
            if ( planDto.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PlanNotFound")
                });

            }

            // Get plan price
            double planPrice = 0.00;

            // Verify if plan price is not null
            if ( (planDto.Result.Price != null) && double.TryParse(planDto.Result.Price, out double parsedPrice) ) {

                // Set plan price
                planPrice = parsedPrice;

            }

            // Verify if the plan price is valid
            if (planPrice < double.Parse("0.01")) {

                // Get the current date and time
                DateTimeOffset currentDate = DateTimeOffset.UtcNow;

                // Add 30 days to the current date
                DateTimeOffset after30Days = currentDate.AddDays(30);

                // Get the user's subscription
                ResponseDto<SubscriptionDto> subscriptionResponse = await subscriptionsRepository.GetSubscriptionByMemberIdAsync(memberInfo.Info!.MemberId);

                // Verify if subscription exists
                if ( subscriptionResponse.Result != null ) {

                    // Create the subscription
                    SubscriptionDto subscription = new() {
                        SubscriptionId = subscriptionResponse.Result.SubscriptionId,
                        MemberId = memberInfo.Info.MemberId,
                        PlanId = planId,
                        OrderId = "",
                        NetId = "",
                        Source = "",
                        Expiration = (int)after30Days.ToUnixTimeSeconds(),
                        Created = subscriptionResponse.Result.Created
                    };

                    // Update the subscription
                    ResponseDto<bool> updateSubscription = await subscriptionsRepository.UpdateSubscriptionAsync(subscription);

                    // Check if the subscription wasn't updated successfully
                    if ( updateSubscription.Result == false ) {

                        // Return error response
                        return new JsonResult(new {
                            success = false,
                            message = updateSubscription.Message
                        });

                    }

                    // Return a json
                    return new JsonResult(new {
                        success = true,
                        message = new Strings().Get("SubscriptionUpdated"),
                        free = 1
                    });

                } else {

                    // Create the subscription
                    SubscriptionDto subscription = new() {
                        MemberId = memberInfo.Info.MemberId,
                        PlanId = planId,
                        OrderId = "",
                        NetId = "",
                        Source = "",
                        Expiration = (int)after30Days.ToUnixTimeSeconds()
                    };

                    // Save the subscription
                    ResponseDto<SubscriptionDto> saveSubscription = await subscriptionsRepository.CreateSubscriptionAsync(subscription);

                    // Check if the subscription wasn't saved successfully
                    if ( saveSubscription.Result == null ) {

                        // Return error response
                        return new JsonResult(new {
                            success = false,
                            message = saveSubscription.Message
                        });

                    }

                    // Return a json
                    return new JsonResult(new {
                        success = true,
                        message = new Strings().Get("SubscriptionCreated"),
                        free = 1
                    });

                }
                
            }

            // Verify if a currency exists
            if ( planDto.Result.Currency == "" ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PayPalCurrencyNotValid")
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
            ResponseDto<List<PlanMetaDto>> savedMeta = await plansRepository.MetaListAsync(planId);

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

            // Gateways list container
            List<object> gatewaysList = new();

            // Get PayPal status
            optionsList.TryGetValue("PayPalEnabled", out string? paypalStatus);

            // Check if PayPal is enabled
            if ( paypalStatus == "1" ) {

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

                // Check if meta has saved PayPalProductId
                if ( !metaList.TryGetValue("PayPalProductId", out string? payPalProductId) ) {

                    // Create a product
                    RestResponseDto productResponse = await new Utils.Gateways.PayPal.Products.CreateProduct().CreateAsync(optionsList, tokenResponse.Data!, planId);

                    // Verify if the response failed
                    if ( productResponse.Success == false ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = productResponse.Message
                        });
                        
                    }

                    // Create a meta
                    List<PlansMetaEntity> productEntity = new()
                    {
                        new() {
                            PlanId = planId,
                            MetaName = "PayPalProductId",
                            MetaValue = productResponse.Data
                        }
                    };

                    // Try to save the meta
                    bool saveProduct = await plansRepository.SavePlanMetaAsync(productEntity); 

                    // Check if the product was saved
                    if ( saveProduct == false ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("PayPalProductWasNotSaved")
                        });
                        
                    }

                    // Set PayPalProductId
                    payPalProductId = productResponse.Data;

                }

                // Check if meta has saved PayPalPlanId
                if ( !metaList.TryGetValue("PayPalPlanId", out string? payPalPlanId) ) {

                    // Create a plan
                    RestResponseDto planResponse = await new Utils.Gateways.PayPal.Plans.CreatePlan().CreateAsync(optionsList, tokenResponse.Data!, planId, payPalProductId!, planPrice.ToString("0.00"), planDto.Result.Currency!);

                    // Verify if the response failed
                    if ( planResponse.Success == false ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = planResponse.Message
                        });
                        
                    }

                    // Create a meta
                    List<PlansMetaEntity> planEntity = new()
                    {
                        new() {
                            PlanId = planId,
                            MetaName = "PayPalPlanId",
                            MetaValue = planResponse.Data
                        }
                    };

                    // Try to save the meta
                    bool savePlan = await plansRepository.SavePlanMetaAsync(planEntity); 

                    // Check if the plan was saved
                    if ( savePlan == false ) {

                        // Return a json
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("PayPalPlanWasNotSaved")
                        });
                        
                    }

                    // Set PayPalPlanId
                    payPalPlanId = planResponse.Data;

                }

                // Read a plan
                RestResponseDto planDetailsResponse = await new Utils.Gateways.PayPal.Plans.ReadPlan().ReadAsync(optionsList, tokenResponse.Data!, payPalPlanId!);

                // Verify if the response failed
                if ( planDetailsResponse.Success == false ) {

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = planDetailsResponse.Message
                    });
                    
                }

                // Decode the Response
                dynamic responsePlanDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(planDetailsResponse.Data!)!;

                // Check if billing_cycles array exists and has at least one element
                if ( responsePlanDecode?.billing_cycles != null ) {

                    // Access the first element of billing_cycles array
                    var firstBillingCycle = responsePlanDecode.billing_cycles[0];

                    // Check if pricing_scheme exists
                    if (firstBillingCycle?.pricing_scheme != null) {

                        // Access fixed_price property of pricing_scheme
                        var fixedPrice = firstBillingCycle.pricing_scheme.fixed_price;

                        // Check if fixed_price and value properties exist
                        if (fixedPrice != null) {

                            // Access value property
                            var value = fixedPrice.value;
                            
                            // Access currency property
                            var currencyCode = fixedPrice.currency_code;   

                            // Verify if the price was changed
                            if ( (value != planPrice) || (currencyCode != planDto.Result.Currency!) ) {

                                // Create a plan
                                RestResponseDto planResponse = await new Utils.Gateways.PayPal.Plans.CreatePlan().CreateAsync(optionsList, tokenResponse.Data!, planId, payPalProductId!, planPrice.ToString("0.00"), planDto.Result.Currency!);

                                // Verify if the response failed
                                if ( planResponse.Success == false ) {

                                    // Return a json
                                    return new JsonResult(new {
                                        success = false,
                                        message = planResponse.Message
                                    });
                                    
                                }

                                // Replace plan's id
                                payPalPlanId = planResponse.Data;

                                // Verify if meta exists
                                if ( savedMeta.Result != null ) {

                                    // Plans Meta Container for update
                                    List<PlansMetaEntity> plansMetaEntities = new();

                                    // Get meta length
                                    int metaLength = savedMeta.Result.Count;

                                    // List the saved meta
                                    for ( int m = 0; m < metaLength; m++ ) {

                                        // Check if meta name is PayPalPlanId
                                        if ( savedMeta.Result[m].MetaName != "PayPalPlanId" ) {
                                            continue;
                                        }

                                        // Add meta to the container
                                        plansMetaEntities.Add(new PlansMetaEntity {
                                            MetaId = savedMeta.Result[m].MetaId,
                                            PlanId = savedMeta.Result[m].PlanId,
                                            MetaName = savedMeta.Result[m].MetaName,
                                            MetaValue = planResponse.Data
                                        });

                                    }

                                    // Update plans meta
                                    plansRepository.UpdateMetaAsync(plansMetaEntities);

                                }

                            }

                        }
                        
                    }
                    
                }

                // Add paypal as gateway
                gatewaysList.Add(new {
                    network = "PayPal",
                    clientId = payPalClientId,
                    planId = payPalPlanId
                });

            }

            // Verify if at least a gateways is configured
            if ( gatewaysList.Count < 1 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("NoPaymentsMethodsAvailable")
                });

            }

            // Return gateways list
            return new JsonResult(new {
                success = true,
                gateways = gatewaysList
            });

        }

    }

}