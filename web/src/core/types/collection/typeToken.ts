/*
 * @type Token
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the type for the csrf token generation
 */

// Type for token
type typeToken = {
    success: boolean;
    token?: string | undefined;
    message?: string | undefined;
};

// Export the type
export default typeToken;