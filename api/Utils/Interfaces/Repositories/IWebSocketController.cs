/*
 * @interface WebSocket Request
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-04-06
 *
 * This interface is used to create the websocket classes
 */

// Namespace for Interfaces
namespace FeChat.Utils.Interfaces {

    // App Namespaces
    using Utils.Interfaces.Repositories.Members;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Interface for Websocket Controller
    /// </summary>
    public interface IWebSocketController {

        /// <summary>
        /// Receive the WebSocket request
        /// </summary>
        /// <param name="context">Information about HTTP request</param>
        /// <param name="membersRepository">Instance for members repository</param>
        /// <param name="messagesRepository">Instance for messages repository</param>
        /// <returns>Empty result</returns>
        Task QueueRequest(HttpContext context, IMembersRepository membersRepository, IMessagesRepository messagesRepository);

    }

}