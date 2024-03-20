/*
 * @layout Admin
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-06
 *
 * This file contains the admin layout with the default components for admin panel
 */

'use client'

// Import the react hooks
import { useContext } from 'react';

// Import the router function
import { useRouter } from 'next/navigation';

// Import the incs
import { cancelSession } from '@/core/inc/incIndex';

// Import the options for website and member
import {WebsiteOptionsContext, MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the SideBar Layout
import SidebarLayout from "@/core/components/admin/layout/SidebarLayout";

// Create the admin structure
const AdminLayout = ({children}: {children: React.ReactNode}): React.JSX.Element => {

    // Get the router
    let router = useRouter();

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Verify if the member is logged in
    if ( (memberOptions.Default === '0') && (parseInt(memberOptions.MemberId) < 1) ) {

        // Cancel the member session
        cancelSession();

    } else if ( (memberOptions.Default === '0') && (parseInt(memberOptions.Role) === 1) ) {

        // Redirect the user to the dashboard page
        router.push(process.env.NEXT_PUBLIC_SITE_URL + 'user/dashboard');

    }

    return (
        <>            
            <SidebarLayout />
            <div className="fc-dashboard-content">
                {children}
            </div>
        </>
    );

}

// Export the admin structure
export default AdminLayout;