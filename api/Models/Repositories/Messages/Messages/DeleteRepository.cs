/*
 * @class Messages Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the messages
 */

// Namespace for Messages Repositories
namespace FeChat.Models.Repositories.Messages.Messages {

    // Use the Entity Framework
    using Microsoft.EntityFrameworkCore;

    // Using catching memory extension
    using Microsoft.Extensions.Caching.Memory;

    // Use general dtos
    using FeChat.Models.Dtos;

    // Use Messages Entities
    using FeChat.Models.Entities.Messages;

    // Use configuration
    using FeChat.Utils.Configuration;

    // Use General utils for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Messages Delete Repository
    /// </summary>
    public class DeleteRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Messages Delete Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public DeleteRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <returns>Bool true if the message was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteMessageAsync(int messageId) {

            try {

                // Select the message by id
                IQueryable<MessageEntity> message = _context.Messages.Where(m => m.MessageId == messageId);

                // Check if message exists
                if ( message == null ) {

                    // Return the error message
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
                    };

                }

                // Delete the message
                _context.Messages.RemoveRange(message);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {                       

                    // Gets attachments ids
                    List<AttachmentEntity> attachmentIds = await _context.Attachments.Where(a => a.MessageId == messageId).ToListAsync();

                    // Check if attachments ids exists
                    if ( attachmentIds.Count > 0 ) {

                        // Remove attachments from the list
                        _context.Attachments.RemoveRange(attachmentIds);

                        // Save changes
                        _context.SaveChanges();

                    }

                    // Remove the caches for messages group
                    new Cache(_memoryCache).Remove("messages"); 

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