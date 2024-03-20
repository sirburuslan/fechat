/*
 * @hook Use Form Registration
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains a custom hook for the registration page to validate the form input
 */

// Import the React hooks
import { useState, useEffect } from 'react';

// Import the words
import {getWord} from '@/core/inc/incIndex';

// Create the useFormRegistration hook
const useFormRegistration = (values: {
    email: string,
    password: string
}): {
    email: string,
    password: string
} => {

    // Current time
    let ctime: number = Date.now();

    // Set a counter
    let [count, setCount] = useState(0);

    // Process the response
    let [response, setData] = useState({email: '', password: ''});

    // Register useEffect hook for monitoring the count changes
    useEffect((): void => {

        // Set a pause
        setTimeout((): void => {

            // Check if count is 1
            if ( Date.now() > count ) {
                
                // Verify if email is missing
                if ( values.email.length === 0 ) {

                    // Set error message
                    setData({
                        email: '',
                        password: response.password
                    });
                    
                } else {
       
                    // Verify if email is valid
                    if (!/\S+@\S+\.\S+/.test(values.email)) {
                        
                        // Set error message
                        setData({
                            email: getWord('auth', 'auth_email_not_valid'),
                            password: response.password
                        });
        
                    } else {

                        // Set error message
                        setData({
                            email: '',
                            password: response.password
                        });
                        
                    }
        
                }

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

// Export the useFormRegistration hook
export default useFormRegistration;