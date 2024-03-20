/*
 * @class Restriction Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-15
 *
 * This class is used for plans restriction data transfer
 */

// Namespace for the plans restrictions
namespace FeChat.Models.Dtos.Plans {

    /// <summary>
    /// Dto for Plans Restriction
    /// </summary>
    public class RestrictionDto {

        /// <summary>
        /// Restriction's ID
        /// </summary>
        public int RestrictionId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Restriction's name
        /// </summary>
        public required string RestrictionName { get; set; }

        /// <summary>
        /// Restriction's value
        /// </summary>
        public int RestrictionValue { get; set; }

    }

}