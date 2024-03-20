/*
 * @class Tokens
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used read the data from the jwt token and validates the anti-csrf tokens
 */

// The General namespace
namespace FeChat.Utils.General {

    // Use the Jwy for token reading
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>
    /// This class provides some methods to works with the Jwt tokens
    /// </summary>
    public class Tokens {

        /// <summary>
        /// Get token data by claim field
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="field">Token field</param>
        /// <returns>string with field's data</returns>
        public string GetTokenData(string accessToken, string field) {

            // Default response
            var response = string.Empty;

            // Get the token handler
            var handler = new JwtSecurityTokenHandler();

            // Verify if the token is readable
            if (handler.ReadToken(accessToken) is JwtSecurityToken jsonToken) {

                // Get data by field
                var fieldData = jsonToken.Claims.FirstOrDefault(c => c.Type == field);

                // Verify if data exists
                if ( fieldData != null ) {

                    // Replace the default response
                    response = fieldData.Value;

                }

            }

            return response;

        }

    }

}