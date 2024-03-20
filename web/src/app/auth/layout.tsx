/*
 * @layout Auth
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-12
 *
 * This file contains the auth layout
 */

'use client'

// Import some React's hooks
import { useEffect } from 'react';

// Import the router function
import { useRouter  } from 'next/navigation';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import styles
import '@/styles/auth/main.scss';

// Create the Layout
const Layout = ({ children }: { children: React.ReactNode }): React.JSX.Element => {

    // Get the router
    let router = useRouter();

    // Run the content after component load
    useEffect((): void => {

        // Check if member data exists
        if ( SecureStorage.getItem('fc_jwt')  ) {

            // Remove the jwt token
            SecureStorage.removeItem('fc_jwt');

            // Remove the jwt role
            SecureStorage.removeItem('fc_role');

            // Redirect to home page
            router.push(process.env.NEXT_PUBLIC_SITE_URL as string);
            
        }

    }, []);

    return (
        <>
            <main className="fc-auth-main">
                {children}
            </main>
        </>
    );

};

// Export the Layout
export default Layout;