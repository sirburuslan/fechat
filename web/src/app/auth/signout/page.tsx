/*
 * @page Sign Out
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-19
 *
 * This file contains the page sign out use to cancel a session
 */

'use client'

// Import the react hooks
import { useEffect } from 'react';

// Import the router function
import { useRouter  } from 'next/navigation';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Create the Page component
const Page = (): React.JSX.Element => {

    // Get the router
    const router = useRouter();

    useEffect(() => {

        // Redirect to the sign in page
        router.push(process.env.NEXT_PUBLIC_SITE_URL + '/auth/signin');        

    }, []); 

    return <></>;

};

// Export the Page component
export default Page;