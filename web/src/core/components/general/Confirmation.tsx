/*
 * @component Confirmation
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-09
 *
 * The component shows a modal with confirmation button to execute an action
 */

'use client'

// Import the react hooks
import { ReactElement, useEffect, useContext, useState  } from 'react';

// Import the incs
import { getIcon, getWord, showNotification } from '@/core/inc/incIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiModal } from '@/core/components/general/ui/UiIndex';

// Create the confirmation component
const Confirmation = (props: {confirmAction: (id: string) => void, children: ReactElement}): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);  

    // Modal status
    const [modalStatus, setModalStatus] = useState('');

    // Confirmation button scope
    const [buttonScope, setButtonScope] = useState('');

    /**
     * Track any click
     * 
     * @param Event e
     */
    const trackClicks = async (e: Event): Promise<void> => {

        // Get the target
        const target = e.target;

        // Check if is clicked the confirmation button
        if ( (target instanceof Element) && (target.classList !== undefined) && target.classList.contains('fc-confirmation-modal-button') ) {
            e.preventDefault();

            // Show the modal
            setModalStatus('fc-modal-show');

            // Verify if data id exists
            if ( target.getAttribute('data-id') ) {

                // Add data id to the confirmation button
                setButtonScope(target.getAttribute('data-id') as string);

            }

        }

    };

    // Track the confirmation button click
    const buttonClick = (e: React.MouseEvent<HTMLButtonElement>): void => {
        e.preventDefault();

        // Verify if button has data id
        if ( e.currentTarget.getAttribute('data-id') ) {

            // Run the action
            props.confirmAction(e.currentTarget.getAttribute('data-id') as string);

            // Hide the modal
            setModalStatus('');

        } else {

            // Show error notification
            showNotification('error', getWord('default', 'error_confirmation_button_not_configured', memberOptions['Language']));

        }

    }

    // Run code for client
    useEffect((): () => void => {

        // Register an event for clicks tracking
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Remove event for clicks tracking
            document.removeEventListener('click', trackClicks);

        }

    });

    return (
        <UiModal size="fc-modal-md" title={getWord('default', 'default_are_you_sure', memberOptions['Language'])} status={modalStatus} updateStatus={setModalStatus}>
            <>
                <div className="col-span-full">
                    <p className="fc-modal-text">{props.children}</p>
                </div> 
                <div className="col-span-full fc-modal-button">
                    <div className="text-right">
                        <button type="button" className="mb-3 flex justify-between fc-option-red-btn fc-confirmation-button" data-id={buttonScope} onClick={buttonClick}>
                            { getIcon('IconFileDownloadDone', {className: 'fc-next-icon'}) }
                            { getWord('default', 'default_confirm', memberOptions['Language']) }
                        </button>
                    </div>
                </div>             
            </>
        </UiModal>
    );

}

// Export the confirmation component
export default Confirmation;