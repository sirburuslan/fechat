/*
 * @class Startup
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-26
 *
 * This class contains the configuration of services and apps
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
    /// Startup Class
    /// </summary>
    public class Startup {

        /// <summary>
        /// Configuration container
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Environment container
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Startup Constructor
        /// </summary>
        /// <param name="configuration">App Configuration</param>
        /// <param name="env">Information about the web hosting environment</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env) {

            // Save configuration
            Configuration = configuration;

            // Save environment
            Environment = env;
            
        }


        /// <summary>
        /// Configure the app services
        /// </summary>
        /// <param name="services">Collection with services</param>
        public void ConfigureServices(IServiceCollection services) {

            // Get the app domain
            var AppDomain = Configuration.GetValue<string>("AppDomain");

            // Get the site url
            var SiteUrl = Configuration.GetValue<string>("SiteUrl");

            // Get the jwt key
            var JwtKey = Configuration.GetValue<string>("JwtKey");  

            // Add multiversions support
            services.AddApiVersioning().AddApiExplorer(options => {
            
                // Format string for the current API version
                options.GroupNameFormat = "'v'VVV";

                // Adds the version in the urls
                options.SubstituteApiVersionInUrl = true;

            });

            // Connect the DB Table Members
            services.AddDbContext<Db>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            // Register MyBackgroundService as a hosted service
            services.AddHostedService<Background>();

            // Register the library for cache storing
            services.AddMemoryCache();

            // Register service for Member
            services.AddScoped<Member>();

            // Register the Events Repository
            services.AddScoped<IEventsRepository, EventsRepository>();

            // Register the Members repository
            services.AddScoped<IMembersRepository, MembersRepository>();           

            // Register the Messages Repository
            services.AddScoped<IMessagesRepository, MessagesRepository>(); 

            // Register the Plans repository
            services.AddScoped<IPlansRepository, PlansRepository>();

            // Register the Settings repository
            services.AddScoped<ISettingsRepository, SettingsRepository>();

            // Register the Subscriptions repository
            services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();  

            // Register the Websites Repository
            services.AddScoped<IWebsitesRepository, WebsitesRepository>();                            

            // Register the localization for the app's strings
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Configure the session state
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

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
            services.AddControllers().ConfigureApiBehaviorOptions(options => {

                options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState) {
                        
                    ContentTypes = {
                        Application.Json
                    },
                    StatusCode = 200
                    
                };

            })
            .AddXmlSerializerFormatters();

            // Verify if the app is in development
            if ( Environment.IsDevelopment() ) {

                // Parse information about endpoints
                services.AddEndpointsApiExplorer();

                // Enable the service which generates a documentation
                services.AddSwaggerGen(c => {

                    // Generate unique Ids
                    c.CustomSchemaIds(type => type.FullName);

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
            services.AddRouting(options => {

                // All urls will be lowercase
                options.LowercaseUrls = true;

            });

            // A Cors Policy with allowed requests settings
            services.AddCors(options => {

                options.AddPolicy(name: "AllowOrigin",
                policy  => {
                    policy.WithOrigins(SiteUrl ?? string.Empty);
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });

            });

            // Add the AntiForgery service
            services.AddAntiforgery(options => {
                options.FormFieldName = "AntiforgeryFieldname";
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.SuppressXFrameOptionsHeader = false;
            });

            // Add HttpClient as a transient service
            services.AddHttpClient();        

            // Add the Mvc support
            services.AddMvc();      

        }

        /// <summary>
        /// Configure the HTTP request pipeline
        /// </summary>
        /// <param name="app">Application's request pipeline</param>
        public void Configure(IApplicationBuilder app) {

            // Redirects all HTTP urls to HTTPS
            app.UseHttpsRedirection();

            // Verify if the app is in development
            if (Environment.IsDevelopment()) {

                // Enable Swagger support
                app.UseSwagger();

                // Enable Swagger UI
                app.UseSwaggerUI();

                // Enables the Developer Exception Page 
                app.UseDeveloperExceptionPage();

            }

            // Get the site url
            var SiteUrl = Configuration.GetValue<string>("SiteUrl");

            // Enables authentication capabilities
            app.UseAuthentication();


            app.UseRouting(); // Enable routing before using endpoints

            // Set the created Cors policy
            app.UseCors(options =>
            {
                options.WithOrigins(SiteUrl ?? string.Empty)
                    .AllowAnyMethod() // Adjust allowed methods (GET, POST, etc.)
                    .AllowAnyHeader(); // Adjust allowed headers as needed
            });            

            // Enables the autorization middleware for requests validation
            app.UseAuthorization();

            app.UseEndpoints(endpoints => // Use the injected IEndpointRouteBuilder
            {
                endpoints.MapControllers(); // Register controllers
            });

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

        }

    }

}