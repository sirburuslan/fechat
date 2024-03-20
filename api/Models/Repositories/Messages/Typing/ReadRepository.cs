/*
 * @class Typing Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the typing
 */

// Namespace for Messages Typing Repositories
namespace FeChat.Models.Repositories.Messages.Typing {

    // Use the Entity Framework
    using Microsoft.EntityFrameworkCore;

    // Use the catching memory
    using Microsoft.Extensions.Caching.Memory;

    // Use the general Dtos for Response
    using FeChat.Models.Dtos;

    // Use the Dtos for messages
    using FeChat.Models.Dtos.Messages;

    // Use the Configuration utils for Db connector
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Typing Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Typing Read Repository Constructor
        /// </summary>
        /// <param name="db">Db connection instance</param>
        public ReadRepository(Db db) {

            // Save the session
            _context = db;

        }
        

        /// <summary>
        /// Get typing data
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>typing data or error message</returns>
        public async Task<ResponseDto<TypingDto>> GetTypingAsync(int threadId, int memberId) {

            try {

                // Get the typing
                TypingDto? typingEntity = await _context.Typing.Select(t => new TypingDto {
                    Id = t.Id,
                    ThreadId = t.ThreadId,
                    MemberId = t.MemberId,
                    Updated = t.Updated
                })
                .Where(t => t.ThreadId == threadId && t.MemberId == memberId)
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync(); 

                // Verify if typing exists
                if ( typingEntity != null ) {

                    // Return the thread data
                    return new ResponseDto<TypingDto> {
                        Result = typingEntity,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<TypingDto> {
                        Result = null,
                        Message = null
                    };
                    
                }

            } catch ( Exception e ) {

                // Return the error message
                return new ResponseDto<TypingDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }
    
}