/*
 * @class Db
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-04-07
 *
 * This class is used organize the app's settings
 */

// Namespace for Configuration Utils
namespace FeChat.Utils.Configuration {

    /// <summary>
    /// App Settings Organizer
    /// </summary>
    public class AppSettings {

        /// <summary>
        /// Site Url Field
        /// </summary>
        public required string SiteUrl { get; set; }

        /// <summary>
        /// Jwt Settings Field
        /// </summary>
        public required JwtSettingsFormat JwtSettings { get; set; }

        /// <summary>
        /// Storage Field
        /// </summary>
        public required StorageFormat Storage { get; set; }

        /// <summary>
        /// Available Logging Options
        /// </summary>
        public required LoggingOptions Logging { get; set; }

        /// <summary>
        /// I'm not using this for CORS because I need url even for callbacks and I'm using SiteUrl instead
        /// </summary>
        public required string AllowedHosts { get; set; }

        /// <summary>
        /// Stores connection details for external data sources
        /// </summary>
        public required ConnectionStringsFormat ConnectionStrings { get; set; }

        /// <summary>
        /// Jwt Settings Format
        /// </summary>
        public class JwtSettingsFormat {

            /// <summary>
            /// Issuer for Jwt Generation
            /// </summary>
            public required string Issuer { get; set; }

            /// <summary>
            /// Audience for Jwt Generation
            /// </summary>            
            public required string Audience { get; set; }

            /// <summary>
            /// Secret Key for Jwt Generation
            /// </summary>                   
            public required string Key { get; set; }

        }

        /// <summary>
        /// Storage Options Container
        /// </summary>
        public class StorageOptions {

            /// <summary>
            /// Client Id for Storage Api
            /// </summary>
            public required string ClientId { get; set;}

        }

        /// <summary>
        /// Storage Format for Api Settings
        /// </summary>
        public class StorageFormat {

            /// <summary>
            /// Default Storage Api
            /// </summary>
            public required string Default { get; set; }

            /// <summary>
            /// List With Available Storage's Apis
            /// </summary>
            public required Dictionary<string, StorageOptions> List { get; set;}

        }

        /// <summary>
        /// Available Logging Options Format
        /// </summary>
        public class LoggingOptions {

            /// <summary>
            /// Logging Levels
            /// </summary>
            public required Dictionary<string, string> LogLevel { get; set; }

        }

        /// <summary>
        /// Format for Connection Strings
        /// </summary>
        public class ConnectionStringsFormat {

            /// <summary>
            /// Connection String by Default
            /// </summary>
            public required string DefaultConnection { get; set; }

        }

    }

}