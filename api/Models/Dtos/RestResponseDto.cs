/*
 * @class Rest Response Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-07
 *
 * This class is used to handle the responses
 */

// Namespace for Dtos
namespace FeChat.Models.Dtos {

    /// <summary>
    /// Rest Response Dto
    /// </summary>
    public class RestResponseDto {

        /// <summary>
        /// Response Status
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Success response content
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Message for the response
        /// </summary>
        public string? Message { get; set; }

    }

}