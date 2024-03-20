/*
 * @component Stats
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This file contains the Stats component for home page
 */

'use client'

// Use the React hooks
import { useContext } from 'react';

// Import Link from Next
import Link from 'next/link';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Stats component
const Stats = (): React.JSX.Element => {

    // Website options
    let {websiteOptions} = useContext(WebsiteOptionsContext);

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);

    return (
        <div className="fc-stats">
            <div className="fc-stats-container">
                <div className="fc-stat">
                    <Link href={(websiteOptions.DemoVideo !== '')?websiteOptions.DemoVideo:'#'}>
                        { getIcon('IconBiPlayCircleFill') }
                        { getWord('public', 'public_watch_demo', memberOptions['Language']) }
                    </Link>
                </div>                
                <div className="fc-stat">
                    <h3>1,000,000 +</h3>
                    <h6>{ getWord('public', 'public_connected_websites', memberOptions['Language']) }</h6>
                </div>
                <div className="fc-stat">
                    <h3>5,000,000 +</h3>
                    <h6>{ getWord('public', 'public_open_threads', memberOptions['Language']) }</h6>
                </div>   
                <div className="fc-stat">
                    <h3>5,000 +</h3>
                    <h6>{ getWord('public', 'public_happy_users', memberOptions['Language']) }</h6>
                </div>                
            </div>
        </div>
    );

};

// Export the Stats component
export default Stats;