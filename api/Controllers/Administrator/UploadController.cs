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

    // Use the Mvc to get the controller
    using Microsoft.AspNetCore.Mvc;

    // Use the Authentication feature to get the access token
    using Microsoft.AspNetCore.Authentication;

    // Use Authorization for access restriction
    using Microsoft.AspNetCore.Authorization;

    // Use Cors libraries
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning library
    using Asp.Versioning;

    // Use general dtos
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use the General namespace to get the Tokens class
    using FeChat.Utils.General;

    // Use the Members Repositories
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Upload Manager
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    public class UploadController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public UploadController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Update the options
        /// </summary>
        /// <param name="file">Uploaded image</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Update response</returns>
        [Authorize]
        [HttpPost("image")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Image(IFormFile file, IMembersRepository membersRepository) {

            // Try to upload the file
            ResponseDto<StorageDto> uploadImage = await new ImageUpload().UploadAsync(_configuration, file);

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