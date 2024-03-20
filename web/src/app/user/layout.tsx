/*
 * @layout for User
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the layout for the user's panel
 */

'use client'

// Import the User Layout
import UserLayout from '@/core/components/user/layouts/UserLayout';

// Import styles
import '@/styles/user/_main.scss';

// Create layout for the user panel
const Layout = ({children}: {children: React.ReactNode}): React.JSX.Element => {

    return (
        <main className="flex fc-user-main">
            <UserLayout>
                {children}
            </UserLayout>       
        </main>
    );

};

// Export the layout
export default Layout;