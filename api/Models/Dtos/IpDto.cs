/*
 * @class Ip Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-18
 *
 * This class is used to transfer the ip's data
 */

// Namespace for general dtos
namespace FeChat.Models.Dtos {

    /// <summary>
    /// IP Dto
    /// </summary>
    public class IpDto {

        /// <summary>
        /// Ip field
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Country Code field
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// Country Name field
        /// </summary>
        public string? CountryName { get; set; }

        /// <summary>
        /// Region Name field
        /// </summary>
        public string? RegionName { get; set; }

        /// <summary>
        /// City Name field
        /// </summary>
        public string? CityName { get; set; }

        /// <summary>
        /// Latitude field
        /// </summary>
        public string? Latitude { get; set; }

        /// <summary>
        /// Longitude field
        /// </summary>
        public string? Longitude { get; set; }

        /// <summary>
        /// Zip Code field
        /// </summary>
        public string? ZipCode { get; set; }

        /// <summary>
        /// Time Zone field
        /// </summary>
        public string? TimeZone { get; set; }

    }

}