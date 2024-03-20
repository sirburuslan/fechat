/*
 * @class Guests Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the guests
 */

// Namespace for Messages Guests Repositories
namespace FeChat.Models.Repositories.Messages.Guests {

    // Use the catching memory
    using Microsoft.Extensions.Caching.Memory;

    // Use the general Dtos for Response
    using FeChat.Models.Dtos;

    // Use the Dtos for messages
    using FeChat.Models.Dtos.Messages;

    // Use Messages entities
    using FeChat.Models.Entities.Messages;

    // Use the Configuration utils for Db connector
    using FeChat.Utils.Configuration;

    // Use General Utils
    using FeChat.Utils.General;

    /// <summary>
    /// Guests Create Repository
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Guests Create Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public CreateRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Create Guests
        /// </summary>
        /// <param name="guestDto">Guest information</param>
        /// <returns>Response with guest id or error message</returns>
        public async Task<ResponseDto<GuestDto>> CreateGuestAsync(GuestDto guestDto) {

            try {

                // Create the entity
                GuestEntity guestEntity = new() {
                    Name = guestDto.Name,
                    Email = guestDto.Email,
                    Ip = guestDto.Ip,
                    Latitude = guestDto.Latitude,
                    Longitude = guestDto.Longitude,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add the entity to Guests
                _context.Guests.Add(guestEntity);

                // Save the changes
                int saveGuest = await _context.SaveChangesAsync();

                // Verify if the entity was saved
                if ( saveGuest > 0 ) {

                    return new ResponseDto<GuestDto> {
                        Result = new GuestDto {
                            GuestId = guestEntity.GuestId
                        },
                        Message = null
                    };

                }
                
                return new ResponseDto<GuestDto> {
                    Result = null,
                    Message = new Strings().Get("GuestNotCreated")
                };

            } catch (Exception ex) {

                return new ResponseDto<GuestDto> {
                    Result = null,
                    Message = ex.Message
                };                

            }

        }

    }
    
}