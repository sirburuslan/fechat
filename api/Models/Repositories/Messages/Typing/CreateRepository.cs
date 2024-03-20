/*
 * @class Typing Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the typing
 */

// Namespace for Messages Typing Repositories
namespace FeChat.Models.Repositories.Messages.Typing {

    // Use the catching memory
    using Microsoft.Extensions.Caching.Memory;

    // Use Messages entities
    using FeChat.Models.Entities.Messages;

    // Use the Configuration utils for Db connector
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Typing Create Repository
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
        /// Typing Create Repository Constructor
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
        /// Save typing
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>Id or error message</returns>
        public async Task SaveTypingAsync(int threadId, int memberId) {

            try {

                // Create the entity
                TypingEntity typingEntity = new() {
                    ThreadId = threadId,
                    MemberId = memberId,
                    Updated = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add entity to the list
                _context.Typing.Add(typingEntity);

                // Save changes
                await _context.SaveChangesAsync();

            } catch ( Exception ex ) {

                Console.WriteLine(ex.Message);

            }

        }

    }
    
}