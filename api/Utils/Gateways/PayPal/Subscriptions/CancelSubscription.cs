/*
 * @class Cancel Subscription
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-09
 *
 * This class is used to cancel subscription on PayPal
 */

// Namespace for PayPal Cancel Subscription
namespace FeChat.Utils.Gateways.PayPal.Subscriptions {

    // System Namespaces
    using System.Text;

    // App Namespaces
    using Models.Dtos;

    /// <summary>
    /// PayPal Cancel Subscription
    /// </summary>
    public class CancelSubscription {

        /// <summary>
        /// Cancel Subscription
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="accessToken">Access Token for PayPal</param>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns>Subscription details or error message</returns>
        public async Task<RestResponseDto> CancelAsync(Dictionary<string, string> optionsList, string accessToken, string subscriptionId) {

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);   

                HttpClient httpClient = new();

                // Set authorization
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Set content type
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // Create the cancel information
                var cancelInfo = new {
                    reason = "Created a new subscription."
                };

                // Create the cancel content
                StringContent cancelContent = new(Newtonsoft.Json.JsonConvert.SerializeObject(cancelInfo), Encoding.UTF8, "application/json");

                // Set the subscriptions url
                string subscriptionsUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/billing/subscriptions/":"https://api.paypal.com/v1/billing/subscriptions/";

                // Make POST request
                HttpResponseMessage SubscriptionResponse = await httpClient.PostAsync(subscriptionsUrl + subscriptionId + "/cancel", cancelContent);

                // Verify if the request is successfully
                if (SubscriptionResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responseSubscriptionJson = await SubscriptionResponse.Content.ReadAsStringAsync();

                    // Return subscription id
                    return new RestResponseDto {
                        Success = true,
                        Message = responseSubscriptionJson
                    };

                } else {

                    // Request failed
                    string errorMessage = await SubscriptionResponse.Content.ReadAsStringAsync();

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