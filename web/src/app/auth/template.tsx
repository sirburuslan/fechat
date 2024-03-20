/*
 * @template for Auth
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the template for auth
 */

'use client';

// Import some React's hooks
import { useContext } from 'react';

// Import the Image component
import Image from 'next/image';

// Import the Link component
import Link from 'next/link';

// Import the options for website and member
import {WebsiteOptionsContext} from '@/core/contexts/OptionsContext';

// Export the Template
export default function Template({ children }: { children: React.ReactNode }) {

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    return (
    <>
        <div className="container mx-auto px-4 md:px-6 fc-auth-container">
            <div className="max-w-md mx-auto">
                {(websiteOptions.SignInPageLogo)?(
                    <div className="grid">
                        <div className="col-1 text-center">
                            <Link href={'/'} className="fc-auth-logo">
                                <Image src={websiteOptions.SignInPageLogo} fill={true} sizes="(max-width: 200px)" alt="Member Access" />
                            </Link>
                        </div>
                    </div>                             
                ):''}               
                <div className="grid">
                    <div className="col-1">
                        {children}
                    </div>
                </div>
            </div>
        </div>
    </>);

}