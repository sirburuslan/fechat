/*
 * @class Members Authentification Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @authd 2024-03-15
 *
 * This class is used to sign in
 */

// Namespace for Members Account repositories
namespace FeChat.Models.Repositories.Members.Account {

    // Use the ASP.NET Core Identity
    using Microsoft.AspNetCore.Identity;

    // Use the Entity Framework
    using Microsoft.EntityFrameworkCore;

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;

    // Use the Options for password hash generation
    using Microsoft.Extensions.Options;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use Dtos for Members
    using FeChat.Models.Dtos.Members;

    // Use the Entities
    using FeChat.Models.Entities.Members;

    // Get the Configuration Utils for connection
    using FeChat.Utils.Configuration;

    // Get the General Utils for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Members Auth Repository pattern
    /// </summary>
    public class AuthRepository {

        /// <summary>
        /// Members table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Members Auth Repository constructor
        /// </summary>
        /// <param name="db">Database connection</param>
        public AuthRepository(Db db) {

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Check if the member and password is correct
        /// </summary>
        /// <param name="memberDto">Member dto with the member's data</param>
        /// <returns>Response with member data</returns>
        public async Task<ResponseDto<MemberDto>> SignIn(MemberDto memberDto) {

            try {

                // Get the member by email
                MemberDto member = await _context.Members
                .Select(m => new MemberDto {
                    MemberId = m.MemberId,
                    Email = m.Email!,
                    Role = m.Role,
                    Password = m.Password!
                })
                .FirstAsync(u => u.Email == memberDto.Email);

                // Verify if the member exists
                if (member == null) {

                    // Create the response
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("AccountNotFound")
                    };

                }

                // Create a MemberEntity for password hashing
                MemberEntity memberEntity = new() {
                    Password = member.Password
                };

                // Init the password hasher
                var passwordHasher = new PasswordHasher<MemberEntity>(Options.Create(new PasswordHasherOptions{CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3}));            

                // Verify if password is valid
                var result = passwordHasher.VerifyHashedPassword(memberEntity, member.Password ?? string.Empty, memberDto.Password!);

                // Verify if result is Success
                if ( result == PasswordVerificationResult.Success ) {

                    // Create the response
                    return new ResponseDto<MemberDto> {
                        Result = member,
                        Message = new Strings().Get("SuccessSignIn")
                    };

                } else {

                    // Create the response
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("IncorrectEmailPassword")
                    };

                }                

            } catch (InvalidOperationException e) {

                // Display error message
                Console.WriteLine("Error message: " + e.Message);

                // Create the response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = new Strings().Get("ErrorOccurred")
                };

            }

        }

    }

}