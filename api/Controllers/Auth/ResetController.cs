/*
 * @class Reset Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used for members password reset
 */

// Namespace for Auth Controllers
namespace FeChat.Controllers.Auth {

    // Use Web system for html sanitizing
    using System.Web;

    // Text Enconding for Javascript sanitizing
    using System.Text.Encodings.Web;   

    // Use the Mvc to get the controller
    using Microsoft.AspNetCore.Mvc;

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

    // Use the Repositories interface to get the member repository
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the General utils classes
    using FeChat.Utils.General;

    /// <summary>
    /// This controller creates a session
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth/[controller]")]
    public class ResetController : Controller {

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
        public ResetController(IConfiguration configuration) {

            // Save the configuration
            _configuration = configuration;
            
        }

        /// <summary>
        /// This method creates a reset code
        /// </summary>
        /// <param name="member">Data transfer object with member information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Reset([FromBody] MemberDto member, IMembersRepository membersRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get the email
            ResponseDto<MemberDto> memberEmail = await membersRepository.GetMemberEmailAsync(member);

            // Verify if a member was found
            if ( memberEmail.Result == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("EmailNotFound")
                });

            }

            // Check if reset time exists
            if ( memberEmail.Result.ResetTime > 0 ) {

                // Check if the reset code request was done recently
                if ( ((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - memberEmail.Result.ResetTime) < 300 ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("ResetCodeRequestedAlready")
                    });
                    
                }

            }

            // Create a reset code
            string resetCode = memberEmail.Result.MemberId + Guid.NewGuid().ToString()[..7];

            // Add reset code to the dto
            MemberDto memberDto = new() {
                MemberId = memberEmail.Result.MemberId,
                ResetCode = resetCode
            };
            
            // Save the reset code
            ResponseDto<bool> saveCode = await membersRepository.UpdateEmailAsync(memberDto);

            // Verify if the reset code was saved
            if ( saveCode.Result ) {

                // Create the json response
                var response = new {
                    success = true,
                    message = new Strings().Get("ResetCodeWasSent")
                };

                // Return a json with response
                return new JsonResult(response);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = (saveCode.Message != null)?saveCode.Message:new Strings().Get("ErrorOccurred")
                };

                // Return a json
                return new JsonResult(response);  

            }

        }

        /// <summary>
        /// This method validates a reset code
        /// </summary>
        /// <param name="code">Reset code which should be validated</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [HttpGet("validate/{code}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Validate(string code, IMembersRepository membersRepository) {

            // Remove unwanted characters from the code
            string safeCode = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(code ?? string.Empty)).Trim();

            // Get the member by reset code
            ResponseDto<MemberDto> memberCode = await membersRepository.GetMemberByCodeAsync(safeCode);

            // Verify if a member was found
            if ( memberCode.Result == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = memberCode.Message
                });

            }

            // Check if reset time exists
            if ( memberCode.Result.ResetTime > 0 ) {

                // Check if the reset code is expired
                if ( ((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - memberCode.Result.ResetTime) > 300 ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("ResetCodeExpired")
                    });
                    
                }

            }

            // Return success response
            return new JsonResult(new {
                success = true
            });

        }

        /// <summary>
        /// Update the member password
        /// </summary>
        /// <param name="memberDto">Contains the received information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("change-password")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> UpdateMemberPassword([FromBody] MemberDto memberDto, IMembersRepository membersRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Verify if memberDto.ResetCode exists
            if ( memberDto.ResetCode == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ResetCodeMissing")
                });

            }

            // Get the member by reset code
            ResponseDto<MemberDto> memberCode = await membersRepository.GetMemberByCodeAsync(memberDto.ResetCode);

            // Verify if a member was found
            if ( memberCode.Result == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = memberCode.Message
                });

            }

            // Check if reset time exists
            if ( memberCode.Result.ResetTime > 0 ) {

                // Check if the reset code is expired
                if ( ((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - memberCode.Result.ResetTime) > 300 ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("ResetCodeExpired")
                    });
                    
                }

            }

            // Verify if password exists
            if ( memberDto.Password == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PleaseEnterPassword")
                });

            } 
            
            // Verify if repeat password exists
            if ( memberDto.RepeatPassword == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PleaseEnterRepeatPassword")
                });

            }  

            // Verify if the password is correct
            if ( memberDto.Password != memberDto.RepeatPassword ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PasswordRepeatPasswordNotMatch")
                });
                
            }

            // Create the member data
            MemberDto memberData = new() {
                MemberId = memberCode.Result.MemberId,
                Password = memberDto.Password.Trim()
            };

            // Member the member's data
            ResponseDto<bool> UpdateMember = await membersRepository.UpdatePasswordAsync(memberData);

            // Verify if member exists
            if ( UpdateMember.Result ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("PasswordWasChanged")
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = UpdateMember.Message ?? new Strings().Get("PasswordWasNotChanged")
                });

            }

        }

    }

}