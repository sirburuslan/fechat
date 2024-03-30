/*
 * @page Google Callback
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-22
 *
 * This file contains the page google callback which handles the response code
 */

'use client'

// Import the React hooks
import { useContext, useState, useEffect, Suspense } from 'react';

// Import the router function
import { useSearchParams, useRouter } from 'next/navigation';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getWord, getOptions, getToken, updateOptions } from '@/core/inc/incIndex';

// Import the types
import { typeOptions, typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {WebsiteOptionsContext, MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Create the page component
const Page = (): React.JSX.Element => {

    // Get the router
    let router = useRouter();

    // Gets the search params
    let searchParams = useSearchParams();
    
    // Message container
    let [message, setMessage] = useState('');

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Get all options
    let getOptionsAll = async (): Promise<void> => {

        // Request the options
        let optionsList: {success: boolean, options?: typeOptions} = await getOptions();

        // Update memberOptions
        updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

    }

    // Monitor websiteOptions change
    useEffect((): void => {

        // Check if Google is configured
        if ( websiteOptions.Default === '0' ) {

            // Verify if Google is not configured
            if ( (websiteOptions.GoogleAuthEnabled !== '1') || (websiteOptions.GoogleClientId === '') || (websiteOptions.GoogleClientSecret === '') ) {

                // Set error message
                setMessage(getWord('auth', 'auth_google_is_not_configured'));

            } else if ( message === '' ) {

                // Get the code from url
                let code: string | null = searchParams.get('code');

                // Change the code to access token and sign up
                (async (): Promise<void> => {

                    try {

                        // Generate a new csrf token
                        let csrfToken: typeToken = await getToken();
            
                        // Check if csrf token is missing
                        if ( !csrfToken.success ) {
            
                            // Show error notification
                            throw new Error(getWord('errors', 'error_csrf_token_not_generated'));
            
                        }
            
                        // Prepare the headers
                        let headers: typePostHeader = {
                            headers: {
                                CsrfToken: csrfToken.token
                            }
                        };
            
                        // Update the fields
                        await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/auth/google', {
                            code: code
                        }, headers)
            
                        // Process the response
                        .then(async (response: AxiosResponse) => {

                            // Verify if the response is successfully
                            if ( response.data.success ) {
                                
                                // Save the jwt token
                                SecureStorage.setItem('fc_jwt', response.data.member.token);

                                // Save the member role
                                SecureStorage.setItem('fc_role', response.data.member.role);

                                // Reload the options
                                getOptionsAll();

                                // Add validating message
                                setMessage(getWord('auth', 'auth_validating'));                                

                                // Set a pause
                                setTimeout((): void => {

                                    // Check if member is administrator
                                    if ( response.data.member.role === 0 ) {

                                        // Redirect the administrator to the dashboard page
                                        router.push(process.env.NEXT_PUBLIC_SITE_URL + 'admin/dashboard');

                                    } else {

                                        // Redirect the user to the dashboard page
                                        router.push(process.env.NEXT_PUBLIC_SITE_URL + 'user/dashboard');

                                    }

                                }, 2000);
            
                            } else if ( typeof response.data.message !== 'undefined' ) {
            
                                // Throw error message
                                throw new Error(response.data.message);
            
                            } else {
            
                                // Keys container
                                let keys: string[] = Object.keys(response.data);
            
                                // Throw error message
                                throw new Error(response.data[keys[0]][0]);                    
            
                            }
            
                        })
            
                        // Catch the error message
                        .catch((error: AxiosError): void => {
            
                            // Check if error message exists
                            if ( typeof error.message !== 'undefined' ) {
            
                                // Throw error message
                                throw new Error(error.message);                    
            
                            }
                            
            
                        })
            
                    } catch (error: unknown) {          
            
                        // Check if error is known
                        if ( error instanceof Error ) {
            
                            // Add error message
                            setMessage(error.message);
            
                        } else {
            
                            // Display in the console the error
                            console.log(error);
            
                        }
            
                    }

                })();            

            }

        }

    }, [websiteOptions]);

    return (
        <Suspense>
            <div className="px-8 pt-6 pb-6 mb-4 fc-auth-validating">
                <div>
                    {(message !== '')?(
                        message 
                    ):(
                        getWord('auth', 'auth_validating')
                    )}
                </div>
            </div>
        </Suspense>
    );

};

// Export the Page component
export default Page;