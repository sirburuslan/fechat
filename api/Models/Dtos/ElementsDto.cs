/*
 * @class Elements Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-12
 *
 * This class is used to transfer the response with generic elements
 */

// Namespace for Dtos
namespace FeChat.Models.Dtos {

    /// <summary>
    /// Search Dto
    /// </summary>
    public class ElementsDto<T> {

        /// <summary>
        /// Elements field
        /// </summary>
        public required List<T> Elements { get; set; }

        /// <summary>
        /// Total number field
        /// </summary>
        public int Total { get; set; }        

        /// <summary>
        /// Page number field
        /// </summary>
        public int Page { get; set; }

    }

}