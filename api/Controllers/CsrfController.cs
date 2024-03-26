/*
 * @class Csrf Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to generate Anti-CSRF tokens
 */

// Namespace for General Controllers
namespace FeChat.Controllers {

    // Use Antiforgery for csrf generation
    using Microsoft.AspNetCore.Antiforgery;

    // Use Mvc for Controllers
    using Microsoft.AspNetCore.Mvc;

    // Use the cors
    using Microsoft.AspNetCore.Cors;

    // Add api version in url
    using Asp.Versioning;

    // Use General classes for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Anti-CSRF tokens generator
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CsrfController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public CsrfController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Generate an Anti-CSRF token
        /// </summary>
        /// <returns>Csrf token or error message</returns>
        [HttpGet("generate")]
        [EnableCors("AllowOrigin")]
        public IActionResult Generate(IAntiforgery antiforgery) {
            
            // Verify if Antiforgery is enabled
            if ( _configuration["Antiforgery"] == "1" ) {

                // Generate a new token
                AntiforgeryTokenSet token = antiforgery.GetAndStoreTokens(HttpContext);

                // Check if a token was generated
                if ( token != null ) {

                    // Return success response
                    return new JsonResult(new {
                        success = true,
                        token
                    });

                } else {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("CSRFNotGenerated")
                    });
                    
                }

            } else {

                // If the frontend is on different domain, antiforgery will be used a generated code based on the current time
                // The Cors Policy anyway allows to control from which domain could be accepted the calls

                try {

                    // Create encrypted text
                    string encryptedText = new Encryptor().Encrypt(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), _configuration["AntiforgerySecretKey"] ?? string.Empty);

                    // Return success response
                    return new JsonResult(new {
                        success = true,
                        token = new {
                            requestToken = encryptedText
                        }
                    });

                } catch (Exception ex) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = ex.Message
                    });
                    
                }

            }

        }
        
    }

}