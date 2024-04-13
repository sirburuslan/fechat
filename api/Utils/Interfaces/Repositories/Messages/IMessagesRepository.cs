/*
 * @interface Messages Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-27
 *
 * This interface is implemented in MessagesRepository
 */

// Namespace for Messages namespace
namespace FeChat.Utils.Interfaces.Repositories.Messages {

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;

    /// <summary>
    /// Interface for Messages Repository
    /// </summary>
    public interface IMessagesRepository {

        /// <summary>
        /// Create a message
        /// </summary>
        /// <param name="messageDto">Contains message data</param>
        /// <returns>Response with message id and message text</returns>
        Task<ResponseDto<MessageDto>> CreateMessageAsync(MessageDto messageDto);

       /// <summary>
        /// Get the messages list
        /// </summary>
        /// <param name="messagesListDto">Parameters for messages list</param>
        /// <returns>List with messages or error message</returns>
        Task<ResponseDto<ElementsDto<MessageDto>>> MessagesListAsync(MessagesListDto messagesListDto);

        /// <summary>
        /// Get unseen messages by thread
        /// </summary>
        /// <param name="ThreadId">Thread ID where will be looked for unseen messages</param>
        /// <param name="MemberId">Member ID which has unseen messages</param>
        /// <returns>Bool and error message if exists</returns>
        Task<ResponseDto<bool>> MessagesUnseenAsync(int ThreadId, int MemberId = 0);

        /// <summary>
        /// Get unseen messages by thread
        /// </summary>
        /// <returns>Unseen messages or error message</returns>
        Task<ResponseDto<List<UnseenMessageDto>>> AllMessagesUnseenAsync();

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <returns>Bool true if the message was deleted</returns>
        Task<ResponseDto<bool>> DeleteMessageAsync(int messageId);

        /// <summary>
        /// Create Guests
        /// </summary>
        /// <param name="guestDto">Guest information</param>
        /// <returns>Response with guest id or error message</returns>
        Task<ResponseDto<GuestDto>> CreateGuestAsync(GuestDto guestDto);

        /// <summary>
        /// Create Threads
        /// </summary>
        /// <param name="threadDto">Thread information</param>
        /// <returns>Response with thread secret and id or error message</returns>
        Task<ResponseDto<ThreadDto>> CreateThreadAsync(ThreadDto threadDto);

        /// <summary>
        /// Gets all threads
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="memberId">Member Id</param>
        /// <param name="websiteId">Website ID</param>
        /// <returns>List with threads</returns>
        Task<ResponseDto<ElementsDto<ResponseThreadDto>>> GetThreadsAsync(SearchDto searchDto, int memberId, int? websiteId);

        /// <summary>
        /// Gets last 5 updated threads
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>List with threads</returns>
        Task<ResponseDto<ElementsDto<ResponseThreadDto>>> GetHotThreadsAsync(int memberId);

        /// <summary>
        /// Gets all threads by time
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="time">Time filter</param>
        /// <returns>List with threads</returns>
        Task<ResponseDto<object>> GetThreadsByTimeAsync(int memberId, int time);

        /// <summary>
        /// Get thread by website's id and thread secret
        /// </summary>
        /// <param name="threadDto">Thread information</param>
        /// <returns>Response with thread or error message</returns>
        Task<ResponseDto<ThreadDto>> GetThreadByWebsiteIdAsync(ThreadDto threadDto);

        /// <summary>
        /// Get thread data
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>ThreadDto with thread's data</returns>
        Task<ResponseDto<ThreadDto>> GetThreadAsync(int threadId, int memberId);

        /// <summary>
        /// Delete thread
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>Bool true if the thread was deleted</returns>
        Task<ResponseDto<bool>> DeleteThreadAsync(int threadId, int memberId);

        /// <summary>
        /// Save typing
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        Task SaveTypingAsync(int threadId, int memberId);

        /// <summary>
        /// Update Typing ID
        /// </summary>
        /// <param name="typingId">Typing Id</param>
        Task UpdateTypingAsync(int typingId);

        /// <summary>
        /// Get typing data
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>typing data or error message</returns>
        Task<ResponseDto<TypingDto>> GetTypingAsync(int threadId, int memberId);

        /// <summary>
        /// Create attachment
        /// </summary>
        /// <param name="attachmentDto">Attachment data</param>
        /// <returns>Saved attachment data or message</returns>
        Task<ResponseDto<AttachmentDto>> CreateAttachment(AttachmentDto attachmentDto);

    }

}