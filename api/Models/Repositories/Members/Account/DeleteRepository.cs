/*
 * @class Members Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete members
 */

// Namespace for Members Account repositories
namespace FeChat.Models.Repositories.Members.Account {

    // Use the Entity Framework Core to extend the LINQ features
    using Microsoft.EntityFrameworkCore;

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use the Entity for Members
    using FeChat.Models.Entities.Members;

    // Get the Configuration Utils for connection
    using FeChat.Utils.Configuration;

    // Get the General Utils for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Members Delete Repository pattern
    /// </summary>
    public class DeleteRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Members table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Members Delete Repository constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Database connection</param>
        public DeleteRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Delete member
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>Bool true if the member was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteMemberAsync(int memberId) {

            try {

                // Select the member by id
                var member = _context.Members.Where(m => m.MemberId == memberId);

                // Delete the member
                _context.Members.RemoveRange(member);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {

                    // Get all members options
                    List<MemberOptionsEntity>? memberOptionsEntities = await _context.MembersOptions.Where(o => o.MemberId == memberId).ToListAsync();

                    // Verify if members options exists
                    if ( (memberOptionsEntities != null) && (memberOptionsEntities.Count > 0) ) {

                        // Remove bulk options
                        _context.MembersOptions.RemoveRange(memberOptionsEntities);

                        // Save changes
                        await _context.SaveChangesAsync();

                    }

                    // Create the cache key
                    string cacheKey = "fc_member_" + memberId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);

                    // Remove the caches for members group
                    new Cache(_memoryCache).Remove("members"); 

                    // Return the success message
                    return new ResponseDto<bool> {
                        Result = true,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
                    };
                    
                }

            } catch ( InvalidOperationException e ) {

                // Return the error message
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };

            }

        }

    }

}