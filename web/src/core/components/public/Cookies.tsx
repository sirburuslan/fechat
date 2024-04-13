/*
 * @component Cookies
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-13
 *
 * This file contains the Cookies component for home page
 */

'use client'

// Use the React hooks
import { useEffect, useState, useContext } from 'react';

// Import Link from Next
import Link from 'next/link';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Cookies component
const Cookies = (): React.JSX.Element => {

    // Website options
    const {websiteOptions} = useContext(WebsiteOptionsContext);

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);
    
    // Hook for show cookies modal
    const [showCookiesModal, hideCookiesModal] = useState(0);
    
    // Run code after component load
    useEffect((): void => {

        // Verify if the cookies modal should be showed
        if ( !SecureStorage.getItem('fc_hide_cookies_modal') ) {

            // Show modal
            hideCookiesModal(1);

        }

    }, []);

    return (
        <>
            {(showCookiesModal > 0)?(
                <div className="fc-cookies">
                    <div className="fc-cookies-top">
                        <h2>{ getWord('public', 'public_website_uses_cookies', memberOptions['Language']) }</h2>
                        <p>{ getWord('public', 'public_website_uses_cookies_description', memberOptions['Language']) } <Link href={(websiteOptions.Cookies !== '')?websiteOptions.Cookies:'#'}>{ getWord('public', 'public_read_more', memberOptions['Language']) }</Link></p>                                                       
                    </div>
                    <div className="fc-cookies-bottom">
                        <Link href="#" onClick={(e: React.MouseEvent): void => {
                            e.preventDefault();

                            // Hide modal
                            hideCookiesModal(0);

                            // Save cookie
                            SecureStorage.setItem('fc_hide_cookies_modal', 1);

                        }}>{ getWord('public', 'public_accept_all', memberOptions['Language']) }</Link> 
                        <Link href="#" onClick={(e: React.MouseEvent): void => {
                            e.preventDefault();

                            // Hide modal
                            hideCookiesModal(0);

                        }}>{ getWord('public', 'public_reject_all', memberOptions['Language']) }</Link>                                                    
                    </div>            
                </div>
            ):''}
        </>

    );

};

// Export the Cookies component
export default Cookies;