/*
 * @page Plans
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-06
 *
 * This file contains the page plans in the user panel
 */

'use client'

// Import the react hooks
import { useContext } from 'react';

// Import the incs
import { getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the Plans List component
import PlansList from '@/core/components/user/plans/PlansList';

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('user', 'user_plans_subscriptions', memberOptions['Language']);

    };

    return (<PlansList title={ getWord('user', 'user_plans_subscriptions', memberOptions['Language']) } />);

}

// Export the page
export default Page;