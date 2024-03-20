/*
 * @component Field Email
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra email field in the app
 */

// Import the React hooks
import { useContext } from 'react';

// Import the incs
import { getIcon, getWord, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the FieldEmail component
const FieldEmail = (params: typeField): React.JSX.Element => {

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);   

    // Generate unique id
    let uniqueId: string = "fc-settings-email-input-" + params.name;

    /**
     * Change value handler
     */
    let changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-option-email #" + uniqueId) as HTMLInputElement).value;

        // Update the fields
        params.hook.setFields(params.hook.fields);

    };

    /**
     * Copy email handler
     */
    let copyEmail = (): void => {

        // Verify if field is undefined
        if ( typeof params.hook.fields[params.name] === 'undefined' ) {

            // Show notification
            showNotification('error', getWord('user', 'default_email_was_not_copied', memberOptions['Language']));
            return;
            
        }

        // Copy the text
        navigator.clipboard.writeText(params.hook.fields[params.name].toString())

        // Process the response
        .then(() => {
            showNotification('success', getWord('default', 'default_email_was_copied', memberOptions['Language']));
        })

        // Process the error
        .catch(() => {
            showNotification('error', getWord('default', 'default_email_was_not_copied', memberOptions['Language']));
        });

    };

    return (
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-email">
                <input
                    type="email"
                    placeholder={ params.data.placeholder }
                    defaultValue={ params.hook.fields[params.name] as string }
                    name={uniqueId}
                    id={uniqueId}
                    className="fc-option-email-input"
                    onInput={(): void => changeValue()}
                    autoComplete={uniqueId}
                />
                <button type="button" className="fc-option-copy-button" onClick={copyEmail}>
                    { getIcon('IconContentCopy') }
                </button>
                <label
                    htmlFor={uniqueId}
                >
                    { params.label }
                </label>
            </div>
        </li>

    );

}

// Export the FieldEmail component
export default FieldEmail;