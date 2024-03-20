/*
 * @type Options
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the type for the app's options
 */


// Options type
type typeOptions = undefined | {
    website?: {[key: string]: string},
    member?: {[key: string]: string},
    message?: any
};

// Export the type
export default typeOptions;