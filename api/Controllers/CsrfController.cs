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
        /// Generate an Anti-CSRF token
        /// </summary>
        /// <returns>Csrf token or error message</returns>
        [HttpGet("generate")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public IActionResult Generate(IAntiforgery antiforgery) {

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

        }
        
    }

}