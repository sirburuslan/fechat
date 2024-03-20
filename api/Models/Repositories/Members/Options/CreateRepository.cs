/*
 * @class Member Options Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used to create the member's options
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
    /// Members Options Read Repository
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Db connection container
        /// </summary>
        private readonly Db _context;

        /// <summary>
        /// Members Options Create constructor
        /// </summary>
        /// <param name="memoryCache">Memory cache instane</param>
        /// <param name="db">Db connection</param>
        public CreateRepository(IMemoryCache memoryCache, Db db) {

            // Set memory cache
            _memoryCache = memoryCache;

            // Set the Db connection
            _context = db;

        }

        /// <summary>
        /// Save bulk options
        /// </summary>
        /// <param name="optionsList">Members options list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SaveOptionsAsync(List<MemberOptionsEntity> optionsList) {

            try {

                // Add range with options
                await _context.MembersOptions.AddRangeAsync(optionsList);

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