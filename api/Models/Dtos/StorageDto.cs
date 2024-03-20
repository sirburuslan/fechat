/*
 * @class Storage Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-30
 *
 * This class is used to return the storage's response
 */

// Namespace for Dtos
namespace FeChat.Models.Dtos {

    /// <summary>
    /// A generic type for responses
    /// </summary>
    public class StorageDto {

        /// <summary>
        /// File's URL
        /// </summary>
        public required string FileUrl { get; set; }

    }

}