/*
 * @layout User
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-20
 *
 * This file contains the user layout with the default components for user panel
 */

'use client'

// Import the react hooks
import { useContext } from 'react';

// Import the router function
import { useRouter, usePathname  } from 'next/navigation';

// Import the incs
import { getWord, cancelSession } from '@/core/inc/incIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the SideBar Layout
import SidebarLayout from "@/core/components/user/layouts/SidebarLayout";

// Import the Plans List component
import PlansList from '@/core/components/user/plans/PlansList';

// Import the Gateways List component
import GatewaysList from "@/core/components/user/gateways/GatewaysList";

// Create the user structure
const UserLayout = ({children}: {children: React.ReactNode}): React.JSX.Element => {

    // Get the router
    let router = useRouter();

    // Get the current path name
    let pathname: string = usePathname();

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);

    // Verify if the member is logged in
    if ( (memberOptions.Default === '0') && (parseInt(memberOptions.MemberId) < 1) ) {

        // Cancel the member session
        cancelSession();

    } else if ( (memberOptions.Default === '0') && (parseInt(memberOptions.Role) === 0) ) {

        // Redirect the administrator to the dashboard page
        router.push(process.env.NEXT_PUBLIC_SITE_URL + 'admin/dashboard');

    } else if ( (memberOptions.Default === '0') && (typeof memberOptions.SubscriptionExpiration !== 'undefined') ) {

        // Get expiration time
        let expirationTime = memberOptions.SubscriptionExpiration.split('/');

        // Verify if the subscription is expired
        if ( Date.now() > (new Date(expirationTime[2].padStart(2, '0') + '/' + expirationTime[1].padStart(2, '0') + '/' + expirationTime[0]).getTime() + 86400000) ) {

            // Check if is the gateways page
            if ( pathname.split('/user/gateways/').length > 1 ) {

                // Display the gateways page
                return (<GatewaysList plan={pathname.split('/user/gateways/')[1]} />);

            }

            return (<PlansList title={ getWord('user', 'user_plan_subscription_expired', memberOptions['Language']) } />);

        }

    } else if ( (memberOptions.Default === '0') && (typeof memberOptions.SubscriptionExpiration === 'undefined') ) {

        // Check if is the gateways page
        if ( pathname.split('/user/gateways/').length > 1 ) {

            // Display the gateways page
            return (<GatewaysList plan={pathname.split('/user/gateways/')[1]} />);

        }

        return (<PlansList title={ getWord('user', 'user_our_plans', memberOptions['Language']) } />);

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

// Export the user structure
export default UserLayout;