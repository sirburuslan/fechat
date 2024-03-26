/*
 * @class Google Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-19
 *
 * This class is used to register members with Google Api
 */

// Namespace for Auth Controllers
namespace FeChat.Controllers.Auth {

    // Use the Jwt classes
    using System.IdentityModel.Tokens.Jwt;

    // Use the Text encoding
    using System.Text;

    // Use Web for HTML decoding
    using System.Web;

    // Use the Claims classes
    using System.Security.Claims;
    
    // Use the Mvc to get the controller
    using Microsoft.AspNetCore.Mvc;

    // Use Tokens to generate tokens
    using Microsoft.IdentityModel.Tokens;

    // Import the Versioning library
    using Asp.Versioning;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use Dtos for Members
    using FeChat.Models.Dtos.Members;

    // Using the Entities for Members
    using FeChat.Models.Entities.Members;

    // Use General Utils
    using FeChat.Utils.General;

    // Use the Events Repositories
    using FeChat.Utils.Interfaces.Repositories.Events;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the Settings Repositories
    using FeChat.Utils.Interfaces.Repositories.Settings;

    /// <summary>
    /// This controller is used to login or sign up user with Google
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth/[controller]")]
    public class GoogleController : Controller {

        /// <summary>
        /// App configuration container
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="configuration">
        /// App configuration
        /// </param>
        public GoogleController(IConfiguration configuration) {

            // Save the configuration
            _configuration = configuration;
            
        }

        /// <summary>
        /// This method validates the member's data and creates an account
        /// </summary>
        /// <param name="googleDto">Google data</param>
        /// <param name="settingsRepository">An instance for the settings repository</param>
        /// <param name="membersRepository">Contains a session to the Members repository</param>
        /// <param name="eventsRepository">Contains a session to the Events repository</param>
        /// <returns>Success or error message</returns>
        [HttpGet]
        public async Task<IActionResult> Access([FromBody] GoogleDto googleDto, ISettingsRepository settingsRepository, IMembersRepository membersRepository, IEventsRepository eventsRepository) {

            // Get the options saved in the database
            ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await settingsRepository.OptionsListAsync();

            // Verify if options exists
            if ( savedOptions.Result != null ) {

                // Lets create a new dictionary list
                Dictionary<string, string> optionsList = new();

                // Get options length
                int optionsLength = savedOptions.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Add option to the dictionary
                    optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                }

                // Get GoogleAuthEnabled
                optionsList.TryGetValue("GoogleAuthEnabled", out string? GoogleAuthEnabled);

                // Get GoogleClientId
                optionsList.TryGetValue("GoogleClientId", out string? GoogleClientId);

                // Get GoogleClientSecret
                optionsList.TryGetValue("GoogleClientSecret", out string? GoogleClientSecret);     

                // Verify if the google api is configured
                if ( (GoogleAuthEnabled != null) && (GoogleAuthEnabled == "1") && (GoogleClientId != null) && (GoogleClientSecret != null) ) {

                    // Init the http client
                    using HttpClient httpClient = new();

                    // Create the content
                    Dictionary<string, string> content = new()
                    {
                        { "client_id", GoogleClientId },
                        { "client_secret", GoogleClientSecret },
                        { "code", googleDto.Code ?? string.Empty },
                        { "redirect_uri", _configuration["SiteUrl"] + "auth/google/callback" },
                        { "grant_type", "authorization_code" },
                        { "access_type", "offline" },
                        { "prompt", "consent" }
                    };

                    // Encode the content
                    FormUrlEncodedContent requestContent = new(content);

                    // Set request
                    HttpResponseMessage responseToken = await httpClient.PostAsync("https://www.googleapis.com/oauth2/v4/token", requestContent);

                    // Verify if has been occurred an error
                    if ( !responseToken.IsSuccessStatusCode ) {

                        // Request failed
                        string errorMessage = await responseToken.Content.ReadAsStringAsync();

                        // Decode the error message
                        dynamic errorMessageDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(errorMessage)!;  

                        // Return error response
                        return new JsonResult(new {
                            success = false,
                            message = HttpUtility.HtmlDecode(errorMessageDecode["error_description"].ToString())
                        });

                    }

                    // Read the response
                    string responseTokenJson = await responseToken.Content.ReadAsStringAsync();

                    // Decode the Response
                    dynamic responseTokenDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(responseTokenJson)!;

                    // Verify if access token exists
                    if ( (responseTokenDecode == null) || !responseTokenDecode!.ContainsKey("access_token") ) {

                        // Return error response
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("AccessCodeNotGenerated")
                        });
                        
                    }

                    // Initialize a new Http Client session
                    using HttpClient accountDataRequest = new();

                    // Request the account data using the token
                    HttpResponseMessage accountDataMessage = await accountDataRequest.GetAsync("https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + responseTokenDecode!["access_token"]);

                    // Verify if has been occurred an error
                    if ( !accountDataMessage.IsSuccessStatusCode ) {

                        // Request failed
                        string errorMessage = await accountDataMessage.Content.ReadAsStringAsync();

                        // Decode the error message
                        dynamic errorMessageDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(errorMessage)!;  

                        // Return error response
                        return new JsonResult(new {
                            success = false,
                            message = HttpUtility.HtmlDecode(errorMessageDecode["error_description"].ToString())
                        });

                    }

                    // Get request
                    string accountJson = await accountDataMessage.Content.ReadAsStringAsync(); 

                    // Decode the Response
                    dynamic responseAccountDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(accountJson)!;

                    // Get member option
                    ResponseDto<OptionDto> responseDto = await membersRepository.GetMemberOptionWithGoogle(responseAccountDecode["id"].ToString());

                    // Verify if member option exists
                    if ( responseDto.Result == null ) {

                        // Create the member
                        NewMemberDto newMemberDto = new() {
                            Email = responseAccountDecode["email"].ToString(),
                            Password = responseAccountDecode["id"].ToString(),
                            Role = 1
                        };

                        // Create member
                        ResponseDto<MemberDto> createMember = await membersRepository.CreateMemberAsync(newMemberDto);

                        // Verify if the account was created
                        if ( createMember.Result == null ) {

                            // Return a json
                            return new JsonResult(new {
                                success = false,
                                message = createMember.Message
                            });                

                        }

                        // Options to save container
                        List<MemberOptionsEntity> optionsSave = new();  

                        // Create the option
                        MemberOptionsEntity option = new() {
                            MemberId = createMember.Result.MemberId,
                            OptionName = "GoogleId",
                            OptionValue = responseAccountDecode["id"].ToString()
                        };

                        // Add option to the save list
                        optionsSave!.Add(option);

                        // Save the option
                        await membersRepository.SaveOptionsAsync(optionsSave);

                        // Prepare and define the secret key
                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"] ?? string.Empty));

                        // Create aa signature with the key
                        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                        // Create a new list with token data as claims
                        var claims = new List<Claim>() {
                            new("MemberId", createMember.Result.MemberId.ToString() ?? string.Empty, ClaimValueTypes.String),
                            new(JwtRegisteredClaimNames.Sub, "membername", ClaimValueTypes.String),
                            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String)
                        };

                        // Create the token
                        var token = new JwtSecurityToken(
                            issuer: _configuration["AppDomain"],
                            audience: _configuration["AppDomain"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(720),
                            signingCredentials: credentials
                        );

                        // Initialize the JwtSecurityTokenHandler class which validates, handles and creates access tokens
                        var tokenHandler = new JwtSecurityTokenHandler();
                        
                        // Create the json response
                        var response = new {
                            success = true,
                            message = createMember.Message,
                            member = new {
                                createMember.Result.MemberId,
                                createMember.Result.Role,
                                Token = tokenHandler.WriteToken(token)
                            }
                        };

                        // Return a json with response
                        return new JsonResult(response);

                    } else {

                        // Member the member's data
                        ResponseDto<MemberDto> memberDto = await membersRepository.GetMemberAsync(responseDto.Result.MemberId);

                        // Verify if member exists
                        if ( memberDto.Result != null ) {

                            // Prepare and define the secret key
                            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"] ?? string.Empty));

                            // Create aa signature with the key
                            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                            // Create a new list with token data as claims
                            var claims = new List<Claim>() {
                                new("MemberId", memberDto.Result.MemberId.ToString() ?? string.Empty, ClaimValueTypes.String),
                                new(JwtRegisteredClaimNames.Sub, "membername", ClaimValueTypes.String),
                                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String)
                            };

                            // Create the token
                            var token = new JwtSecurityToken(
                                issuer: _configuration["AppDomain"],
                                audience: _configuration["AppDomain"],
                                claims: claims,
                                expires: DateTime.UtcNow.AddHours(720),
                                signingCredentials: credentials
                            );

                            // Initialize the JwtSecurityTokenHandler class which validates, handles and creates access tokens
                            var tokenHandler = new JwtSecurityTokenHandler();
                            
                            // Create the json response
                            var response = new {
                                success = true,
                                message = memberDto.Message,
                                member = new {
                                    memberDto.Result.MemberId,
                                    memberDto.Result.Role,
                                    Token = tokenHandler.WriteToken(token)
                                }
                            };

                            // Return a json with response
                            return new JsonResult(response);

                        } else {

                            // Return error response
                            return new JsonResult(new {
                                success = false,
                                message = memberDto.Message
                            });

                        }

                    }

                }

            }

            // Return error response
            return new JsonResult(new {
                success = false,
                message = new Strings().Get("GoogleNotConfigured")
            });

        }

    }
    
}