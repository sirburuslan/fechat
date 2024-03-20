/*
 * @component Faq
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-13
 *
 * This file contains the Faq component for home page
 */

'use client'

// Use the React hooks
import { useContext } from 'react';

// Import Link from Next
import Link from 'next/link';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Faq component
const Faq = (): React.JSX.Element => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    /**
     * Show or hide an answer
     * 
     * @param Event e 
     */
    let showHideAnswer = (e: React.MouseEvent): void => {
        e.preventDefault();

        // Verify if answer is showed
        if (e.currentTarget.getAttribute('aria-expanded') === 'false') {
            e.currentTarget.setAttribute('aria-expanded', 'true');
        } else {
            e.currentTarget.setAttribute('aria-expanded', 'false');
        }

    };

    return (
        <div className="fc-faq">
            <div className="fc-faq-container">
                <div className="w-full">
                    <h2>{ getWord('public', 'public_questions_answers', memberOptions['Language']) }</h2>
                </div>
                <div className="w-full">
                    <ul>
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_how_to_create_chat', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_how_to_create_chat_answer', memberOptions['Language']) }</p>
                            </div>
                        </li>
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_could_i_create_multiple_chats', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_could_i_create_multiple_chats_answer', memberOptions['Language']) }</p>
                            </div>                            
                        </li>
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_to_receive_notifications', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_to_receive_notifications_answer', memberOptions['Language']) }</p>
                            </div>
                        </li>
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_could_i_add_my_team', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_could_i_add_my_team_answer', memberOptions['Language']) }</p>
                            </div>                            
                        </li>                                                
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_how_to_delete_account', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_how_to_delete_account_answer', memberOptions['Language']) }</p>
                            </div>                            
                        </li>
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_how_to_get_support', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_how_to_get_support_answer', memberOptions['Language']) }</p>
                            </div>                            
                        </li>
                        <li>
                            <Link href="#" aria-expanded="false" onClick={showHideAnswer}>
                                <span>
                                    { getWord('public', 'public_do_you_offer_refund', memberOptions['Language']) }
                                </span>
                                { getIcon('IconExpandMore') }
                            </Link>
                            <div className="fc-faq-answer">
                                <p>{ getWord('public', 'public_do_you_offer_refund_answer', memberOptions['Language']) }</p>
                            </div>                              
                        </li>                        
                    </ul>
                </div>                                            
            </div>
        </div>
    );

};

// Export the Faq component
export default Faq;