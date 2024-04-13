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

    // System Namespaces
    using System.IdentityModel.Tokens.Jwt; 
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Utils.Configuration;
    using Utils.General;
    using Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// This controller creates a session
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth/[controller]")]
    public class SignInController : Controller {

        /// <summary>
        /// App Settings container.
        /// </summary>
        private readonly AppSettings _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInController"/> class.
        /// </summary>
        /// <param name="options">All App Options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public SignInController(IOptions<AppSettings> options) {

            // Save the configuration
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), new Strings().Get("OptionsNotFound"));
            
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
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSettings.Key ?? string.Empty));

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
                    issuer: _options.JwtSettings.Issuer,
                    audience: _options.JwtSettings.Audience,
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