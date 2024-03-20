/*
 * @class Sanitize
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-13
 *
 * This class is used to sanitize strings
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // Use the RegularExpressions for Regex usage
    using System.Text.RegularExpressions;

    /// <summary>
    /// Sanitize data
    /// </summary>
    public class Sanitize {

        /// <summary>
        /// Sanitize string
        /// </summary>
        /// <param name="text">Text which should be sanitized</param>
        /// <returns>Sanitized text</returns>
        public string String(string text) {

            // Allowed characters
            string pattern = "[^a-zA-Z0-9 ]";

            // Use Regex.Replace to remove characters that match the pattern
            string sanitizedString = Regex.Replace(text, pattern, "");

            // Return santized string
            return sanitizedString;

        }

    }

}