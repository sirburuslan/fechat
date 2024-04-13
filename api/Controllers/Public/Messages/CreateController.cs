/*
 * @class Messages Create Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the messages for guests
 */

// Namespace for Public Messages Controllers
namespace FeChat.Controllers.Public.Messages {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Models.Dtos.Websites;
    using Utils.Configuration;
    using Utils.General;
    using Utils.Interfaces.Repositories.Messages;
    using Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Create Messages Controller
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/messages")]
    public class CreateController: Controller {

        /// <summary>
        /// App Settings container.
        /// </summary>
        private readonly AppSettings _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateController"/> class.
        /// </summary>
        /// <param name="options">All App Options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public CreateController(IOptions<AppSettings> options) {

            // Save the configuration
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), new Strings().Get("OptionsNotFound"));
            
        }

        /// <summary>
        /// Create a message
        /// </summary>
        /// <param name="messageDto">Message content</param>
        /// <param name="websitesRepository">An instance for the websites repository</param>
        /// <param name="messagesRepository">An instance for the messages repository</param>
        /// <returns>Success or error message</returns>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDto messageDto, IWebsitesRepository websitesRepository, IMessagesRepository messagesRepository) {

            // Check if website id exists
            if ( messageDto.WebsiteId == 0 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteIdMissing")
                });
                
            }

            // Check if message has at least 3 characters
            if ( (messageDto.Message != null) && (messageDto.Message.Length < 2) ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("MessageTooShort")
                });

            }

            // Check if Thread Secret is empty
            if ( (messageDto.ThreadSecret == null) || (messageDto.ThreadSecret == "") ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadSecretMissing")
                });

            }

            // Get website from the database
            ResponseDto<WebsiteDto> websiteResponse = await websitesRepository.GetWebsiteInfoAsync(messageDto.WebsiteId);

            // Check if website exists
            if ( websiteResponse.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteNotFound")
                });
                
            }

            // Verify if the chat is enabled
            if ( websiteResponse.Result.Enabled != 1 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ChatDisabled")
                });
                
            }

            // Create the thread's data
            ThreadDto thread = new() {
                WebsiteId = messageDto.WebsiteId,
                ThreadSecret = messageDto.ThreadSecret
            };

            // Get the thread from the database
            ResponseDto<ThreadDto> getThread = await messagesRepository.GetThreadByWebsiteIdAsync(thread);

            // Verify if thread exists
            if ( getThread.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadNotFound")
                });

            }

            // Set thread id to the message
            messageDto.ThreadId = getThread.Result!.ThreadId;

            // Set thread secret to the message
            messageDto.ThreadSecret = getThread.Result!.ThreadSecret;

            // Save the message
            ResponseDto<MessageDto> responseDto = await messagesRepository.CreateMessageAsync(messageDto);

            // Verify if the messages was created
            if ( responseDto.Result != null ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    message = responseDto.Message
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = responseDto.Message
                });

            }

        }

        /// <summary>
        /// Create a message with attachments
        /// </summary>
        /// <param name="files">Uploaded files</param>
        /// <param name="websiteId">Website Id</param>
        /// <param name="threadSecret">Thread Secret</param>
        /// <param name="websitesRepository">An instance for the websites repository</param>
        /// <param name="messagesRepository">An instance for the messages repository</param>
        /// <returns>Upload response</returns>
        [HttpPost("attachments/{websiteId}/{threadSecret}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CreateMessageAttachments(List<IFormFile> files, int websiteId, string threadSecret, IWebsitesRepository websitesRepository, IMessagesRepository messagesRepository) {            

            // Verify if there are more than 3 files
            if ( files.Count > 3 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("TooManyImages")
                });
                
            }

            // Check if website id exists
            if ( websiteId == 0 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteIdMissing")
                });
                
            }

            // Check if Thread Secret is empty
            if ( (threadSecret == null) || (threadSecret == "") ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadSecretMissing")
                });

            }

            // Get website from the database
            ResponseDto<WebsiteDto> websiteResponse = await websitesRepository.GetWebsiteInfoAsync(websiteId);

            // Check if website exists
            if ( websiteResponse.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("WebsiteNotFound")
                });
                
            }

            // Verify if the chat is enabled
            if ( websiteResponse.Result.Enabled != 1 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ChatDisabled")
                });
                
            }

            // Create the thread's data
            ThreadDto thread = new() {
                WebsiteId = websiteId,
                ThreadSecret = threadSecret
            };

            // Get the thread from the database
            ResponseDto<ThreadDto> getThread = await messagesRepository.GetThreadByWebsiteIdAsync(thread);

            // Verify if thread exists
            if ( getThread.Result == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("ThreadNotFound")
                });

            }

            // Uploaded images container
            List<string> uploadedImages = new();

            // Total files
            int totalFiles = files.Count;

            // List the files
            for ( int f = 0; f < totalFiles; f++ ) {

                // Try to upload the file
                ResponseDto<StorageDto> uploadImage = await new ImageUpload().UploadAsync(_options.Storage, files[f]);

                // Check if the file was uploaded
                if ( uploadImage.Result != null ) {

                    // Save the url
                    uploadedImages.Add(uploadImage.Result.FileUrl);

                }

            }

            // Verify if files were uploaded
            if ( uploadedImages.Count < 1 ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("MessageNotCreated")
                });
                
            }

            // Create the message parameters
            MessageDto messageDto = new() {
                ThreadId = getThread.Result.ThreadId
            };

            // Save the message
            ResponseDto<MessageDto> responseDto = await messagesRepository.CreateMessageAsync(messageDto);

            // Verify if the messages was created
            if ( responseDto.Result != null ) {

                // Count uploaded and saved images
                int totalUploaded = 0;

                // Total files
                int total = uploadedImages.Count;

                // List the files
                for ( int f = 0; f < total; f++ ) {

                    // Create attachment data
                    AttachmentDto attachmentDto = new() {
                        MessageId = responseDto.Result.MessageId,
                        Link = uploadedImages[f]
                    };

                    // Save attachment
                    ResponseDto<AttachmentDto> saveAttachment = await messagesRepository.CreateAttachment(attachmentDto);

                    // Check if the attachment was saved
                    if ( saveAttachment.Result != null ) {

                        // Increase the number of uploaded files
                        totalUploaded++;

                    }

                }

                // Verify if files were uploaded
                if ( totalUploaded > 0 ) {

                    // Return a json
                    return new JsonResult(new {
                        success = true,
                        message = responseDto.Message
                    });

                } else {

                    // Delete the message
                    await messagesRepository.DeleteMessageAsync(responseDto.Result.MessageId);

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("MessageNotCreated")
                    });
                    
                }

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = responseDto.Message
                });

            }

        }        

    }

}