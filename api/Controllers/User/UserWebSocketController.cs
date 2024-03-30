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

    // Use the classes to work with Websocket communications
    using System.Net.WebSockets;

    // Use Text for Encoding
    using System.Text;

    // Use Web encodings
    using System.Text.Encodings.Web;

    // Use Json classes for json encoding
    using System.Text.Json;

    // Use Net web classes for html sanitization
    using System.Web;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the MVC support for Controllers feature
    using Microsoft.AspNetCore.Mvc;

    // Use the versioning support to add version in url
    using Asp.Versioning;

    // Use General dto for responses
    using FeChat.Models.Dtos;

    // Use Members Dto
    using FeChat.Models.Dtos.Members;

    // Use Messages Dto
    using FeChat.Models.Dtos.Messages;
    
    // Use General Utils for strings
    using FeChat.Utils.General;

    // Use interfaces for Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;
    
    // Use interfaces for Messages Repositories
    using FeChat.Utils.Interfaces.Repositories.Messages;

    /// <summary>
    /// Websocket Controller for User
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/websocket")]
    public class UserWebSocketController: Controller {

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
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> QueueRequest(HttpContext context, IMembersRepository membersRepository, IMessagesRepository messagesRepository) {
Console.WriteLine(1);
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
Console.WriteLine(2);
                    // Prepare the response
                    byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                        success = false,
                        message = new Strings().Get("NoTokenFound")
                    }));

                    // Send the response
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Return empty result
                    return new EmptyResult();
                    
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

                        // Return empty result
                        return new EmptyResult();

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

                        // Return empty result
                        return new EmptyResult();

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

                        // Return empty result
                        return new EmptyResult();

                    }

                    // Verify if member is not administrator
                    if ( member.Result.Role != 1 ) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("NoPermissionsForAction")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Return empty result
                        return new EmptyResult();

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

                        return BadRequest(ex.Message);

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

                        // Return empty result
                        return new EmptyResult();

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

                        // Return empty result
                        return new EmptyResult();

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

                        // Return empty result
                        return new EmptyResult();

                    }

                    // Verify if member is not administrator
                    if ( member.Result.Role != 1 ) {

                        // Prepare the response
                        byte[] response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
                            success = false,
                            message = new Strings().Get("NoPermissionsForAction")
                        }));

                        // Send the response
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Return empty result
                        return new EmptyResult();

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

                        // Return empty result
                        return new EmptyResult();

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

                        return BadRequest(ex.Message);

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

            return new EmptyResult();

        }

    }

}