/*
 * @class Create Product
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to create products on PayPal
 */

// Namespace for PayPal Products
namespace FeChat.Utils.Gateways.PayPal.Products {

    // System Namespaces
    using System.Text;

    // App Namespaces
    using Models.Dtos;

    /// <summary>
    /// Create Products Class
    /// </summary>
    public class CreateProduct {

        /// <summary>
        /// Create a product
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="accessToken">Access Token for PayPal</param>
        /// <param name="planId">Plan Id</param>
        /// <returns>Product ID or error</returns>
        public async Task<RestResponseDto> CreateAsync(Dictionary<string, string> optionsList, string accessToken, int planId) {

            try {

                // Get the PayPal Sandbox Status
                optionsList.TryGetValue("PayPalSandboxEnabled", out string? payPalSandboxEnabled);      

                // Create a http instance for product creation
                using HttpClient httpProductCreate = new();

                // Set authorization
                httpProductCreate.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                
                // Set PayPal Request Id
                httpProductCreate.DefaultRequestHeaders.Add("PayPal-Request-Id", $"Plan {planId}");

                // Create the product information
                var productInfo = new {
                    name = $"Plan {planId}",
                    type = "SERVICE"
                };

                // Create the product content
                StringContent productContent = new(Newtonsoft.Json.JsonConvert.SerializeObject(productInfo), Encoding.UTF8, "application/json");

                // Set the product url
                string productUrl = (payPalSandboxEnabled == "1")?"https://api-m.sandbox.paypal.com/v1/catalogs/products":"https://api.paypal.com/v1/catalogs/products";
                
                // Make POST request
                HttpResponseMessage ProductResponse = await httpProductCreate.PostAsync(productUrl, productContent);

                // Verify if the request is successfully
                if (ProductResponse.IsSuccessStatusCode) {

                    // Read the response
                    string responseProductJson = await ProductResponse.Content.ReadAsStringAsync();

                    // Decode the Response
                    dynamic responseProductDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(responseProductJson)!;
                    
                    // Return product id
                    return new RestResponseDto {
                        Success = true,
                        Data = responseProductDecode.id
                    };

                } else {

                    // Request failed
                    string errorMessage = await ProductResponse.Content.ReadAsStringAsync();

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