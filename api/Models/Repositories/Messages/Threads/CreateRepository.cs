/*
 * @class Threads Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the threads
 */

// Namespace for Messages Threads Repositories
namespace FeChat.Models.Repositories.Messages.Threads {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Models.Entities.Messages;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Threads Create Repository
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
        /// Threads Create Repository Constructor
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
        /// Create Threads
        /// </summary>
        /// <param name="threadDto">Thread information</param>
        /// <returns>Response with thread secret and id or error message</returns>
        public async Task<ResponseDto<ThreadDto>> CreateThreadAsync(ThreadDto threadDto) {

            try {

                // Generate unique ID
                string ThreadSecret = Guid.NewGuid().ToString()[..7];

                // Create the entity
                ThreadEntity threadEntity = new() {
                    ThreadSecret = ThreadSecret,
                    MemberId = threadDto.MemberId,
                    WebsiteId = threadDto.WebsiteId,
                    GuestId = threadDto.GuestId,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add the entity to Threads
                _context.Threads.Add(threadEntity);

                // Save the changes
                int saveThread = await _context.SaveChangesAsync();

                // Verify if the entity was saved
                if ( saveThread > 0 ) {

                    // Remove the caches for threads group
                    new Cache(_memoryCache).Remove("threads"); 

                    return new ResponseDto<ThreadDto> {
                        Result = new ThreadDto {
                            ThreadId = threadEntity.ThreadId,
                            ThreadSecret = ThreadSecret
                        },
                        Message = null
                    };

                }
                
                return new ResponseDto<ThreadDto> {
                    Result = null,
                    Message = new Strings().Get("ThreadNotCreated")
                };

            } catch (Exception ex) {

                return new ResponseDto<ThreadDto> {
                    Result = null,
                    Message = ex.Message
                };                

            }

        }

    }
    
}