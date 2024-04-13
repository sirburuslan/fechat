/*
 * @class Language Middleware
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-04-04
 *
 * This scope of this class is to create a middleware which will validate member's access and will change the language
 */

// Namespace for Extensions
namespace FeChat.Utils.Extensions {

    // System Namespaces
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Utils.General;
    using Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Language Middleware
    /// </summary>
    public class LanguageMiddleware : IMiddleware
    {

        /// <summary>
        /// Entry Point for Middleware
        /// </summary>
        /// <param name="context">HTTP request information</param>
        /// <param name="next">Is the delegate to run the next middleware</param>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next) {

            // Default language container
            string lang = "en-US";

            // Check if is requested the administrator controllers
            if ( context.Request.Path.ToString().IndexOf("/api/v1/admin/") == 0 ) {

                // Get current member data
                ResponseDto<MemberDto> memberAccess = await new Access(context).IsAdminAsync(context.RequestServices.GetService<IMembersRepository>()!);

                // Check if memberAccess contains an error message
                if ( memberAccess.Result == null ) {

                    // Serialize the response object to JSON
                    var jsonResponse = JsonConvert.SerializeObject(new {
                        success = false,
                        message = memberAccess.Message
                    });

                    // Set the content type header
                    context.Response.ContentType = "application/json";

                    // Write the JSON response to the response body
                    await context.Response.WriteAsync(jsonResponse);
                    return;

                }

                // Get the member service
                var member = context.RequestServices.GetService<Member>();

                // Check if member is not null
                if ( member != null ) {

                    // Set Member
                    member.Info = memberAccess.Result;
                    
                }

                // Set language
                lang = (memberAccess.Result.Language != null)?memberAccess.Result.Language:"";

            } else if ( (context.Request.Path.ToString().IndexOf("/api/v1/user") == 0) && ( context.Request.Path != "/api/v1/user/websocket" ) ) {

                // Get current member data
                ResponseDto<MemberDto> memberAccess = await new Access(context).IsUserAsync(context.RequestServices.GetService<IMembersRepository>()!);

                // Check if memberAccess contains an error message
                if ( memberAccess.Result == null ) {

                    // Serialize the response object to JSON
                    var jsonResponse = JsonConvert.SerializeObject(new {
                        success = false,
                        message = memberAccess.Message
                    });

                    // Set the content type header
                    context.Response.ContentType = "application/json";

                    // Write the JSON response to the response body
                    await context.Response.WriteAsync(jsonResponse);
                    return;

                }

                // Get the member service
                var member = context.RequestServices.GetService<Member>();

                // Check if member is not null
                if ( member != null ) {

                    // Set Member
                    member.Info = memberAccess.Result;
                    
                }

                // Set language
                lang = (memberAccess.Result.Language != null)?memberAccess.Result.Language:"";

            }

            // Set the culture for the request
            var cultureInfo = new CultureInfo(lang);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            // Call the next middleware in the pipeline
            await next(context);

        }
        
    }

}