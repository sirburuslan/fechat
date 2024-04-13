/*
 * @class Feature Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-17
 *
 * This class is used for plans feature data transfer
 */

// Namespace for the plans features
namespace FeChat.Models.Dtos.Plans {

    // System Namespaces
    using System.Text.Encodings.Web;
    using System.Web;

    /// <summary>
    /// Dto for Plans Feature
    /// </summary>
    public class FeatureDto {

        /// <summary>
        /// Plan feature text container
        /// </summary>
        private string? _featureText; 

        /// <summary>
        /// Feature's ID
        /// </summary>
        public int FeatureId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Feature's name
        /// </summary>
        public required string FeatureText {
            get => _featureText!;
            set => _featureText = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();  
        }

    }

}