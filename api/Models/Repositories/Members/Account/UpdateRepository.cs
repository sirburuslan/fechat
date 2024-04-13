/*
 * @class Members Update Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update members
 */

// Namespace for Members Account repositories
namespace FeChat.Models.Repositories.Members.Account {

    // System Namespaces
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Models.Entities.Members;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Members Update Repository pattern
    /// </summary>
    public class UpdateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Members table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Members Update Repository constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Database connection</param>
        public UpdateRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Update a member
        /// </summary>
        /// <param name="memberDto">Member entity with the member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdateMemberAsync(MemberEntity memberDto) {

            try {

                // Update the entities in the database
                _context.Members.UpdateRange(memberDto);

                // Save the changes
                int save = await _context.SaveChangesAsync();

                // Check if the member's data was updated
                if ( save > 0 ) {

                    // Create the cache key
                    string cacheKey = "fc_member_" + memberDto.MemberId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);  

                    // Remove the caches for members group
                    new Cache(_memoryCache).Remove("members");                  

                    // Return error response
                    return new ResponseDto<bool> {
                        Result = true,
                        Message = null
                    };

                } else {

                    // Return error response
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
                    };

                }

            } catch (InvalidOperationException e) {

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Update a member email
        /// </summary>
        /// <param name="memberDto">Member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdateEmailAsync(MemberDto memberDto) {

            try {

                // Find the item you want to update
                MemberEntity? memberData = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberDto.MemberId);

                // Verify if the member was found
                if (memberData!= null) {

                    // Update the reset code
                    memberData.ResetCode = memberDto.ResetCode;

                    // Update the reset time
                    memberData.ResetTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();                    

                    // Mark the item as modified
                    _context.Entry(memberData).State = EntityState.Modified;

                    // Save changes to the database
                    int saveChanges = await _context.SaveChangesAsync();

                    // Verify if the changes were saved
                    if ( saveChanges > 0 ) {

                        // Remove the caches for members group
                        new Cache(_memoryCache).Remove("members");        

                        // Return error response
                        return new ResponseDto<bool> {
                            Result = true,
                            Message = null
                        };

                    }

                }

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = null
                };

            } catch (InvalidOperationException e) {

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };                     

            }

        }

        /// <summary>
        /// Update a member password
        /// </summary>
        /// <param name="memberDto">Member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdatePasswordAsync(MemberDto memberDto) {

            try {

                // Init the password hasher
                var passwordHasher = new PasswordHasher<MemberEntity>(Options.Create(new PasswordHasherOptions{CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3}));

                // Find the item you want to update
                MemberEntity? memberData = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberDto.MemberId);

                // Verify if the member was found
                if (memberData!= null) {

                    // Update the item
                    memberData.Password = passwordHasher.HashPassword(memberData, memberDto.Password!.Trim());

                    // Empty the reset code
                    memberData.ResetCode = "";

                    // Mark the item as modified
                    _context.Entry(memberData).State = EntityState.Modified;

                    // Save changes to the database
                    int passwordUpdated = await _context.SaveChangesAsync();

                    // Verify if the password was updated
                    if ( passwordUpdated > 0 ) {

                        // Return error response
                        return new ResponseDto<bool> {
                            Result = true,
                            Message = null
                        };

                    }

                }

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = null
                };

            } catch (InvalidOperationException e) {

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };                     

            }

        }

    }

}