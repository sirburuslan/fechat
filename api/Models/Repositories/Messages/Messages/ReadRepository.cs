/*
 * @class Messages Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to read the messages
 */

// Namespace for Messages Repositories
namespace FeChat.Models.Repositories.Messages.Messages {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Models.Entities.Messages;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Messages Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Messages Read Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public ReadRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Get the messages list
        /// </summary>
        /// <param name="messagesListDto">Parameters for messages list</param>
        /// <returns>List with messages or error message</returns>
        public async Task<ResponseDto<ElementsDto<MessageDto>>> MessagesListAsync(MessagesListDto messagesListDto) {

            try {

                // Prepare the page
                int page = (messagesListDto.Page > 0)?messagesListDto.Page:1;

                // Prepare the total results
                int total = 10;

                // Create the cache key
                string cacheKey = "fc_messages_" + messagesListDto.ThreadId + "_" + page;

                // Verify if cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out Tuple<List<MessageDto>, int>? messagesResponse) ) {

                    // Get the messages
                    List<MessageDto> messages = await _context.Messages
                    .GroupJoin(
                        _context.Attachments,
                        message => message.MessageId,
                        a => a.MessageId,
                        (message, attachments) => new {
                            message,
                            Attachments = attachments.ToList()
                        }
                    )
                    .SelectMany(
                        a => a.Attachments.DefaultIfEmpty(),
                        (m, a) => new MessageDto {
                            MessageId = m.message.MessageId,
                            ThreadId = m.message.ThreadId,
                            MemberId = m.message.MemberId,
                            Message = m.message.Message,
                            Seen = m.message.Seen,
                            Created = m.message.Created,
                            Attachments = m.Attachments.Select(att => att.Link).ToArray()!
                        }
                    )
                    .Where(m => m.ThreadId == messagesListDto.ThreadId)
                    .OrderByDescending(m => m.MessageId)
                    .Skip((page - 1) * total)
                    .Take(total)
                    .ToListAsync();

                    // Get total number of messages
                    int totalCount = await _context.Messages.Where(m => m.ThreadId == messagesListDto.ThreadId).CountAsync();   

                    // Add data to messages list response
                    messagesResponse = new Tuple<List<MessageDto>, int>(messages, totalCount);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, messagesResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("messages", cacheKey);                 

                }

                // Verify if messages list exists
                if ( (messagesResponse != null) && (messagesResponse.Item1.Count > 0) ) {

                    // Get unseen messages in the response
                    List<MessageEntity> unseenMessages = await _context.Messages.Where(m => m.ThreadId == messagesListDto.ThreadId && m.MemberId != messagesListDto.MemberId && m.Seen == 0).ToListAsync();

                    // Verify if unseen messages exists
                    if ( unseenMessages.Count > 0 ) {

                        // List unseen messages
                        for ( int s = 0; s < unseenMessages.Count; s++ ) {

                            // Mark as seen
                            unseenMessages[s].Seen = 1;

                        }

                        // Update range
                        _context.UpdateRange(unseenMessages);

                        // Save changes
                        await _context.SaveChangesAsync();

                        // Remove the caches for messages group
                        new Cache(_memoryCache).Remove("messages"); 

                    }

                    return new ResponseDto<ElementsDto<MessageDto>> {
                        Result = new ElementsDto<MessageDto> {
                            Elements = messagesResponse.Item1,
                            Total = messagesResponse.Item2,
                            Page = messagesListDto.Page
                        },
                        Message = null
                    };

                } else {

                    return new ResponseDto<ElementsDto<MessageDto>> {
                        Result = null,
                        Message = new Strings().Get("NoMessagesFound")
                    };

                }

            } catch(Exception ex) {

                return new ResponseDto<ElementsDto<MessageDto>> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

        /// <summary>
        /// Get unseen messages by thread
        /// </summary>
        /// <param name="threadId">Thread ID where will be looked for unseen messages</param>
        /// <param name="memberId">Member ID which has unseen messages</param>
        /// <returns>Bool and error message if exists</returns>
        public async Task<ResponseDto<bool>> MessagesUnseenAsync(int threadId, int memberId = 0) {

            try {

                // Create the cache key
                string cacheKey = "fc_messages_unseen_" + threadId + "_" + memberId;

                // Verify if cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out List<MessageDto>? unseenMessages ) ) {

                    // Check if Member Id is not 0
                    if ( memberId > 0 ) {

                        // Get an unseen message
                        unseenMessages = await _context.Messages
                        .Select(m => new MessageDto {
                            ThreadId = m.ThreadId,
                            MemberId = m.MemberId,
                            Seen = m.Seen
                        })
                        .Join(
                            _context.Threads,
                            m => m.ThreadId,
                            t => t.ThreadId,
                            (m, t) => new MessageDto {
                                ThreadId = m.ThreadId,
                                MemberId = t.MemberId,
                                Seen = m.Seen
                            }
                        )
                        .Where(m => m.MemberId == memberId && m.Seen == 0)
                        .ToListAsync();

                    } else {

                        // Get an unseen message
                        unseenMessages = await _context.Messages
                        .Select(m => new MessageDto {
                            ThreadId = m.ThreadId,
                            MemberId = m.MemberId,
                            Seen = m.Seen
                        })
                        .Where(m => m.ThreadId == threadId && m.MemberId != memberId && m.Seen == 0)
                        .ToListAsync();

                    }

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, unseenMessages, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("messages", cacheKey);  

                }

                // Verify if unseen messages exists
                if ( (unseenMessages != null) && (unseenMessages.Count > 0) ) {

                    return new ResponseDto<bool> {
                        Result = true,
                        Message = null
                    };

                } else {

                    return new ResponseDto<bool> {
                        Result = false,
                        Message = new Strings().Get("NoMessagesFound")
                    };

                }

            } catch(Exception ex) {

                return new ResponseDto<bool> {
                    Result = false,
                    Message = ex.Message
                };

            }

        }

        /// <summary>
        /// Get unseen messages by thread
        /// </summary>
        /// <returns>Unseen messages or error message</returns>
        public async Task<ResponseDto<List<UnseenMessageDto>>> AllMessagesUnseenAsync() {

            try {

                // Calculate the start time
                int startTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 300;

                // Calculate the end time
                int endTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 360;

                // Get an unseen message
                List<UnseenMessageDto>? unseenMessages = await _context.Messages
                    .Join(
                        _context.Threads,
                        m => m.ThreadId,
                        t => t.ThreadId,
                        (m, t) => new UnseenMessageDto {
                            MessageId = m.MessageId,
                            MemberId = m.MemberId,
                            Seen = m.Seen,
                            Created = m.Created,
                            ThreadOwner = t.MemberId                                                
                        }
                    )
                    .Join(
                        _context.Members,
                        m => m.ThreadOwner,
                        u => u.MemberId,
                        (m, u) => new UnseenMessageDto {
                            MessageId = m.MessageId,
                            MemberId = m.MemberId,
                            Seen = m.Seen,
                            Created = m.Created,
                            ThreadOwner = m.ThreadOwner,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email
                        }
                    )
                    .Where(m => startTime >= m.Created && endTime <= m.Created && m.MemberId == 0 && m.Seen == 0)
                    .ToListAsync();

                // Verify if unseen messages exists
                if ( (unseenMessages != null) && (unseenMessages.Count > 0) ) {

                    return new ResponseDto<List<UnseenMessageDto>> {
                        Result = unseenMessages,
                        Message = null
                    };

                } else {

                    return new ResponseDto<List<UnseenMessageDto>> {
                        Result = null,
                        Message = new Strings().Get("NoMessagesFound")
                    };

                }

            } catch(Exception ex) {

                return new ResponseDto<List<UnseenMessageDto>> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }
    
}