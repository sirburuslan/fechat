/*
 * @class Plans Meta Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-07
 *
 * This class is used for meta data transfer
 */

// Namespace for Admin Dtos
namespace FeChat.Models.Dtos.Plans {

    /// <summary>
    /// Plan Meta Dto
    /// </summary>
    public class PlanMetaDto {

        /// <summary>
        /// Meta's ID
        /// </summary>
        public int MetaId { get; set; }        

        /// <summary>
        /// Plan's ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Meta's Name
        /// </summary>
        public required string MetaName { get; set; }

        /// <summary>
        /// Meta's Value
        /// </summary>
        public string? MetaValue { get; set; }

    }

}