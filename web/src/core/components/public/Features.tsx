/*
 * @component Features
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-13
 *
 * This file contains the Features component for home page
 */

'use client'

// Use the React hooks
import { useContext } from 'react';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Features component
const Features = (): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    return (
        <div className="fc-features">
            <div className="fc-features-container">
                <div className="w-full">
                    <h2>{ getWord('public', 'public_what_makes_website_different', memberOptions['Language']) }</h2>
                </div>
                <div className="w-full">
                    <ul className="fc-features-list">
                        <li>
                            { getIcon('IconSupportAgent') }
                            <h4>{ getWord('public', 'public_24_7_support', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_get_helps_in_minutes', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconPayments') }
                            <h4>{ getWord('public', 'public_scaled_pricing', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_scaled_pricing_description', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconTranslate') }
                            <h4>{ getWord('public', 'public_multi_language_support', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_multi_language_support_description', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconScheduleSend') }
                            <h4>{ getWord('public', 'public_notifications', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_notifications_description', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconHistory') }
                            <h4>{ getWord('public', 'public_chat_hstory', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_chat_hstory_description', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconLocationOn') }
                            <h4>{ getWord('public', 'public_clients_location', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_clients_location_description', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconCropOriginal') }
                            <h4>{ getWord('public', 'public_attachments', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_attachments_description', memberOptions['Language']) }</p>
                        </li>
                        <li>
                            { getIcon('IconTouchApp') }
                            <h4>{ getWord('public', 'public_easy_to_use', memberOptions['Language']) }</h4>
                            <p>{ getWord('public', 'public_easy_to_use_description', memberOptions['Language']) }</p>
                        </li>
                    </ul>
                </div>                
            </div>
        </div>
    );

};

// Export the Features component
export default Features;