/*
 * @class Websocket Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-04
 *
 * This class is used to handle the websocket requests
 */

// Namespace for Public Controllers
namespace FeChat.Controllers.Public {

    // System Namespaces
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Encodings.Web;
    using System.Web;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Utils.General;
    using Utils.Interfaces;
    using Utils.Interfaces.Repositories.Members;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Websocket Controller
    /// </summary>
    public class WebSocketController: IWebSocketController {

        /// <summary>
        /// Websocket Status
        /// </summary>
        private bool webSocketActive = true;

        /// <summary>
        /// Receive the WebSocket request
        /// </summary>
        /// <param name="context">Information about HTTP request</param>
        /// <param name="membersRepository">Instance for members repository</param>
        /// <param name="messagesRepository">Instance for messages repository</param>
        /// <returns>Empty result</returns>
        public async Task QueueRequest(HttpContext context, IMembersRepository? membersRepository, IMessagesRepository messagesRepository) {

            // Accept an incoming WebSocket connection
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            // Prepare temporary data storage
            byte[] buffer = new byte[1024 * 4];

            // Parse the received data and save temporary
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Get the json stored as a string
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // Decode the json code to be possible to read the fields
            Dictionary<string, string>? fields = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            // Verify if fields exists
            if ( (fields != null) && (fields.Count > 0) ) {

                // Verify if website id exists
                if ( !fields.ContainsKey("WebsiteId") ) {

                    // Prepare the response
                    byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                        success = false,
                        message = new Strings().Get("WebsiteIdMissing")
                    }));

                    // Send the response
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Close the websocket connection
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);
                    
                }

                // Verify if thread secret exists
                if ( !fields.ContainsKey("ThreadSecret") ) {

                    // Prepare the response
                    byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                        success = false,
                        message = new Strings().Get("ThreadSecretMissing")
                    }));

                    // Send the response
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Close the websocket connection
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);
                    
                }       

                // Sanitize the website id
                int WebsiteId = int.Parse(HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(fields["WebsiteId"])));

                // Sanitize the Thread Secret
                string ThreadSecret = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(fields["ThreadSecret"]));                


                // Check if Thread Secret is empty
                if ( (ThreadSecret == null) || (ThreadSecret == "") ) {

                    // Prepare the response
                    byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                        success = false,
                        message = new Strings().Get("ThreadSecretMissing")
                    }));

                    // Send the response
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Close the websocket connection
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                }

                // Create the thread's data
                ThreadDto thread = new() {
                    WebsiteId = WebsiteId,
                    ThreadSecret = ThreadSecret
                };

                // Get the thread from the database
                ResponseDto<ThreadDto> getThread = await messagesRepository.GetThreadByWebsiteIdAsync(thread);

                // Verify if thread exists
                if ( getThread.Result == null ) {

                    // Prepare the response
                    byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                        success = false,
                        message = new Strings().Get("ThreadNotFound")
                    }));

                    // Send the response
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Close the websocket connection
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                } else {

                    // Start the background task
                    Task backgroundTask = Task.Run(async () => {

                        // Run until the websocket connection will be closed
                        while (webSocketActive == true) {

                            // Check for unseen messages
                            ResponseDto<bool> unseen = await messagesRepository.MessagesUnseenAsync(getThread.Result.ThreadId, 0);

                            // Verify if unseen messages exist
                            if (unseen.Result == true) {

                                // Prepare the response
                                byte[] unseenMessages = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                                    success = true,
                                    unseen = 1
                                }));

                                // Send the response
                                await webSocket.SendAsync(new ArraySegment<byte>(unseenMessages), WebSocketMessageType.Text, true, CancellationToken.None);

                            } else {

                                // Get the typing data
                                ResponseDto<TypingDto> typing = await messagesRepository.GetTypingAsync(getThread.Result.ThreadId, getThread.Result.MemberId);

                                // Verify if typing data exists
                                if ( typing.Result != null ) {
                                
                                    // Verify if updated typing has less than 3 seconds
                                    if ( ((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 3) <= typing.Result.Updated ) {

                                        // Prepare the response
                                        byte[] typingMessages = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                                            success = true,
                                            typing = 1
                                        }));

                                        // Send the response
                                        await webSocket.SendAsync(new ArraySegment<byte>(typingMessages), WebSocketMessageType.Text, true, CancellationToken.None);                                        

                                    }

                                }

                            }

                            // Pause for 3 seconds
                            await Task.Delay(3000);
                        }

                    });

                    try {

                        // Prepare temporary data storage
                        byte[] bufferMemory = new byte[1024];

                        // Run if the connection is open
                        while (webSocket.State == WebSocketState.Open) {

                            // Receives data from the WebSocket connection asynchronously.
                            var clientResponse = await webSocket.ReceiveAsync(new ArraySegment<byte>(bufferMemory), CancellationToken.None);

                            // Verify if a close message is received
                            if (clientResponse.MessageType == WebSocketMessageType.Close) {
                                
                                // Close websocket
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            }

                        }

                    } catch (Exception ex) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = ex.Message
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    } finally {

                        // Stop to check for unseen messages
                        webSocketActive = false;

                    } 

                }

            }

            // Close the websocket connection
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

        }

    }

}