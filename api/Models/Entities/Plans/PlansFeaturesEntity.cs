/*
 * @class Plan Features Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-15
 *
 * This class is used build the Plan Features entity
 */

// Namespace for the Plans Entities
namespace FeChat.Models.Entities.Plans {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Plans Features Entity
    /// </summary>
    public class PlansFeaturesEntity {
        
        /// <summary>
        /// Feature ID
        /// </summary>
        [Key]
        [Required]
        public int FeatureId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        [Required]
        public int PlanId { get; set; }

        /// <summary>
        /// Feature Text
        /// </summary>
        [Required]
        [MaxLength(250)]
        public required string FeatureText { get; set; }

    }

}