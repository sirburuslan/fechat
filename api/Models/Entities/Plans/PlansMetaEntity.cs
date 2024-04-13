/*
 * @class Plan Meta Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-07
 *
 * This class is used build the Plan Meta entity
 */

// Namespace for the Plans Entities
namespace FeChat.Models.Entities.Plans {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Plans Meta Entity
    /// </summary>
    public class PlansMetaEntity {
        
        /// <summary>
        /// Meta ID
        /// </summary>
        [Key]
        [Required]
        public int MetaId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        [Required]
        public int PlanId { get; set; }

        /// <summary>
        /// Meta Name
        /// </summary>
        [Required]
        [MaxLength(250)]
        public required string MetaName { get; set; }

        /// <summary>
        /// Meta Value
        /// </summary>
        public string? MetaValue { get; set; }

    }

}