/*
 * @class Members Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create members
 */

// Namespace for Members Account repositories
namespace FeChat.Models.Repositories.Members.Account {

    // System Namespaces
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Models.Entities.Members;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Members Create Repository pattern
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Members table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Members Create Repository constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Database connection</param>
        public CreateRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Create a member
        /// </summary>
        /// <param name="newMemberDto">Member dto with the member's data</param>
        /// <returns>Response with member data</returns>
        public async Task<ResponseDto<MemberDto>> CreateMemberAsync(NewMemberDto newMemberDto) {

            try {

                // Verify if the email is already registered
                if ( _context.Members.Any(db_member => db_member.Email == newMemberDto.Email!.Trim()) ) {

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("EmailFound")
                    };                

                }

                // Init the password hasher
                var passwordHasher = new PasswordHasher<MemberEntity>(Options.Create(new PasswordHasherOptions{CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3}));

                // Init the member entity to use for password hashing
                MemberEntity memberEntity = new();

                // Init the Member entity
                MemberEntity entity = new()
                {

                    // Set the member's first name
                    FirstName = (newMemberDto.FirstName != null)?newMemberDto.FirstName.Trim():"",

                    // Set the member's last name
                    LastName = (newMemberDto.LastName != null)?newMemberDto.LastName.Trim():"",                    

                    // Set the member's email
                    Email = newMemberDto.Email!.Trim(),

                    // Set the member's password
                    Password = passwordHasher.HashPassword(memberEntity, newMemberDto.Password!.Trim()),

                    // Set the member's role
                    Role = newMemberDto.Role,

                    // Set the joined time
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()

                };

                // Add the entity to the database
                _context.Members.Add(entity);

                // Save the changes
                int saveMember = await _context.SaveChangesAsync();

                // Verify if the member was created
                if ( saveMember > 0 ) {

                    // Remove the caches for members group
                    new Cache(_memoryCache).Remove("members"); 

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = new MemberDto {
                            MemberId = entity.MemberId
                        },
                        Message = new Strings().Get("AccountCreated")
                    };  

                } else {

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("AccountNotCreated")
                    };  
                    
                }

            } catch (InvalidOperationException e) {

                // Return response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = e.Message
                };                   

            }

        }

    }

}