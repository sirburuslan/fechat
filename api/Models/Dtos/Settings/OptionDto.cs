/*
 * @class Option Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-28
 *
 * This class is used for option data transfer
 */

// Namespace for Settings Dtos
namespace FeChat.Models.Dtos.Settings {

    /// <summary>
    /// Settings Option Dto
    /// </summary>
    public class OptionDto {

        /// <summary>
        /// Option's ID
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// Option's Name
        /// </summary>
        public required string OptionName { get; set; }

        /// <summary>
        /// Option's Value
        /// </summary>
        public string? OptionValue { get; set; }

    }

}