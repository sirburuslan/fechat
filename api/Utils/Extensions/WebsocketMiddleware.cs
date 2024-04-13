/*
 * @class Websocket Middleware
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-04-05
 *
 * This scope of this class is to create a middleware which will process the websockets requests
 */

// Namespace for Extensions
namespace FeChat.Utils.Extensions {

    // System Namespaces
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;    

    // App Namespaces
    using Controllers.Public;
    using Controllers.User;
    using Utils.Interfaces;
    using Utils.Interfaces.Repositories.Members;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Websocket Middleware
    /// </summary>
    public class WebSocketMiddleware : IMiddleware
    {

        /// <summary>
        /// Members Repository Container
        /// </summary>
        private readonly IMembersRepository _membersRepository;

        /// <summary>
        /// Messages Repository Container
        /// </summary>
        private readonly IMessagesRepository _messagesRepository;

        /// <summary>
        /// WebSocket Constructor
        /// </summary>
        /// <param name="membersRepository">Members Repository</param>
        /// <param name="messagesRepository">Messages Repository</param>
        public WebSocketMiddleware(IMembersRepository membersRepository, IMessagesRepository messagesRepository)
        {

            // Set members repository to the container
            _membersRepository = membersRepository;

            // Set messages repository to the container
            _messagesRepository = messagesRepository;
            
        }

        /// <summary>
        /// Entry Point for Middleware
        /// </summary>
        /// <param name="context">Http Request Context</param>
        /// <param name="next">A delegate for the next middleware</param>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            // Verify if the http request contains a websocket request
            if (context.WebSockets.IsWebSocketRequest)
            {
                // Use routing to determine the appropriate controller
                WebSocketRoute? route = GetWebSocketRoute(context.Request.Path);

                // Verify if a websocket route exists
                if ((route != null) && (route.Controller != null))
                {

                    // Pass the websocket request to the right controller
                    await route.Controller.QueueRequest(context, _membersRepository, _messagesRepository);
                    return;

                }
                
            }

            // If not a WebSocket request or route not found, continue to next middleware
            await next(context);

        }

        private static WebSocketRoute? GetWebSocketRoute(PathString path)
        {
            // Define your WebSocket routes here
            Dictionary<PathString, WebSocketRoute> routes = new () {
                { new PathString("/api/v1/user/websocket"), new WebSocketRoute { Controller = new UserWebSocketController() } },
                { new PathString("/api/v1/websocket"), new WebSocketRoute { Controller = new WebSocketController() } },
            };

            // Verify if the path exists in the created routes list
            if (routes.TryGetValue(path, out var route))
            {
                return route;
            }

            return null;
        }

        /// <summary>
        /// This class is used as container for WebSocket route information
        /// </summary>
        private class WebSocketRoute
        {

            /// <summary>
            /// This is a reference for the WebSocket Controller
            /// </summary>
            public IWebSocketController? Controller { get; set; }

        }
        
    }
}