/*
 * @class Settings Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-28
 *
 * This class is used manage the general settings
 */

// Namespace for Settings repositories
namespace FeChat.Models.Repositories.Settings {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Settings;
    using Models.Entities.Settings;
    using Utils.Configuration;
    using Utils.General;
    using Utils.Interfaces.Repositories.Settings;

    /// <summary>
    /// Repository for general settings
    /// </summary>
    public class SettingsRepository: ISettingsRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Settings table context container
        /// </summary>
        private readonly Db _context;

        /// <summary>
        /// Settings Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Memory cache session</param>
        /// <param name="db">Database connection</param>
        public SettingsRepository(IMemoryCache memoryCache, Db db) {

            // Set memory cache
            _memoryCache = memoryCache;
            
            // Add db to the global context
            _context = db;

        }

        /// <summary>
        /// Save bulk options
        /// </summary>
        /// <param name="options">Members options list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SaveOptionsAsync(List<SettingsEntity> options) {

            try {

                // Add range with options
                await _context.Settings.AddRangeAsync(options);

                // Save the options
                int save = _context.SaveChanges();

                // Create the cache key
                string cacheKey = "fc_member_settings";

                // Delete the cache
                _memoryCache.Remove(cacheKey);

                return save > 0;

            } catch (InvalidOperationException) {

                return false;

            }

        }

        /// <summary>
        /// Update bulk options
        /// </summary>
        /// <param name="options">Settings options list</param>
        /// <returns>Boolean response</returns>
        public bool UpdateOptionsAsync(List<SettingsEntity> options) {

            try {

                // Update the entities in the database
                _context.Settings.UpdateRange(options); // Right now UpdateRangeAsync is not available

                // Save the options
                int save = _context.SaveChanges();

                // Create the cache key
                string cacheKey = "fc_member_settings";

                // Delete the cache
                _memoryCache.Remove(cacheKey);

                return save > 0;

            } catch (InvalidOperationException) {

                return false;

            }

        }

        /// <summary>
        /// Get the settings options
        /// </summary>
        /// <returns>The options list or null</returns>
        public async Task<ResponseDto<List<OptionDto>>> OptionsListAsync() {

            try {

                // Create the cache key for settings
                string cacheKey = "fc_member_settings";

                // Verify if the options are saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out List<OptionDto>? optionsList) ) {

                    // Request the options
                    optionsList = await _context.Settings
                    .Select(o => new OptionDto {
                        OptionId = o.OptionId,
                        OptionName = o.OptionName,
                        OptionValue = o.OptionValue
                    })
                    .ToListAsync();

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, optionsList, cacheOptions);

                }

                // Verify if options exists
                if ( (optionsList != null) && (optionsList.Count > 0) ) {

                    // Return the options
                    return new ResponseDto<List<OptionDto>> {
                        Result = optionsList,
                        Message = null
                    };

                } else {

                    // Return the missing options message
                    return new ResponseDto<List<OptionDto>> {
                        Result = null,
                        Message = new Strings().Get("NoOptionsFound")
                    };                    

                }

            } catch ( InvalidOperationException e ) {

                // Return the error message
                return new ResponseDto<List<OptionDto>> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }

}