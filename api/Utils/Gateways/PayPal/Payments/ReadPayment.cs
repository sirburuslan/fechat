/*
 * @class Read Payment
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-20
 *
 * This class is used to read payments from PayPal
 */

// Namespace for PayPal Payments
namespace FeChat.Utils.Gateways.PayPal.Payments {

    // Use General Dtos
    using FeChat.Models.Dtos;

    /// <summary>
    /// Read Payments Class
    /// </summary>
    public class ReadPayment {

        /// <summary>
        /// Get a Payment
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="accessToken">Access Token for PayPal</param>
        /// <param name="payPalPaymentId">Payment Id</param>
        /// <returns>Payment data or error</returns>
        public async Task<RestResponseDto> GetAsync(Dictionary<string, string> optionsList, string accessToken, string payPalPaymentId) {

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);   

                HttpClient httpClient = new();

                // Set authorization
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Set the payment url
                string paymentUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/payments/sale/":"https://api.paypal.com/v1/payments/sale/";

                // Make POST request
                HttpResponseMessage PaymentResponse = await httpClient.GetAsync(paymentUrl + payPalPaymentId);

                // Verify if the request is successfully
                if (PaymentResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responsePaymentJson = await PaymentResponse.Content.ReadAsStringAsync();

                    // Return Payment id
                    return new RestResponseDto {
                        Success = true,
                        Data = responsePaymentJson
                    };

                } else {

                    // Request failed
                    string errorMessage = await PaymentResponse.Content.ReadAsStringAsync();

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