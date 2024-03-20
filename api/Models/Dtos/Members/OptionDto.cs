/*
 * @class Option Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used for option data transfer
 */


// Namespace for Members dtos
namespace FeChat.Models.Dtos.Members {

    /// <summary>
    /// Member Option Dto
    /// </summary>
    public class OptionDto {

        /// <summary>
        /// Option's ID
        /// </summary>
        public int OptionId { get; set; }        

        /// <summary>
        /// Member's ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Option's name
        /// </summary>
        public required string OptionName { get; set; }

        /// <summary>
        /// Option's value
        /// </summary>
        public required string OptionValue { get; set; }

    }

}