/*
 * @class User Websocket Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to handle the websocket requests
 */

// Namespace for Users Controllers
namespace FeChat.Controllers.User {

    // System Namespaces
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Web;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Models.Dtos.Messages;
    using Utils.General;
    using Utils.Interfaces;
    using Utils.Interfaces.Repositories.Members;
    using Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Websocket Controller for User
    /// </summary>
    public class UserWebSocketController: IWebSocketController {

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
        public async Task QueueRequest(HttpContext context, IMembersRepository membersRepository, IMessagesRepository messagesRepository) {

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
                
                // Verify if access token exists
                if ( !fields.ContainsKey("AccessToken") ) {

                    // Prepare the response
                    byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                        success = false,
                        message = new Strings().Get("NoTokenFound")
                    }));

                    // Send the response
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Close the websocket connection
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);
                    
                }

                // Verify if thread id exists
                if ( !fields.ContainsKey("ThreadId") ) {

                    // Sanitize the access token
                    string AccessToken = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(fields["AccessToken"]));

                    // Check if access token exists
                    if (AccessToken == null) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("NoTokenFound")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }

                    // Get the member's ID
                    string MemberId = new Tokens().GetTokenData(AccessToken ?? string.Empty, "MemberId");

                    // Verify if MemberId has no value
                    if (MemberId == "") {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("TokenNoValid")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }

                    // Get the email
                    ResponseDto<MemberDto> member = await membersRepository.GetMemberAsync(int.Parse(MemberId));

                    // Verify if member exists
                    if ( member.Result == null ) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("AccountNotFound")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }

                    // Start the background task
                    Task backgroundTask = Task.Run(async () => {

                        // Run until the websocket connection will be closed
                        while (webSocketActive == true) {

                            // Check for unseen messages
                            ResponseDto<bool> unseen = await messagesRepository.MessagesUnseenAsync(0, member.Result!.MemberId);

                            // Verify if unseen messages exist
                            if (unseen.Result == true) {

                                // Prepare the response
                                byte[] unseenMessages = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                                    success = true,
                                    unseen = 1
                                }));

                                // Send the response
                                await webSocket.SendAsync(new ArraySegment<byte>(unseenMessages), WebSocketMessageType.Text, true, CancellationToken.None);
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
                    
                } else {

                    // Sanitize the access token
                    string AccessToken = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(fields["AccessToken"]));  

                    // Sanitize the thread id
                    int threadId = int.Parse(HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(fields["ThreadId"])));

                    // Check if access token exists
                    if (AccessToken == null) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("NoTokenFound")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }

                    // Get the member's ID
                    string MemberId = new Tokens().GetTokenData(AccessToken ?? string.Empty, "MemberId");

                    // Verify if MemberId has no value
                    if (MemberId == "") {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("TokenNoValid")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }

                    // Get the email
                    ResponseDto<MemberDto> member = await membersRepository.GetMemberAsync(int.Parse(MemberId));

                    // Verify if member exists
                    if ( member.Result == null ) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("AccountNotFound")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }
                    // Get the thread's data
                    ResponseDto<ThreadDto> threadDto = await messagesRepository.GetThreadAsync(threadId, member.Result!.MemberId);

                    // Verify if thread exists
                    if ( threadDto.Result == null ) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("ThreadNotFound")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Close the websocket connection
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

                    }

                    // Start the background task
                    Task backgroundTask = Task.Run(async () => {

                        // Run until the websocket connection will be closed
                        while (webSocketActive == true) {

                            // Check for unseen messages
                            ResponseDto<bool> unseen = await messagesRepository.MessagesUnseenAsync(threadId, member.Result!.MemberId);

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
                                ResponseDto<TypingDto> typing = await messagesRepository.GetTypingAsync(threadId, 0);

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

            } else {

                // Prepare the response
                byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                    success = false,
                    message = new Strings().Get("AccessTokenThreadIdMissing")
                }));

                // Send the response
                await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

            }

            // Close the websocket connection
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, new Strings().Get("ClosingWebSocketConnection"), CancellationToken.None);

        }

    }

}