/*
 * @class Messages Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to manage the messages
 */

// Namespace for Messages Repositories
namespace FeChat.Models.Repositories.Messages {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Utils.Configuration;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Messages Repository
    /// </summary>
    public class MessagesRepository: IMessagesRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public MessagesRepository(IMemoryCache memoryCache, Db db) {

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

            // Init Create Repository
            Messages.CreateRepository createRepository = new(_memoryCache, _context);

            // Create a message and return the response
            return await createRepository.CreateMessageAsync(messageDto);

        }

        /// <summary>
        /// Get the messages list
        /// </summary>
        /// <param name="messagesListDto">Parameters for messages list</param>
        /// <returns>List with messages or error message</returns>
        public async Task<ResponseDto<ElementsDto<MessageDto>>> MessagesListAsync(MessagesListDto messagesListDto) {

             // Init Read Repository
            Messages.ReadRepository readRepository = new(_memoryCache, _context);

            // Gets the messages list and return the response
            return await readRepository.MessagesListAsync(messagesListDto);

        }

        /// <summary>
        /// Get unseen messages by thread
        /// </summary>
        /// <param name="threadId">Thread ID where will be looked for unseen messages</param>
        /// <param name="memberId">Member ID which has unseen messages</param>
        /// <returns>Bool and error message if exists</returns>
        public async Task<ResponseDto<bool>> MessagesUnseenAsync(int threadId, int memberId = 0) {

            // Init Read Repository
            Messages.ReadRepository readRepository = new(_memoryCache, _context);

            // Read a message and return the response
            return await readRepository.MessagesUnseenAsync(threadId, memberId);

        }

        /// <summary>
        /// Get unseen messages by thread
        /// </summary>
        /// <returns>Unseen messages or error message</returns>
        public async Task<ResponseDto<List<UnseenMessageDto>>> AllMessagesUnseenAsync() {

            // Init Read Repository
            Messages.ReadRepository readRepository = new(_memoryCache, _context);

            // Read all unseen messages and return the response
            return await readRepository.AllMessagesUnseenAsync();

        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <returns>Bool true if the message was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteMessageAsync(int messageId) {

            // Init Delete Repository
            Messages.DeleteRepository deleteRepository = new(_memoryCache, _context);

            // Delete a message and return the response
            return await deleteRepository.DeleteMessageAsync(messageId);

        }

        /// <summary>
        /// Create Guests
        /// </summary>
        /// <param name="guestDto">Guest information</param>
        /// <returns>Response with guest id or error message</returns>
        public async Task<ResponseDto<GuestDto>> CreateGuestAsync(GuestDto guestDto) {

            // Init Create Repository
            Guests.CreateRepository createRepository = new(_context);

            // Create a guest and return the response
            return await createRepository.CreateGuestAsync(guestDto);

        }

        /// <summary>
        /// Create Threads
        /// </summary>
        /// <param name="threadDto">Thread information</param>
        /// <returns>Response with thread secret and id or error message</returns>
        public async Task<ResponseDto<ThreadDto>> CreateThreadAsync(ThreadDto threadDto) {

            // Init Create Repository
            Threads.CreateRepository createRepository = new(_memoryCache, _context);

            // Create a guest and return the response
            return await createRepository.CreateThreadAsync(threadDto);

        }

        /// <summary>
        /// Gets all threads
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="memberId">Member Id</param>
        /// <param name="websiteId">Website ID</param>
        /// <returns>List with threads</returns>
        public async Task<ResponseDto<ElementsDto<ResponseThreadDto>>> GetThreadsAsync(SearchDto searchDto, int memberId, int? websiteId) {

            // Init Read Repository
            Threads.ReadRepository readRepository = new(_memoryCache, _context);

            // Get the threads and return the response
            return await readRepository.GetThreadsAsync(searchDto, memberId, websiteId);

        }

        /// <summary>
        /// Gets last 5 updated threads
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>List with threads</returns>
        public async Task<ResponseDto<ElementsDto<ResponseThreadDto>>> GetHotThreadsAsync(int memberId) {

            // Init Read Repository
            Threads.ReadRepository readRepository = new(_memoryCache, _context);

            // Get the last updated threads and return the response
            return await readRepository.GetHotThreadsAsync(memberId);


        }

        /// <summary>
        /// Gets all threads by time
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="time">Time filter</param>
        /// <returns>List with threads</returns>
        public async Task<ResponseDto<object>> GetThreadsByTimeAsync(int memberId, int time) {

            // Init Read Repository
            Threads.ReadRepository readRepository = new(_memoryCache, _context);

            // Get the threads by time and return the response
            return await readRepository.GetThreadsByTimeAsync(memberId, time);

        }

        /// <summary>
        /// Get thread by website's id and thread secret
        /// </summary>
        /// <param name="threadDto">Thread information</param>
        /// <returns>Response with thread or error message</returns>
        public async Task<ResponseDto<ThreadDto>> GetThreadByWebsiteIdAsync(ThreadDto threadDto) {

            // Init Read Repository
            Threads.ReadRepository readRepository = new(_memoryCache, _context);

            // Get the thread by website id and return the response
            return await readRepository.GetThreadByWebsiteIdAsync(threadDto);

        }

        /// <summary>
        /// Get thread data
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>ThreadDto with thread's data</returns>
        public async Task<ResponseDto<ThreadDto>> GetThreadAsync(int threadId, int memberId) {

            // Init Read Repository
            Threads.ReadRepository readRepository = new(_memoryCache, _context);

            // Get the thread by id and return the response
            return await readRepository.GetThreadAsync(threadId, memberId);

        }

        /// <summary>
        /// Delete thread
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>Bool true if the thread was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteThreadAsync(int threadId, int memberId) {

            // Init Delete Repository
            Threads.DeleteRepository deleteRepository = new(_memoryCache, _context);

            // Delete the thread and return the response
            return await deleteRepository.DeleteThreadAsync(threadId, memberId);

        }

        /// <summary>
        /// Save typing
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        public async Task SaveTypingAsync(int threadId, int memberId) {

            // Init Create Repository
            Typing.CreateRepository createRepository = new(_context);

            // Create typing
            await createRepository.SaveTypingAsync(threadId, memberId);

        }

        /// <summary>
        /// Update Typing ID
        /// </summary>
        /// <param name="typingId">Typing Id</param>
        public async Task UpdateTypingAsync(int typingId) {

            // Init Update Repository
            Typing.UpdateRepository updateRepository = new(_context);

            // Update typing time
            await updateRepository.UpdateTypingAsync(typingId);

        }       

        /// <summary>
        /// Get typing data
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>typing data or error message</returns>
        public async Task<ResponseDto<TypingDto>> GetTypingAsync(int threadId, int memberId) {

            // Init Read Repository
            Typing.ReadRepository readRepository = new(_context);

            // Read typing time
            return await readRepository.GetTypingAsync(threadId, memberId);

        }  

        /// <summary>
        /// Create attachment
        /// </summary>
        /// <param name="attachmentDto">Attachment data</param>
        /// <returns>Saved attachment data or message</returns>
        public async Task<ResponseDto<AttachmentDto>> CreateAttachment(AttachmentDto attachmentDto) {

            // Init Create Repository
            Attachments.CreateRepository createRepository = new(_context);

            // Create a message and return the response
            return await createRepository.CreateAttachmentAsync(attachmentDto);

        }

    }
    
}