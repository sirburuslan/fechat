/*
 * @page Google Access
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-19
 *
 * This file contains the page google access which allows to sign up and login with Google
 */

'use client'

// Import some React's hooks
import { useEffect, useContext, useState } from 'react';

// Import the router function
import { useRouter } from 'next/navigation';

// Import the incs
import { getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext } from '@/core/contexts/OptionsContext';

// Create the page component
const Page = (): React.JSX.Element => {

    // Get the router
    let router = useRouter();

    // Error message container
    let [errorMessage, setErrorMessage] = useState('');

    // Website options
    let { websiteOptions } = useContext(WebsiteOptionsContext);

    // Monitor websiteOptions change
    useEffect((): void => {

        // Check if Google is configured
        if ( websiteOptions.Default === '0' ) {

            // Verify if Google is not configured
            if ( (websiteOptions.GoogleAuthEnabled !== '1') || (websiteOptions.GoogleClientId === '') || (websiteOptions.GoogleClientSecret === '') ) {

                // Set error message
                setErrorMessage(getWord('auth', 'auth_google_is_not_configured'));

            } else {

                //Login params
                let authParams = {
                    client_id: websiteOptions.GoogleClientId,
                    scope: 'https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email',
                    redirect_uri: process.env.NEXT_PUBLIC_SITE_URL + 'auth/google/callback',
                    response_type: 'code',
                    access_type: 'offline',
                    prompt: 'consent'
                };

                // Build the login URL
                let loginUrl = `https://accounts.google.com/o/oauth2/v2/auth?${new URLSearchParams(authParams)}`;

                // Redirect to the login page
                router.push(loginUrl);                

            }

        }

    }, [websiteOptions]);

    return (
        <>
            <div className="px-8 pt-6 pb-6 mb-4 fc-auth-redirecting">
                <div>
                    {(errorMessage !== '')?(
                        errorMessage 
                    ):(
                        getWord('auth', 'auth_redirecting')
                    )}
                </div>
            </div>
        </>
    );

};

// Export the Page component
export default Page;