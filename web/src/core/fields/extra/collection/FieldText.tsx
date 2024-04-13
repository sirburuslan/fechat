/*
 * @component Field Text
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra text field in the app
 */

'use client'

// Import the React hooks
import { useEffect } from 'react';

// Import the incs
import { unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldText component
const FieldText = (params: typeField): React.JSX.Element => {

    // Generate unique id
    const uniqueId: string = "fc-settings-text-input-" + params.name.toLowerCase();

    /**
     * Change value handler
     */
    const changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-option-text #" + uniqueId) as HTMLInputElement).value;

        // Update the fields
        params.hook.setFields(params.hook.fields);

    };

    // Check if default is 0
    if ( params.hook.fields.Default === 0 && document.querySelector(".fc-option-text #" + uniqueId) ) {

        // Replace default value
        (document.querySelector(".fc-option-text #" + uniqueId) as HTMLInputElement).value = params.hook.fields[params.name] as string;

    }

    return (
        <li className="fc-extra-option" data-option={ params.name.toLowerCase() }>
            <div className="relative fc-option-text">
                <input
                    type="text"
                    placeholder={ params.data.placeholder }
                    defaultValue={ unescapeRegexString(params.hook.fields[params.name] as string) }
                    name={uniqueId}
                    id={uniqueId}
                    className="fc-option-text-input"
                    onInput={(): void => changeValue()}
                    autoComplete={uniqueId}
                />
                <label
                    htmlFor={uniqueId}
                >
                    { params.label }
                </label>
            </div>
        </li>

    );

}

// Export the FieldText component
export default FieldText;