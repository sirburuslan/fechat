/*
 * @class Program
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-02
 *
 * This class is the entry point in the app
 */

// App namespace
namespace FeChat {

    // Use Globalization for cultures
    using System.Globalization;
    
    // Use the Text for key enconding
    using System.Text;

    // Use the media types
    using static System.Net.Mime.MediaTypeNames;    

    // Use the MVC design pattern
    using Microsoft.AspNetCore.Mvc;
    
    // Use entity framework
    using Microsoft.EntityFrameworkCore;

    // Use the documentation for api
    using Microsoft.OpenApi.Models;

    // Use JwtBearer to validate JWTs
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    // Use the Identity Model to create the tokens parameters
    using Microsoft.IdentityModel.Tokens;

    // Use the User controllers
    using FeChat.Controllers.User;

    // Use the Public controllers
    using FeChat.Controllers.Public;

    // Use the General Dtos
    using FeChat.Models.Dtos;

    // Use the Members Dtos
    using FeChat.Models.Dtos.Members;

    // Use the Settings repositories
    using FeChat.Models.Repositories.Settings;

    // Use the Events repositories
    using FeChat.Models.Repositories.Events;

    // Use the Members repositories
    using FeChat.Models.Repositories.Members;

    // Use the Messages repositories
    using FeChat.Models.Repositories.Messages;  

    // Use the Plans repositories
    using FeChat.Models.Repositories.Plans;   

    // Use the Subscriptions repositories
    using FeChat.Models.Repositories.Subscriptions;   
    
    // Use the Websites repositories
    using FeChat.Models.Repositories.Websites;     

    // Use the configuration utils for db
    using FeChat.Utils.Configuration;

    // Use the general utils for filters
    using FeChat.Utils.General;

    // Use the settings interfaces for repositories
    using FeChat.Utils.Interfaces.Repositories.Settings;

    // Use the events interfaces for repositories
    using FeChat.Utils.Interfaces.Repositories.Events;

    // Use the members interfaces for repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the interfaces for messages repositories
    using FeChat.Utils.Interfaces.Repositories.Messages;

    // Use the plans interfaces for repositories
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use the subscriptions interfaces for repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    // Use the interfaces for websites repositories
    using FeChat.Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Program class for main entry
    /// </summary>
    public class Program {

        /// <summary>
        /// Main class used as entry point for the app
        /// </summary>
        /// <param name="args">
        /// Command-line arguments passed when the application is started
        /// </param>
        /// <returns>
        /// void
        /// </returns>
        public static void Main(string[] args) {

            // Create an instance of WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);

            // Get the app domain
            var AppDomain = builder.Configuration.GetValue<string>("AppDomain");

            // Get the site url
            var SiteUrl = builder.Configuration.GetValue<string>("SiteUrl");

            // Get the jwt key
            var JwtKey = builder.Configuration.GetValue<string>("JwtKey");  

            // Add multiversions support
            builder.Services.AddApiVersioning().AddApiExplorer(options => {
            
                // Format string for the current API version
                options.GroupNameFormat = "'v'VVV";

                // Adds the version in the urls
                options.SubstituteApiVersionInUrl = true;

            });

            // Connect the DB Table Members
            builder.Services.AddDbContext<Db>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            // Register the library for cache storing
            builder.Services.AddMemoryCache();

            // Register service for Member
            builder.Services.AddScoped<Member>();

            // Register the Events Repository
            builder.Services.AddScoped<IEventsRepository, EventsRepository>();

            // Register the Members repository
            builder.Services.AddScoped<IMembersRepository, MembersRepository>();           

            // Register the Messages Repository
            builder.Services.AddScoped<IMessagesRepository, MessagesRepository>(); 

            // Register the Plans repository
            builder.Services.AddScoped<IPlansRepository, PlansRepository>();

            // Register the Settings repository
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

            // Register the Subscriptions repository
            builder.Services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();  

            // Register the Websites Repository
            builder.Services.AddScoped<IWebsitesRepository, WebsitesRepository>();                            

            // Register the localization for the app's strings
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Configure the session state
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = AppDomain,
                    ValidAudience = AppDomain,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey ?? string.Empty))
                };
                
            });

            // Add services for controllers
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => {

                options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState) {
                        
                    ContentTypes = {
                        Application.Json
                    },
                    StatusCode = 200
                    
                };

            })
            .AddXmlSerializerFormatters();

            // Verify if the app is in development
            if ( builder.Environment.IsDevelopment() ) {

                // Parse information about endpoints
                builder.Services.AddEndpointsApiExplorer();

                // Enable the service which generates a documentation
                builder.Services.AddSwaggerGen(c => {

                    // Configure SwashBuckle to generate a documentation for v1
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FeChat Api", Version = "v1" });

                    // Define a custom filter to remove URLs without a version
                    c.DocumentFilter<RemoveUnversionedUrlsFilter>();

                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);


                });

            }

            // Set rules for routes
            builder.Services.AddRouting(options => {

                // All urls will be lowercase
                options.LowercaseUrls = true;

            });

            // A Cors Policy with allowed requests settings
            builder.Services.AddCors(options => {

                options.AddPolicy(name: "AllowOrigin",
                policy  => {
                    policy.WithOrigins(SiteUrl ?? string.Empty);
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });

            });

            // Add the AntiForgery service
            builder.Services.AddAntiforgery(options => {
                options.Cookie.Name = "X-CSRF-TOKEN";
            });    

            // Add HttpClient as a transient service
            builder.Services.AddHttpClient();        

            // Add the Mvc support
            builder.Services.AddMvc();            

            // Create an instance of WebApplication
            var app = builder.Build();

            // Redirects all HTTP urls to HTTPS
            //app.UseHttpsRedirection();

            // Verify if the app is in development
            if (app.Environment.IsDevelopment()) {

                // Enable Swagger support
                app.UseSwagger();

                // Enable Swagger UI
                app.UseSwaggerUI();

                // Enables the Developer Exception Page 
                app.UseDeveloperExceptionPage();

            }

            // Set the created Cors policy
            app.UseCors("AllowOrigin");

            // Enables authentication capabilities
            app.UseAuthentication();

            // Enables the autorization middleware for requests validation
            app.UseAuthorization();

            // Create endpoints from all controllers methods
            app.MapControllers();

            app.UseCookiePolicy();

            // Enable support for WebSocket requests
            app.UseWebSockets();

            // Catch all requests
            app.Use(async (context, next) => {

                // Check if is a WebSocket request
                if (context.WebSockets.IsWebSocketRequest) {

                    // Retrieve the service provider from the HttpContext
                    IServiceProvider serviceProvider = context.RequestServices;
                    
                    // Get the members repository from the service provider
                    IMembersRepository members = serviceProvider.GetService<IMembersRepository>()!;

                    // Get the messages repository from the service provider
                    IMessagesRepository messages = serviceProvider.GetService<IMessagesRepository>()!;

                    // Check if is requested web socket for user
                    if ( context.Request.Path == "/api/v1/user/websocket" ) {

                        // Handle WebSocket connection
                        await new UserWebSocketController().QueueRequest(context, members, messages);

                    } else {

                        // Handle WebSocket connection
                        await new WebSocketController().QueueRequest(context, messages);
                        
                    }

                } else {

                    // Handle non-WebSocket requests
                    await next();

                }

            });

            /*app.Use(async (context, next) =>
            {




            // Accessing all headers
            foreach (var header in context.Request.Headers)
            {
                string headerName = header.Key;
                string headerValue = header.Value;

                // Output or use the headerName and headerValue as needed
                Console.WriteLine($"{headerName}: {headerValue}");

            }

                if (context.Request.Headers.TryGetValue("X-CSRF-TOKEN", out var token))
                {


            var request = context.Request;

            // Get all the cookies from the request
            var cookies = request.Cookies;
        Console.WriteLine(context.Request.Cookies.Count);
            // Iterate through the cookies and display their names and values
            foreach (var cookie in cookies)
            {
                string cookieName = cookie.Key;
                string cookieValue = cookie.Value;

                // You can do whatever you want with the cookie information
                // For example, you can log it, return it as part of a response, etc.

                // Here, we'll just print the cookie information to the console
                Console.WriteLine($"Cookie Name: {cookieName}, Value: {cookieValue}");
            }

                    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();


                    
                    try
                    {
                        antiforgery.ValidateRequestAsync(context).Wait();
                        Console.WriteLine("Is Good");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        context.Response.StatusCode = 800;
                        return;
                    }

        Console.WriteLine(token);


                    
                    try
                    {
                        antiforgery.ValidateRequestAsync(context).Wait();
                        Console.WriteLine("Is Good");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        context.Response.StatusCode = 800;
                        return;
                    }
                }

                await next();
            });*/

            app.Use(async (context, next) => {

                // Default language container
                string lang = "en-US";

                // Check if is requested the administrator controllers
                if ( context.Request.Path.ToString().IndexOf("/api/v1/admin/") == 0 ) {

                    // Get current member data
                    ResponseDto<MemberDto> memberAccess = await new Access(context).IsAdminAsync(context.RequestServices.GetService<IMembersRepository>()!);

                    // Check if memberAccess contains an error message
                    if ( memberAccess.Result == null ) {

                        // Handle unauthorized access here
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
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

                } else if ( context.Request.Path.ToString().IndexOf("/api/v1/user") == 0 ) {

                    // Get current member data
                    ResponseDto<MemberDto> memberAccess = await new Access(context).IsUserAsync(context.RequestServices.GetService<IMembersRepository>()!);

                    // Check if memberAccess contains an error message
                    if ( memberAccess.Result == null ) {

                        // Handle unauthorized access here
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
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
                await next();

            });


            // Run the app
            app.Run();

        }

    }

}