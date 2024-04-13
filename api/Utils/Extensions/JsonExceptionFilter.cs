/*
 * @class Exceptions Filter
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-04-12
 *
 * This scope of this class is to turn in json the catched exceptions
 */

// Namespace for Extensions
namespace FeChat.Utils.Extensions {

    // System Namespaces
    using Microsoft.AspNetCore.Mvc;    
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Json Exception Filter
    /// </summary>
    public class JsonExceptionFilter : IExceptionFilter
    {

        /// <summary>
        /// Turn exception message in json
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {

            // Re-create the error response
            var errorResponse = new
            {
                success = false,
                message = context.Exception.Message
            };

            // Set the status code to 200 Bad Request
            context.HttpContext.Response.StatusCode = 200;

            // Serialize the error response to JSON
            var jsonResult = new JsonResult(errorResponse);

            // Replace the original response with the JSON error response
            context.Result = jsonResult;
            context.ExceptionHandled = true;

        }
        
    }

}