/*
 * @class Messages Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to create the messages
 */

// Namespace for Messages Repositories
namespace FeChat.Models.Repositories.Messages.Messages {

    // Using catching memory extension
    using Microsoft.Extensions.Caching.Memory;

    // Use general dtos
    using FeChat.Models.Dtos;

    // Use dtos for messages
    using FeChat.Models.Dtos.Messages;

    // Use Messages Entities
    using FeChat.Models.Entities.Messages;

    // Use configuration
    using FeChat.Utils.Configuration;

    // Use General utils for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Messages Create Repository
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
        /// Messages Create Repository Constructor
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
        /// Create a message
        /// </summary>
        /// <param name="messageDto">Contains message data</param>
        /// <returns>Response with message id and message text</returns>
        public async Task<ResponseDto<MessageDto>> CreateMessageAsync(MessageDto messageDto) {

            try {

                // Create the message entity
                MessageEntity messageEntity = new() {
                    ThreadId = messageDto.ThreadId,
                    MemberId = messageDto.MemberId,
                    Message = messageDto.Message,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add entity
                _context.Messages.Add(messageEntity);

                // Save changes
                int saveMessage = await _context.SaveChangesAsync();

                // Verify if the message was saved
                if ( saveMessage > 0 ) {

                    // Remove the caches for messages group
                    new Cache(_memoryCache).Remove("messages"); 

                    // Remove the caches for threads group
                    new Cache(_memoryCache).Remove("threads"); 

                    // Create an return success message
                    return new ResponseDto<MessageDto> {
                        Result = new MessageDto {
                            MessageId = messageEntity.MessageId
                        },
                        Message = new Strings().Get("MessageCreated")
                    };

                } else {

                    return new ResponseDto<MessageDto> {
                        Result = null,
                        Message = new Strings().Get("MessageNotCreated")
                    };  

                }

            } catch (Exception ex) {

                return new ResponseDto<MessageDto> {
                    Result = null,
                    Message = ex.Message
                };  

            }

        }

    }
    
}