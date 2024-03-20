/*
 * @component Ui Modal
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component shows the ui modal in the app
 */

'use client'

// Import the React's hooks
import { ReactElement, SetStateAction, Dispatch } from 'react';

// Import the incs
import { getIcon } from '@/core/inc/incIndex';

// Create the Modal
const UiModal = (props: {size?: string, title: string, status: string, updateStatus: Dispatch<SetStateAction<string>>, children: ReactElement}): React.JSX.Element => {

    // Hide modal handler
    let hideModal = (): void => {

        // Start to hide modal
        props.updateStatus('fc-modal-hide');

        // Set pause
        setTimeout((): void => {

            // Hide modal
            props.updateStatus('');

        }, 200);

    };

    return (
        <div className={`fc-modal ${props.size} ${props.status}`} aria-labelledby="modal-title" role="dialog" aria-modal="true">
            <div className="fixed inset-0 bg-black bg-opacity-50 transition-opacity fc-modal-cover" onClick={hideModal}></div>
            <div className="fc-modal-container">
                <div className="fc-modal-header flex justify-between">
                    <h3>
                        {props.title}
                    </h3>
                    <button type="button" className="fc-modal-hide-modal" onClick={hideModal}>
                        {getIcon('IconClose')}
                    </button>
                </div>
                <div className="fc-modal-body">
                    {props.children}           
                </div>
            </div>
        </div>
    );

}

// Export the modal
export default UiModal;