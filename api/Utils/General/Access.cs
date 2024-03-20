/*
 * @class Access Validator
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class verifies if the token is valid and if the member has access
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // Use the Authentication to get the token from http context
    using Microsoft.AspNetCore.Authentication;

    // Use the General Dto classes
    using FeChat.Models.Dtos;

    // Use the Member Dto classes
    using FeChat.Models.Dtos.Members;

    // Use the Members repository
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Access Validator
    /// </summary>
    public class Access {

        /// <summary>
        /// Http Context container
        /// </summary>
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Access Constructor
        /// </summary>
        /// <param name="httpContext">Http context</param>
        public Access(HttpContext httpContext) {

            // Set http context
            _httpContext = httpContext;

        }

        /// <summary>
        /// Check if the member is admin
        /// </summary>
        /// <returns>Member dto or error message</returns>
        public async Task<ResponseDto<MemberDto>> IsAdminAsync(IMembersRepository members) {

            // Retrieve the access token from the HttpContext
            string accessToken = _httpContext.GetTokenAsync("access_token").Result ?? string.Empty;

            // Check if access token exists
            if (accessToken == null) {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("NoTokenFound")
                };

            }

            // Get the member's ID
            string MemberId = new Tokens().GetTokenData(accessToken ?? string.Empty, "MemberId");

            // Verify if MemberId has no value
            if (MemberId == "") {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("TokenNoValid")
                };

            }

            // Get the email
            ResponseDto<MemberDto> member = await members.GetMemberAsync(int.Parse(MemberId));

            // Verify if member exists
            if ( member.Result == null ) {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("AccountNotFound")
                };

            }

            // Verify if member is not administrator
            if ( member.Result.Role != 0 ) {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("NoPermissionsForAction")
                };

            }

            // Return success response
            return new ResponseDto<MemberDto> {
                Result = new MemberDto {
                    MemberId = member.Result.MemberId,
                    Role = member.Result.Role,
                    Language = ((member.Result.Language != null) && (member.Result.Language != "english"))?"ro-RO":"en-US"
                },
                Message = null
            };

        }

        /// <summary>
        /// Check if the member is user
        /// </summary>
        /// <returns>Member dto or error message</returns>
        public async Task<ResponseDto<MemberDto>> IsUserAsync(IMembersRepository members) {

            // Retrieve the access token from the HttpContext
            string accessToken = _httpContext.GetTokenAsync("access_token").Result ?? string.Empty;

            // Check if access token exists
            if (accessToken == null) {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("NoTokenFound")
                };

            }

            // Get the member's ID
            string MemberId = new Tokens().GetTokenData(accessToken ?? string.Empty, "MemberId");

            // Verify if MemberId has no value
            if (MemberId == "") {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("TokenNoValid")
                };

            }

            // Get the email
            ResponseDto<MemberDto> member = await members.GetMemberAsync(int.Parse(MemberId));

            // Verify if member exists
            if ( member.Result == null ) {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("AccountNotFound")
                };

            }

            // Verify if member is not administrator
            if ( member.Result.Role != 1 ) {

                // Return error response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("NoPermissionsForAction")
                };

            }

            // Return success response
            return new ResponseDto<MemberDto> {
                Result = new MemberDto {
                    MemberId = member.Result.MemberId,
                    Role = member.Result.Role,
                    Language = ((member.Result.Language != null) && (member.Result.Language != "english"))?"ro-RO":"en-US"
                },
                Message = null
            };

        }

    }

}