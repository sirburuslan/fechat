/*
 * @class Read Plan
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to read plans on PayPal
 */

// Namespace for PayPal Plans
namespace FeChat.Utils.Gateways.PayPal.Plans {

    // App Namespaces
    using Models.Dtos;

    /// <summary>
    /// Read Plans Class
    /// </summary>
    public class ReadPlan {

        /// <summary>
        /// Read a plan
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="accessToken">Access Token for PayPal</param>
        /// <param name="payPalPlanId">Plan Id</param>
        /// <returns>Plan's data or error</returns>
        public async Task<RestResponseDto> ReadAsync(Dictionary<string, string> optionsList, string accessToken, string payPalPlanId) {

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);   

                HttpClient httpClient = new();

                // Set authorization
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Set the plan url
                string planUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/billing/plans/":"https://api.paypal.com/v1/billing/plans/";

                // Make POST request
                HttpResponseMessage PlanResponse = await httpClient.GetAsync(planUrl + payPalPlanId);

                // Verify if the request is successfully
                if (PlanResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responsePlanJson = await PlanResponse.Content.ReadAsStringAsync();

                    // Return plan id
                    return new RestResponseDto {
                        Success = true,
                        Data = responsePlanJson
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