/*
 * @class Sign In Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used for members sign in
 */

// Namespace for Auth Controllers
namespace FeChat.Controllers.Auth {

    // Use the Claims for member saving information
    using System.Security.Claims;

    // Use the Text namespace for key encoding
    using System.Text;

    // Use the Jwt namespace for JWT tokens creation
    using System.IdentityModel.Tokens.Jwt;    

    // Use the Mvc to get the controller
    using Microsoft.AspNetCore.Mvc;

    // Use the Tokens library for tokens creation
    using Microsoft.IdentityModel.Tokens;

    // Use the Cors to control the requests origins
    using Microsoft.AspNetCore.Cors;

    // Use the Authorization feature to allow guests access
    using Microsoft.AspNetCore.Authorization;

    // Use The Versioning namespace for api versioning
    using Asp.Versioning;

    // Use General Dto
    using FeChat.Models.Dtos;

    // Use Members dto to validate and hold member data
    using FeChat.Models.Dtos.Members;

    // Use General Utils
    using FeChat.Utils.General;

    // Use the Repositories interface to get the member repository
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// This controller creates a session
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth/[controller]")]
    public class SignInController : Controller {

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
        public SignInController(IConfiguration configuration) {

            // Save the configuration
            _configuration = configuration;
            
        }

        /// <summary>
        /// This methods verifies if the member's information is valid
        /// </summary>
        /// <param name="member">Data transfer object with member information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success message and member's data or error message</returns>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> SignIn([FromBody] MemberDto member, IMembersRepository membersRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get all members
            ResponseDto<ElementsDto<MemberDto>> membersList = await membersRepository.GetMembersAsync(new SearchDto {
                Page = 1,
                Search = ""
            });

            // Verify if members exists
            if ( membersList.Result == null ) {

                // Prepare the default member
                NewMemberDto newMemberDto = new() {
                    Email = "administrator@example.com",
                    Password = "12345678",
                    Role = 0
                };

                // Create the default member
                await membersRepository.CreateMemberAsync(newMemberDto);

            }

            // Checks if the member data is correct
            ResponseDto<MemberDto> memberDto = await membersRepository.SignIn(member);

            // Verify if signIn is valid
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

                // Create a error response
                var response = new {
                    success = false,
                    message = new Strings().Get("IncorrectEmailPassword")
                };

                // Return a json
                return new JsonResult(response);  

            }

        }

    }

}