/*
 * @class Read Subscription
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to read subscription on PayPal
 */

// Namespace for PayPal Read Subscription
namespace FeChat.Utils.Gateways.PayPal.Subscriptions {

    // Use General Dtos
    using FeChat.Models.Dtos;

    /// <summary>
    /// PayPal Read Subscription
    /// </summary>
    public class ReadSubscription {

        /// <summary>
        /// Read Subscription
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="accessToken">Access Token for PayPal</param>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns>Subscription details or error message</returns>
        public async Task<RestResponseDto> GetAsync(Dictionary<string, string> optionsList, string accessToken, string subscriptionId) {

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);   

                HttpClient httpClient = new();

                // Set authorization
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Set the subscriptions url
                string subscriptionsUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/billing/subscriptions/":"https://api.paypal.com/v1/billing/subscriptions/";

                // Make POST request
                HttpResponseMessage SubscriptionResponse = await httpClient.GetAsync(subscriptionsUrl + subscriptionId);

                // Verify if the request is successfully
                if (SubscriptionResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responseSubscriptionJson = await SubscriptionResponse.Content.ReadAsStringAsync();

                    // Return subscription id
                    return new RestResponseDto {
                        Success = true,
                        Data = responseSubscriptionJson
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