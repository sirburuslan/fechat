/*
 * @class Member Options Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used to read the member's options
 */

// Namespace for Members Repositories model
namespace FeChat.Models.Repositories.Members.Options {
    
    // Use the Entity Framework for requests
    using Microsoft.EntityFrameworkCore;
    
    // Use Cache to store the quries
    using Microsoft.Extensions.Caching.Memory;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use Dtos for members
    using FeChat.Models.Dtos.Members;
    
    // Use the configuration for Db instance
    using FeChat.Utils.Configuration;

    // Use General namespace for the strings
    using FeChat.Utils.General;

    /// <summary>
    /// Members Options Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Db connection container
        /// </summary>
        private readonly Db _context;

        /// <summary>
        /// Members Options Read constructor
        /// </summary>
        /// <param name="memoryCache">Memory cache instane</param>
        /// <param name="db">Db connection</param>
        public ReadRepository(IMemoryCache memoryCache, Db db) {

            // Set memory cache
            _memoryCache = memoryCache;

            // Set the Db connection
            _context = db;

        }

        /// <summary>
        /// Get the options list
        /// </summary>
        /// <param name="MemberId">Member's ID</param>
        /// <returns>Get the options list</returns>
        public async Task<ResponseDto<List<OptionDto>>> OptionsListAsync(int MemberId) {

            try {

                // Create the cache key
                string cacheKey = "fc_member_options_" + MemberId;

                // Verify if the options are saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out List<OptionDto>? optionsEntities) ) {

                    // Get all options
                    optionsEntities = await _context.MembersOptions
                    .Select(s => new OptionDto {
                        OptionId = s.OptionId,
                        MemberId = s.MemberId,
                        OptionName = s.OptionName,
                        OptionValue = s.OptionValue ?? string.Empty
                    })
                    .Where(o => o.MemberId == MemberId )
                    .ToListAsync();

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, optionsEntities, cacheOptions);

                }

                // Verify if options exists
                if ( (optionsEntities != null) && (optionsEntities.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<List<OptionDto>> {
                        Result = optionsEntities,
                        Message = null
                    };                    

                } else {

                    // Return the response
                    return new ResponseDto<List<OptionDto>> {
                        Result = null,
                        Message = new Strings().Get("NoOptionsFound")
                    };

                }

            } catch (InvalidOperationException ex) {

                // Return the response
                return new ResponseDto<List<OptionDto>> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

        /// <summary>
        /// Get Member Option with Google ID
        /// </summary>
        /// <param name="googleId">Google ID</param>
        /// <returns>Option of the user or error message</returns>
        public async Task<ResponseDto<OptionDto>> GetMemberOptionWithGoogle(string googleId) {

            try {

                // Get option
                OptionDto? optionDto = await _context.MembersOptions
                .Select(s => new OptionDto {
                    OptionId = s.OptionId,
                    MemberId = s.MemberId,
                    OptionName = s.OptionName,
                    OptionValue = s.OptionValue ?? string.Empty
                })
                .FirstAsync(o => o.OptionName == "GoogleId" && o.OptionValue == googleId);

                // Verify if option exists
                if ( optionDto != null ) {

                    // Return the response
                    return new ResponseDto<OptionDto> {
                        Result = optionDto,
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<OptionDto> {
                        Result = null,
                        Message = new Strings().Get("AccountNotFound")
                    };
                    
                }

            } catch ( Exception ex ) {

                // Return the response
                return new ResponseDto<OptionDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }

}