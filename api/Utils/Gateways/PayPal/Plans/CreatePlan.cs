/*
 * @class Create Plan
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to create plans on PayPal
 */

// Namespace for PayPal Plans
namespace FeChat.Utils.Gateways.PayPal.Plans {

    // System Namespaces
    using System.Text;

    // App Namespaces
    using Models.Dtos;

    /// <summary>
    /// Create Plans Class
    /// </summary>
    public class CreatePlan {

        /// <summary>
        /// Create a plan
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="accessToken">Access Token for PayPal</param>
        /// <param name="planId">Plan Id</param>
        /// <param name="payPalProductId">PayPal Product Id</param>
        /// <param name="planPrice">Plan Price</param>
        /// <param name="planCurrency">Plan Currency</param>
        /// <returns>Plan ID or error</returns>
        public async Task<RestResponseDto> CreateAsync(Dictionary<string, string> optionsList, string accessToken, int planId, string payPalProductId, string planPrice, string planCurrency) {

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);   

                // Create a http instance for plan creation
                using HttpClient httpPlanCreate = new();

                // Set authorization
                httpPlanCreate.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                
                // Set return preference
                httpPlanCreate.DefaultRequestHeaders.Add("Prefer", "return=representation");    

                // Create the plan information
                var planInfo = new {
                    product_id = payPalProductId,
                    name = $"Plan {planId}",
                    description = "Chat Suscription",
                    status = "ACTIVE",
                    billing_cycles = new[] {
                        new {
                            frequency = new {
                                interval_unit = "MONTH",
                                interval_count = 1
                            },
                            tenure_type = "REGULAR",
                            sequence = 1,
                            total_cycles = 999,
                            pricing_scheme = new {
                                fixed_price = new {
                                    value = planPrice,
                                    currency_code = planCurrency
                                }

                            }

                        }

                    },
                    payment_preferences = new {
                        service_type = "PREPAID",
                        auto_bill_outstanding = true,
                        setup_fee_failure_action = "CONTINUE",
                        payment_failure_threshold = 3                                
                    }
                };

                // Create the plan content
                StringContent planContent = new(Newtonsoft.Json.JsonConvert.SerializeObject(planInfo), Encoding.UTF8, "application/json");

                // Set the plan url
                string planUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/billing/plans":"https://api.paypal.com/v1/billing/plans";
                
                // Make POST request
                HttpResponseMessage PlanResponse = await httpPlanCreate.PostAsync(planUrl, planContent);

                // Verify if the request is successfully
                if (PlanResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responsePlanJson = await PlanResponse.Content.ReadAsStringAsync();

                    // Decode the Response
                    dynamic responsePlanDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(responsePlanJson)!;

                    // Return plan id
                    return new RestResponseDto {
                        Success = true,
                        Data = responsePlanDecode.id
                    };

                } else {

                    // Request failed
                    string errorMessage = await PlanResponse.Content.ReadAsStringAsync();

                    // Decode the error message
                    dynamic errorMessageDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(errorMessage)!;                    

                    // Return error
                    return new RestResponseDto {
                        Success = false,
                        Message = errorMessageDecode.message
                    };

                }

            } catch (Exception ex) {

                // Return error
                return new RestResponseDto {
                    Success = false,
                    Message = ex.Message
                };

            }

        }

    }

}