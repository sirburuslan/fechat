/*
 * @class Tokens
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to generate access tokens for PayPal
 */

// Namespace for PayPal Gateway
namespace FeChat.Utils.Gateways.PayPal {

    // Use text for encoding
    using System.Text;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use General Utils
    using FeChat.Utils.General;

    /// <summary>
    /// Tokens Generation Class
    /// </summary>
    public class Tokens {

        /// <summary>
        /// General access token
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <returns>Return access token or error message</returns>
        public async Task<RestResponseDto> GenerateAsync(Dictionary<string, string> optionsList) { 

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);                

                // Get the PayPal Client Id
                optionsList.TryGetValue("PayPalClientId", out string? payPalClientId);

                // Get the PayPal Client Secret
                optionsList.TryGetValue("PayPalClientSecret", out string? payPalClientSecret);   

                // Verify if client id and secret exists
                if ( (payPalClientId == "") || (payPalClientSecret == "") ) {

                    // Return error
                    return new RestResponseDto {
                        Success = false,
                        Message = new Strings().Get("PayPalNotConfigured")
                    };

                } 

                // Create http client instance
                using HttpClient httpAccessToken = new();

                // Set Authorization
                httpAccessToken.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payPalClientId}:{payPalClientSecret}"))}");

                // Set grant type
                StringContent tokenData = new("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                // Set the tokenUrl
                string tokenUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/oauth2/token":"https://api.paypal.com/v1/oauth2/token";
                
                // Make POST request
                HttpResponseMessage TokenResponse = await httpAccessToken.PostAsync(tokenUrl, tokenData);

                // Verify if the request is successfully
                if (TokenResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responseJson = await TokenResponse.Content.ReadAsStringAsync();

                    // Decode the Response
                    dynamic responseDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson)!;

                    // Verify if access token exists
                    if ( responseDecode.access_token == null ) {

                        // Return error
                        return new RestResponseDto {
                            Success = false,
                            Message = new Strings().Get("PayPalTokenMissing")
                        };

                    }

                    // Return access token
                    return new RestResponseDto {
                        Success = true,
                        Data = responseDecode.access_token
                    };                    

                } else {

                    // Request failed
                    string errorMessage = await TokenResponse.Content.ReadAsStringAsync();

                    // Decode the error message
                    dynamic errorMessageDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(errorMessage)!;                    

                    // Return error
                    return new RestResponseDto {
                        Success = false,
                        Message = errorMessageDecode.message
                    };

                }

            } catch (Exception ex) {

                // Return error message
                return new RestResponseDto {
                    Success = false,
                    Message = ex.Message
                };

            }

        }

    }

}