/*
 * @component Field Phone
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra phone field in the app
 */

// Import the React hooks
import { useContext } from 'react';

// Import the incs
import { getIcon, getWord, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the FieldPhone component
const FieldPhone = (params: typeField): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);   

    // Generate unique id
    const uniqueId: string = "fc-settings-phone-input-" + params.name;

    /**
     * Change value handler
     */
    const changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-option-number #" + uniqueId) as HTMLInputElement).value;

        // Update the fields
        params.hook.setFields(params.hook.fields);

    };

    /**
     * Copy phone handler
     */
    const copyPhone = (): void => {

        // Verify if field is undefined
        if ( typeof params.hook.fields[params.name] === 'undefined' ) {

            // Show notification
            showNotification('error', getWord('user', 'default_phone_was_not_copied', memberOptions['Language']));
            return;
            
        }

        // Copy the text
        navigator.clipboard.writeText(params.hook.fields[params.name].toString())

        // Process the response
        .then(() => {
            showNotification('success', getWord('default', 'default_phone_was_copied', memberOptions['Language']));
        })

        // Process the error
        .catch(() => {
            showNotification('error', getWord('default', 'default_phone_was_not_copied', memberOptions['Language']));
        });

    };

    return (
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-number">
                <input
                    type="number"
                    placeholder={ params.data.placeholder }
                    defaultValue={ params.hook.fields[params.name] as string }
                    name={uniqueId}
                    id={uniqueId}
                    className="fc-option-number-input"
                    onInput={(): void => changeValue()}
                    autoComplete="new-phone"
                />
                <button type="button" className="fc-option-copy-button" onClick={copyPhone}>
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

// Export the FieldPhone component
export default FieldPhone;