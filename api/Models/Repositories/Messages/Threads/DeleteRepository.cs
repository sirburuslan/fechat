/*
 * @class Threads Delete Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to delete the threads
 */

// Namespace for Messages Threads Repositories
namespace FeChat.Models.Repositories.Messages.Threads {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Entities.Messages;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Threads Delete Repository
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
        /// Threads Delete Repository Constructor
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
        /// Delete thread
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>Bool true if the thread was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteThreadAsync(int threadId, int memberId) {

            try {

                // Select the thread by id
                IQueryable<ThreadEntity> thread = _context.Threads.Where(m => m.ThreadId == threadId && m.MemberId == memberId);

                // Check if thread exists
                if ( thread == null ) {

                    // Return the error message
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
                    };

                }

                // Get the guest id
                int? guestId = thread.FirstOrDefault()!.GuestId;

                // Delete the thread
                _context.Threads.RemoveRange(thread);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {   

                    // Check if guest exists
                    if ( guestId != null ) {

                        // Get guest by fuest id
                        GuestEntity? guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestId == guestId);

                        // Check if guest exists in the database
                        if ( guest != null ) {

                            // Remove guest from the list
                            _context.Guests.RemoveRange(guest);

                            // Save changes
                            _context.SaveChanges();
                            
                        }

                    }

                    // Get the typing
                    List<TypingEntity>? typingEntities = await _context.Typing.Where(t => t.ThreadId == threadId).ToListAsync();

                    // Verify if typing exists
                    if ( (typingEntities != null) && (typingEntities.Count > 0) ) {

                        // Remove the typing entities
                        _context.Typing.RemoveRange(typingEntities);

                        // Save the changes
                        await _context.SaveChangesAsync();

                    }

                    // Messages ids for deletion
                    List<MessageEntity>? messageEntities = await _context.Messages.Where(m => m.ThreadId == threadId).Select(m => new MessageEntity { MessageId = m.MessageId }).ToListAsync();

                    // Check if messages exists
                    if ( (messageEntities != null) && (messageEntities.Count > 0) ) {

                        // Gets attachments ids
                        List<AttachmentEntity>? attachmentIds = await _context.Attachments.Where(a => messageEntities.Select(entity => entity.MessageId).Contains(a.MessageId)).ToListAsync();

                        // Check if attachments ids exists
                        if ( (attachmentIds != null) && (attachmentIds.Count > 0) ) {

                            // Remove attachments from the list
                            _context.Attachments.RemoveRange(attachmentIds);

                            // Save changes
                            _context.SaveChanges();

                        }

                        // Remove messages from the list
                        _context.Messages.RemoveRange(messageEntities);

                        // Save changes
                        _context.SaveChanges();
                        
                    }

                    // Create the cache key
                    string cacheKey = "fc_thread_" + threadId;

                    // Delete the cache
                    _memoryCache.Remove(cacheKey);

                    // Remove the caches for threads group
                    new Cache(_memoryCache).Remove("threads"); 

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