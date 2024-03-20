/*
 * @class Image Upload
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to upload images
 */

// General Utils namespace
namespace FeChat.Utils.General {

    // Use Reflection to use the Assembly
    using System.Reflection;

    // Use general dtos
    using FeChat.Models.Dtos;

    // Load Storage
    using FeChat.Utils.Interfaces;

    /// <summary>
    /// This class has the scope to upload images for both users and administrators
    /// </summary>
    public class ImageUpload {

        /// <summary>
        /// Upload the image
        /// </summary>
        /// <param name="configuration">App configuration</param>
        /// <param name="file">Uploaded file</param>
        /// <returns>Url of the uploaded file or null</returns>
        public async Task<ResponseDto<StorageDto>> UploadAsync(IConfiguration configuration, IFormFile file) {

            // Get the default selected storage
            string defaultStorage = configuration.GetSection("Storage:Default").Value ?? string.Empty;

            // Check if storage is not selected
            if ( defaultStorage == null ) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("NoStorageSelected")
                };
                
            }

            // Create the storage class name
            string storageClass = "FeChat.Utils.Storage." + defaultStorage;

            // Get the assembly
            Assembly assembly = Assembly.Load("api");

            // Get type of the class
            Type? type = assembly.GetType(storageClass);

            // Check if type is not null
            if ( type == null ) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("StorageNotFound")
                };
                
            }

            // Verify if the file exists
            if (file == null || file.Length == 0) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("NoFileUploaded")
                };

            }

            // Check if file name exists
            if ( file.FileName == null ) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("NoValidFile")
                };
                
            }

            // Maximum allowed file size
            int maxFileSize = 2097152;

            // Get the file size in bytes
            int fileSize = (int) file.Length;

            // Verify if the file is too big
            if ( fileSize > maxFileSize ) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("ImageTooBig")
                };
                
            }

            // Get the file name
            string fileName = file.FileName;

            // Check if the file name has a valid extension
            string fileExtension = Path.GetExtension(fileName);

            // Supported extensions
            List<string> expectedExtensions = new() {
                ".jpg",
                ".jpeg",
                ".gif",
                ".png"
            };

            // Verify if the file has correct extension
            if ( !expectedExtensions.Any(ext => string.Equals(fileExtension, ext, StringComparison.OrdinalIgnoreCase)) ) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("NoValidFormat")
                };              

            }

            // Check if the image's content is valid
            if ( !IsFileContentValid(file, expectedExtensions) ) {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("NoValidFormat")
                };    

            }

            // Instantiate the class dynamically
            IStorage instance = (IStorage)Activator.CreateInstance(type)!;

            // Try to upload the file
            ResponseDto<StorageDto> uploadImage = await instance!.UploadAsync(configuration, file);

            // Check if the file was uploaded
            if ( uploadImage.Result != null ) {

                // Return a success response
                return new ResponseDto<StorageDto> {
                    Result = uploadImage.Result,
                    Message = null
                };

            } else {

                // Return error response
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = uploadImage.Message
                };

            }

        }

        /// <summary>
        /// Check if file content is valid
        /// </summary>
        /// <param name="file">Uploaded file</param>
        /// <param name="allowedContentTypes">Allowed formats</param>
        /// <returns></returns>
        protected bool IsFileContentValid(IFormFile file, List<string> allowedContentTypes) {

            // Read the first few bytes of the file to determine its signature
            byte[] fileSignature = new byte[8];

            // Open the request stream
            using Stream stream = file.OpenReadStream();
            
            // Read the file
            stream.Read(fileSignature, 0, fileSignature.Length);

            // List the supported formats
            foreach (string allowedContentType in allowedContentTypes) {

                // Check if format is supported
                if (HasFileSignature(fileSignature, allowedContentType)) {
                    return true;
                }

            }

            return false;

        }

        /// <summary>
        /// Verify if the signature is correct
        /// </summary>
        /// <param name="fileSignature">Uploaded file signature</param>
        /// <param name="allowedContentType">Allowed format for file</param>
        /// <returns></returns>
        protected bool HasFileSignature(byte[] fileSignature, string allowedContentType) {

            // Implement logic to check if the file signature matches the allowed content type
            return allowedContentType.ToLower() switch {
                ".jpg" => HasJPEGSignature(fileSignature),
                ".jpeg" => HasJPEGSignature(fileSignature),
                ".gif" => HasGIFSignature(fileSignature),
                ".png" => HasPNGSignature(fileSignature),
                _ => false,
            };
        }

        /// <summary>
        /// Check if the uploaded file is a valif jpeg
        /// </summary>
        /// <param name="fileSignature">File signature</param>
        /// <returns>Boolean true if the signature is correct</returns>
        private static bool HasJPEGSignature(byte[] fileSignature) {

            // Set jpeg signature
            byte[] jpegSignature = { 0xFF, 0xD8, 0xFF, 0xE0 };

            if (fileSignature.Length >= jpegSignature.Length) {

                for (int i = 0; i < jpegSignature.Length; i++) {

                    if (fileSignature[i] != jpegSignature[i]) {
                        return false;
                    }

                }

                return true;

            }

            return false;
            
        }

        /// <summary>
        /// Check if the uploaded file is a valif gif
        /// </summary>
        /// <param name="fileSignature">File signature</param>
        /// <returns>Boolean true if the signature is correct</returns>
        private static bool HasGIFSignature(byte[] fileSignature) {

            // Set gif signature
            byte[] gifSignature = { 0x47, 0x49, 0x46, 0x38 };

            if (fileSignature.Length >= gifSignature.Length) {

                for (int i = 0; i < gifSignature.Length; i++) {

                    if (fileSignature[i] != gifSignature[i]) {
                        return false;
                    }

                }

                return true;

            }

            return false;

        }

        /// <summary>
        /// Check if the uploaded file is a valif png
        /// </summary>
        /// <param name="fileSignature">File signature</param>
        /// <returns>Boolean true if the signature is correct</returns>
        private static bool HasPNGSignature(byte[] fileSignature) {

            // Set png signature
            byte[] pngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            if (fileSignature.Length >= pngSignature.Length) {

                for (int i = 0; i < pngSignature.Length; i++) {

                    if (fileSignature[i] != pngSignature[i]) {
                        return false;
                    }

                }

                return true;

            }

            return false;
        }

    }

}