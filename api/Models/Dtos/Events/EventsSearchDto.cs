/*
 * @class Events Search Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This class is used to transfer the parameters for reading the events
 */

// Namespace for Dtos
namespace FeChat.Models.Dtos {

    // System Namespaces
    using System.Web;
    using System.Text.Encodings.Web;

    /// <summary>
    /// Events Search Dto
    /// </summary>
    public class EventsSearchDto {

        /// <summary>
        /// Year container
        /// </summary>
        private string? _year; 

        /// <summary>
        /// Month container
        /// </summary>
        private string? _month; 

        /// <summary>
        /// Date container
        /// </summary>
        private string? _date;   
        
        /// <summary>
        /// MemberId field
        /// </summary>
        public int MemberId { get; set; }            

        /// <summary>
        /// Year field
        /// </summary>
        public string? Year {
            get => _year;
            set => _year = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Month field
        /// </summary>
        public string? Month {
            get => _month;
            set => _month = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Date field
        /// </summary>
        public string? Date {
            get => _date;
            set => _date = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

    }

}