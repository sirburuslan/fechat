/*
 * @class Strings
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-19
 *
 * This class is used to read the strings resources
 */

// General Utils namespace
namespace FeChat.Utils.General {

    // Use the Resources to access the Resources Manager
    using System.Resources;

    /// <summary>
    /// This class was created to manage better the strings
    /// </summary>
    public class Strings {

        /// <summary>
        /// Get a string by name and culture
        /// </summary>
        /// <param name="name">Name of the string</param>
        /// <returns>string with value</returns>
        public string Get(string name) {

            // Init the Resource Manager class
            ResourceManager rm = new("api.Resources.Strings", typeof(Strings).Assembly);

            // Return string or empty
            return rm.GetString(name) ?? string.Empty;

        }

    }

}