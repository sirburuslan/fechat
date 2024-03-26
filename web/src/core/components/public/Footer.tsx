/*
 * @component Footer
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This file contains the Footer component for home page
 */

'use client'

// Use the React hooks
import { useContext } from 'react';

// Import Script from Next
import Script from 'next/script'

// Import Link from Next
import Link from 'next/link';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Footer component
const Footer = (): React.JSX.Element => {

    // Website options
    let {websiteOptions} = useContext(WebsiteOptionsContext);

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);

    return (
        <>
            <div className="fc-footer">
                <div className="fc-footer-container">
                    <ul className="fc-footer-terms-links">
                        <li>
                            © { new Date().getFullYear() } { getWord('default', 'default_website_name', memberOptions['Language']) }
                        </li>
                        <li>
                            <Link href={(websiteOptions.PrivacyPolicy !== '')?websiteOptions.PrivacyPolicy:'#'}>
                                { getWord('default', 'default_privacy_policy', memberOptions['Language']) }
                            </Link>
                        </li>
                        <li>
                            <Link href={(websiteOptions.TermsOfService !== '')?websiteOptions.TermsOfService:'#'}>
                                { getWord('default', 'default_terms_of_service', memberOptions['Language']) }
                            </Link>
                        </li>                    
                    </ul>
                    <ul className="fc-footer-social-links">
                        <li>
                            <Link href="#">
                                { getIcon('IconBiFacebook') }
                            </Link>
                        </li>   
                        <li>
                            <Link href="#">
                                { getIcon('IconBiWhatsapp') }
                            </Link>
                        </li>
                        <li>
                            <Link href="#">
                                { getIcon('IconBiTwitterX') }
                            </Link>
                        </li>                                                        
                    </ul>                                                          
                </div>
            </div>
            {(websiteOptions.AnalyticsCode !== '')?(
                <Script id="fc-analytics-code" dangerouslySetInnerHTML={{ __html: websiteOptions.AnalyticsCode }} />
            ):''}
        </>
    );

};

// Export the Footer component
export default Footer;