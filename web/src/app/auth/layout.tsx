/*
 * @layout Auth
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-26
 *
 * This file contains the auth layout
 */

'use client'

// Import some React's hooks
import { useEffect } from 'react';

// Import the router function
import { useRouter, usePathname  } from 'next/navigation';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import styles
import '@/styles/auth/main.scss';

// Create the Layout
const Layout = ({ children }: { children: React.ReactNode }): React.JSX.Element => {

    // Get the router
    const router = useRouter();

    // Get the current path name
    const pathname: string = usePathname();

    // Run the content after component load
    useEffect((): void => {

        // Check if path name is sign out page
        if ( pathname === '/auth/signout' ) {

            // Remove the jwt token
            SecureStorage.removeItem('fc_jwt');

            // Remove the jwt role
            SecureStorage.removeItem('fc_role');

        } else if ( SecureStorage.getItem('fc_jwt')  ) {

            // Check if member is administrator
            if ( SecureStorage.getItem('fc_role') === 0 ) {

                // Redirect the administrator to the dashboard page
                router.push(process.env.NEXT_PUBLIC_SITE_URL + 'admin/dashboard');

            } else {

                // Redirect the user to the dashboard page
                router.push(process.env.NEXT_PUBLIC_SITE_URL + 'user/dashboard');

            }
            
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