/*
 * @type Header
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the type for the header in the rest requests
 */

// Type for post header
type typePostHeader = {
    headers: {
        [key: string]: string | undefined
    }
};

// Export the headers
export type {
    typePostHeader
};