// Import the React module
import React from 'react';

// Import Metadata function from Next
import { Metadata } from 'next';

// Import the Next Link
import Link from 'next/link';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Set the page title
export const metadata: Metadata = {
    title: getWord('errors', 'error_errors_session_expired')
};

// Create the Session Expired component
const Page = (): React.JSX.Element => {

    return (
        <div className="fc-errors-container flex items-center justify-center">
            <div className="fc-errors-text">
                <h2>{ getWord('errors', 'error_your_session_expired') }</h2>
                <Link href={process.env.NEXT_PUBLIC_SITE_URL as unknown as URL} className="flex justify-between">
                    { getWord('errors', 'error_go_home') }  { getIcon('IconArrowRightAlt', {className: 'fc-errors-homepage-icon'}) }
                </Link>
            </div>
        </div>
    );

}

// Export the Session Expired component
export default Page;