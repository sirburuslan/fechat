/*
 * @component Field Textarea
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra textarea field in the app
 */

// Import the React hooks
import { useContext } from 'react';

// Import the incs
import { getIcon, getWord, showNotification, unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the FieldTextarea component
const FieldTextarea = (params: typeField): React.JSX.Element => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);  

    // Generate unique id
    let uniqueId: string = "fc-settings-textarea-input-" + params.name;

    /**
     * Change value handler
     */
    let changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-option-textarea #" + uniqueId) as HTMLInputElement).value;

        // Update the fields
        params.hook.setFields(params.hook.fields);

    };

    /**
     * Copy textarea vaue handler
     */
    let copyValue = (): void => {

        // Verify if field is undefined
        if ( typeof params.hook.fields[params.name] === 'undefined' ) {

            // Show notification
            showNotification('user', getWord('user', 'user_value_was_not_copied', memberOptions['Language']));
            return;
            
        }

        // Copy the text
        navigator.clipboard.writeText(params.hook.fields[params.name].toString())

        // Process the response
        .then(() => {
            showNotification('success', getWord('user', 'user_value_was_copied', memberOptions['Language']));
        })

        // Process the error
        .catch(() => {
            showNotification('user', getWord('user', 'user_value_was_not_copied', memberOptions['Language']));
        });

    };

    return (
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-textarea">
                <textarea 
                    placeholder={ params.data.placeholder }
                    defaultValue={ unescapeRegexString(params.hook.fields[params.name] as string) }
                    name={uniqueId}
                    id={uniqueId}
                    className="fc-option-textarea-input"
                    onChange={(): void => changeValue()}
                    onInput={(): void => changeValue()}
                    autoComplete={uniqueId}
                />
                <button type="button" className="fc-option-copy-button" onClick={copyValue}>
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

// Export the FieldTextarea component
export default FieldTextarea;