/*
 * @layout Administrator
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the layout for the administrator panel
 */

'use client'

// Import the Admin Layout
import AdminLayout from '@/core/components/admin/layout/AdminLayout';

// Import styles
import '@/styles/admin/_main.scss';

// Create layout for the administrator panel
const Layout = ({children}: {children: React.ReactNode}): React.JSX.Element => {

    return (
        <main className="flex fc-admin-main">
            <AdminLayout>
                {children}
            </AdminLayout>         
        </main>
    );

};

// Export the layout
export default Layout;