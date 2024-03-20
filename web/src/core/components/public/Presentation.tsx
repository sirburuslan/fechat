/*
 * @component Presentation
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-16
 *
 * This file contains the Presentation component for home page
 */

'use client'

// Use the React hooks
import { SyntheticEvent, useContext } from 'react';

// Import Link from Next
import Link from 'next/link';

// Import Image from Next
import Image from 'next/image';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Presentation component
const Presentation = (): React.JSX.Element => {

    // Website options
    let {websiteOptions} = useContext(WebsiteOptionsContext);

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);

    return (
        <div className="fc-presentation">
            <div className="fc-presentation-container">
                <div className="fc-presentation-text">
                    <h1>{ getWord('public', 'public_start_now_to_offer', memberOptions['Language']) } <span>{ getWord('public', 'public_real_time', memberOptions['Language']) }</span><br /> { getWord('public', 'public_support_to_your_clients', memberOptions['Language']) }</h1>
                    <h4>{ getWord('public', 'public_add_live_chat_to_your_website', memberOptions['Language']) }<br /> { getWord('public', 'public_use_it_with_no_restrictions', memberOptions['Language']) }</h4>
                    {(websiteOptions.RegistrationEnabled === '1')?(
                        <Link href="/auth/registration" className="fc-get-started">
                            { getWord('public', 'public_get_started_free', memberOptions['Language']) }
                        </Link>
                    ):''}
                    <Link href="#" className="fc-book-call">
                        { getWord('public', 'public_book_a_call_now', memberOptions['Language']) }
                    </Link>
                    <h6>
                        <span>
                            { getIcon('IconTour') }
                            { getWord('public', 'public_set_up_in_minutes', memberOptions['Language']) }
                        </span>
                        <span>
                            { getIcon('IconTour') }
                            { getWord('public', 'public_no_credit_card_required', memberOptions['Language']) }
                        </span>   
                    </h6>                   
                </div>
                <div className="fc-presentation-image">
                    <Image src={'/assets/img/presentation.png'} width={500} height={400} priority={true} alt="Presentation" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                </div>
            </div>
        </div>
    );

};

// Export the Presentation
export default Presentation;