/*
 * @class Startup
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-04-07
 *
 * This class contains the configuration of services and apps
 */

// App namespace
namespace FeChat {
    
    // System Namespaces
    using System.Reflection;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;   
    using Microsoft.OpenApi.Models;
    

    // App Namespaces
    using Utils.Configuration;
    using Utils.Extensions;
    using Utils.General;

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

            // Bind app settings to the app settings class
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // Add app settings class in the IOptions
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<AppSettings>>().Value);

            // Connect the DB Table Members
            services.AddDbContext<Db>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            // Register MyBackgroundService as a hosted service
            services.AddHostedService<Background>();

            // Register the library for cache storing
            services.AddMemoryCache();

            // Register the library for responses compression
            services.AddResponseCompression();

            // Register the WebSocket Middleware
            services.AddTransient<WebSocketMiddleware>();

            // Register service for Member
            services.AddScoped<Member>();

            // Register all repositories
            services.RegisterRepositories(Assembly.GetExecutingAssembly());                      

            // Register the localization for the app's strings
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Add multiversions support
            services.AddApiVersioning().AddApiExplorer(options => {
            
                // Format string for the current API version
                options.GroupNameFormat = "'v'VVV";

                // Adds the version in the urls
                options.SubstituteApiVersionInUrl = true;

            });

            // Configure the session state
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetValue<string>("AppSettings:JwtSettings:Issuer"),
                    ValidAudience = Configuration.GetValue<string>("AppSettings:JwtSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("AppSettings:JwtSettings:Key") ?? string.Empty))
                };
                
            });

            // Add services for controllers
            services.AddControllers(options =>
            {
                options.Filters.Add<JsonExceptionFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState)
                {
                    ContentTypes = {
                        "application/json"
                    },
                    StatusCode = 200
                };
            });

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

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
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
                    policy.WithOrigins(Configuration.GetValue<string>("AppSettings:SiteUrl") ?? string.Empty);
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });

            });

            // Register the Language Middleware
            services.AddSingleton<LanguageMiddleware>();

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

            // Verify if the app is in development
            if (Environment.IsDevelopment()) {

                // Enable Swagger support
                app.UseSwagger();

                // Enable Swagger UI
                app.UseSwaggerUI();

            } else {

                // Enable the HTTP Strict Transport Security policy
                app.UseHsts();

            }

            // Redirects all HTTP urls to HTTPS
            app.UseHttpsRedirection();

            // Enable support for WebSocket requests
            app.UseWebSockets();

            // Use the Responses Compression
            app.UseResponseCompression();

            // Use the Websocket Middleware
            app.UseMiddleware<WebSocketMiddleware>();

            // Enables authentication capabilities
            app.UseAuthentication();

            // Enable routing before using endpoints
            app.UseRouting(); 

            // Set the created Cors policy
            app.UseCors(options =>
            {
                options.WithOrigins(Configuration.GetValue<string>("AppSettings:SiteUrl") ?? string.Empty)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }); 

            // Use the Language Middleware
            app.UseMiddleware<LanguageMiddleware>();      

            // Enables the autorization middleware for requests validation
            app.UseAuthorization();

            // Use the injected IEndpointRouteBuilder
            app.UseEndpoints(endpoints => 
            {
                // Register controllers
                endpoints.MapControllers(); 
            });

            app.UseCookiePolicy();

        }

    }

}