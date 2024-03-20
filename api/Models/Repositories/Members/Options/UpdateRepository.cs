/*
 * @class Member Options Update Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used to update the member's options
 */

// Namespace for Members Options Repositories model
namespace FeChat.Models.Repositories.Members.Options {
    
    // Use Cache to store the quries
    using Microsoft.Extensions.Caching.Memory;

    // Use the entities
    using FeChat.Models.Entities.Members;
    
    // Use the configuration for Db instance
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Members Options Update Repository
    /// </summary>
    public class UpdateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Db connection container
        /// </summary>
        private readonly Db _context;

        /// <summary>
        /// Members Options Update constructor
        /// </summary>
        /// <param name="memoryCache">Memory cache instane</param>
        /// <param name="db">Db connection</param>
        public UpdateRepository(IMemoryCache memoryCache, Db db) {

            // Set memory cache
            _memoryCache = memoryCache;

            // Set the Db connection
            _context = db;

        }

        /// <summary>
        /// Update bulk options
        /// </summary>
        /// <param name="optionsList">Members options list</param>
        /// <returns>Boolean response</returns>
        public bool UpdateOptions(List<MemberOptionsEntity> optionsList) {

            try {

                // Update the entities in the database
                _context.MembersOptions.UpdateRange(optionsList); // Right now UpdateRangeAsync is not available

                // Save the options
                int save = _context.SaveChanges();

                // Create the cache key
                string cacheKey = "fc_member_options_" + optionsList.First().MemberId;

                // Delete the cache
                _memoryCache.Remove(cacheKey);

                return save > 0;

            } catch (InvalidOperationException) {

                return false;

            }

        }

    }

}