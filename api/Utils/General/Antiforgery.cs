/*
 * @class Antiforgery Validator
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-23
 *
 * This class validates an antiforgery token
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // Use the Antiforgery classes
    using Microsoft.AspNetCore.Antiforgery;    

    /// <summary>
    /// Antiforgery Validator
    /// </summary>
    public class Antiforgery {

        /// <summary>
        /// Http Context container
        /// </summary>
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Antiforgery Constructor
        /// </summary>
        /// <param name="httpContext">Http context</param>
        /// <param name="configuration">App configuration</param>
        public Antiforgery(HttpContext httpContext, IConfiguration configuration) {

            // Set http context
            _httpContext = httpContext;

            // Add configuration to the container
            _configuration = configuration;

        }
        /// <summary>
        /// Validates an antiforgery token
        /// </summary>
        /// <returns>Bool true or false</returns>
        public async Task<bool> Validate() {

            try {

                // Verify if Antiforgery is enabled
                if ( _configuration["Antiforgery"] == "1" ) {

                    // Get the IAntiforgery instance
                    IAntiforgery antiforgery = _httpContext.RequestServices.GetRequiredService<IAntiforgery>();

                    // Try to validate
                    await antiforgery.ValidateRequestAsync(_httpContext);

                } else {

                    // Get the csrf token
                    _httpContext.Request.Headers.TryGetValue("CsrfToken", out Microsoft.Extensions.Primitives.StringValues token);

                    // Get the time
                    int? time = int.Parse(new Encryptor().Decrypt(token!, _configuration["AntiforgerySecretKey"] ?? string.Empty));

                    // Verify if time is null
                    if ( time == null ) {
                        return false;
                    }
                    
                    // Verify if the token was generated more than 1 minute ago
                    if ( DateTimeOffset.UtcNow.ToUnixTimeSeconds() - time > 60 ) {
                        return false;
                    }

                }

                return true;

            } catch (Exception) {

                return false;

            }

        }

    }

}