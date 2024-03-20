/*
 * @inc Tokens
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file generates csrf tokens
 */

// Import axios module
import axios, { AxiosResponse } from 'axios';

// Import incs
import { getWord } from '@/core/inc/incIndex';

// Import types
import { typeToken } from '@/core/types/typesIndex';

// Generate a new token
const getToken = async (): Promise<typeToken> => {

    try {

        // Request the fields value
        let response: AxiosResponse = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/csrf/generate');

        // Verify if the response is successfully
        if ( response.data.success ) {

            return {
                success: true,
                token: response.data.token.requestToken
            };

        } else {

            // Throw error notification
            throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

        }  

    } catch (error: unknown) {

        // Check if error is known
        if ( error instanceof Error ) {

            // Return error
            return {
                success: false,
                message: error.message
            };

        } else {

            // Display in the console the error
            console.log(error);

            // Return error
            return {
                success: false
            };            

        }

    }

}

// Export the getToken function
export default getToken;