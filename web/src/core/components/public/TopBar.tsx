/*
 * @component Top Bar
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-16
 *
 * This file contains the Top Bar component for home page
 */

'use client'

// Use the React hooks
import { useContext, useState, useEffect, SyntheticEvent } from 'react';

// Import Link from Next
import Link from 'next/link';

// Import Image from Next
import Image from 'next/image';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Top Bar component
const TopBar = (): React.JSX.Element => {

    // Website options
    const {websiteOptions} = useContext(WebsiteOptionsContext);

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext);

    // Hook for auth links
    const [authLinks, updateAuthLinks] = useState(0);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = (websiteOptions.WebsiteName !== '')?websiteOptions.WebsiteName + ' | ' + getWord('public', 'public_chat_for_every_website', memberOptions['Language']):getWord('default', 'default_website_name') + ' | ' + getWord('public', 'public_chat_for_every_website', memberOptions['Language']);

    };

    // Run code after component load
    useEffect((): void => {

        // Check if jwt token is saved
        if ( SecureStorage.getItem('fc_jwt') ) {

            // Replace the auth links
            updateAuthLinks(1);

        }

    }, []);

    /**
     * Scroll Page
     * 
     * @param Event e 
     */
    const scrollPage = (e: React.MouseEvent): void => {
        e.preventDefault();

        // Check if href is features
        if ( e.currentTarget.getAttribute('href') === '#features' ) {

            // Get features position from top
            const featuresTopPosition = (document.getElementsByClassName('fc-features')[0] as HTMLElement).offsetTop;

            // Scroll the page
            window.scrollTo({ top: featuresTopPosition, behavior: 'smooth' });
            
        } else if ( e.currentTarget.getAttribute('href') === '#pricing' ) {

            // Get plans position from top
            const plansTopPosition = (document.getElementsByClassName('fc-plans')[0] as HTMLElement).offsetTop;

            // Scroll the page
            window.scrollTo({ top: plansTopPosition, behavior: 'smooth' });
            
        } else if ( e.currentTarget.getAttribute('href') === '#faq' ) {

            // Get faq position from top
            const faqTopPosition = (document.getElementsByClassName('fc-faq')[0] as HTMLElement).offsetTop;

            // Scroll the page
            window.scrollTo({ top: faqTopPosition, behavior: 'smooth' });
            
        }

    };

    return (
        <div className="fc-top-bar">
            <div className="fc-top-bar-container">
                <div className="fc-top-bar-logo">
                    <Link href={process.env.NEXT_PUBLIC_SITE_URL as unknown as URL}>
                        {(websiteOptions.HomePageLogo != '')?(
                            <Image src={websiteOptions.HomePageLogo} layout="fill" alt="Website Logo" className="fc-dashboard-sidebar-header-logo-large" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                        ):((websiteOptions.WebsiteName !== '')?websiteOptions.WebsiteName:getWord('default', 'default_website_name'))}
                    </Link>
                </div>
                <div className="fc-top-bar-menu">
                    <ul>
                        <li>
                            <Link href="#features" onClick={scrollPage}>{ getWord('public', 'public_features', memberOptions['Language']) }</Link>
                        </li>
                        <li>
                            <Link href="#pricing" onClick={scrollPage}>{ getWord('public', 'public_pricing', memberOptions['Language']) }</Link>
                        </li> 
                        <li>
                            <Link href="#faq" onClick={scrollPage}>{ getWord('public', 'public_faq', memberOptions['Language']) }</Link>
                        </li>                                               
                    </ul>
                </div>
                <div className="fc-top-bar-auth">
                    {(authLinks > 0)?(
                        <>
                            {( SecureStorage.getItem('fc_role') === 0 )?(
                                <Link href="/admin/dashboard" className="fc-auth-sign-in" scroll={false}>{ getWord('public', 'public_dashboard', memberOptions['Language']) }</Link>
                            ):(
                                <Link href="/user/dashboard" className="fc-auth-sign-in" scroll={false}>{ getWord('public', 'public_dashboard', memberOptions['Language']) }</Link>
                            )}
                            
                        </>
                    ):(
                        <>
                            <Link href="/auth/signin" className="fc-auth-sign-in" scroll={false}>{ getWord('auth', 'auth_sign_in', memberOptions['Language']) }</Link>
                            {(websiteOptions.RegistrationEnabled === '1')?(
                                <Link href="/auth/registration" className="fc-auth-sign-up" scroll={false}>{ getWord('auth', 'auth_sign_up', memberOptions['Language']) }</Link>
                            ):''}
                        </>                       
                    )}
                </div>
            </div>
        </div>
    );

};

// Export the Top Bar
export default TopBar;