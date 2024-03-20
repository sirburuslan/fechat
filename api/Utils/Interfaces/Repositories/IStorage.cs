/*
 * @interface Storage
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This interface is used to create the storage classes
 */

// Namespace for general interfaces
namespace FeChat.Utils.Interfaces {

    // General Dtos to use the storage dto
    using FeChat.Models.Dtos;

    /// <summary>
    /// Interface for Storage
    /// </summary>
    public interface IStorage {

        /// <summary>
        /// Upload the file on external storage
        /// </summary>
        /// <param name="configuration">App configuration</param>
        /// <param name="file">Uploaded file</param>
        /// <returns>Url of the uploaded file or null</returns>
        Task<ResponseDto<StorageDto>> UploadAsync(IConfiguration configuration, IFormFile file);

    }

}