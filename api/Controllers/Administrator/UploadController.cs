/*
 * @class Upload Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to upload files
 */

// Administrator Controllers namespace
namespace FeChat.Controllers.Administrator {

    // System Namespaces
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Asp.Versioning;

    // App Namespaces
    using Models.Dtos;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Upload Manager
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    public class UploadController: Controller {

        /// <summary>
        /// App Settings container.
        /// </summary>
        private readonly AppSettings _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="options">All App Options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public UploadController(IOptions<AppSettings> options) {

            // Save the configuration
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), new Strings().Get("OptionsNotFound"));
            
        }

        /// <summary>
        /// Update the options
        /// </summary>
        /// <param name="file">Uploaded image</param>
        /// <returns>Update response</returns>
        [Authorize]
        [HttpPost("image")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Image(IFormFile file) {

            // Try to upload the file
            ResponseDto<StorageDto> uploadImage = await new ImageUpload().UploadAsync(_options.Storage, file);

            // Check if the file was uploaded
            if ( uploadImage.Result != null ) {

                // Create a success response
                var response = new {
                    success = true,
                    uploadImage.Result.FileUrl
                };

                // Return a json
                return new JsonResult(response);

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = uploadImage.Message
                });

            }

        }

    }

}