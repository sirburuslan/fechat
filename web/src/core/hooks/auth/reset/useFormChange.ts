/*
 * @hook Use Form Change
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains a custom hook for the password change page to validate the form input
 */


// Import the React hooks
import { useState, useEffect } from 'react';

// Create the useFormChange hook
const useFormChange = (values: {
    password: string,
    repeatPassword: string
}): {
    password: string,
    repeatPassword: string
} => {

    // Current time
    const ctime: number = Date.now();

    // Set a counter
    const [count, setCount] = useState(0);

    // Process the response
    const [response, setData] = useState({password: '', repeatPassword: ''});

    // Register useEffect hook for monitoring the count changes
    useEffect((): void => {

        // Set a pause
        setTimeout((): void => {

            // Check if count is 1
            if ( Date.now() > count ) {

                // Set error message
                setData({
                    password: response.password,
                    repeatPassword: response.repeatPassword
                });

            }

        }, 1000);

    }, [count]);

    // Register useEffect hook for monitoring the values changes
    useEffect((): void => {

        // Set count
        setCount(ctime + 1000);

    }, [values]);

    return response;

}

// Export the useFormChange hook
export default useFormChange;