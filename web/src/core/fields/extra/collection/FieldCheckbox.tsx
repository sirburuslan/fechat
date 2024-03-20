/*
 * @component Field Checkbox
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra checkbox field in the app
 */

'use client'

// Import react hooks
import { useEffect } from 'react';

// Import incs
import { getIcon } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldCheckbox component
const FieldCheckbox = (params: typeField): React.JSX.Element => {

    // Verify if document exists
    if ( typeof document !== 'undefined' ) {

        // Get input
        let input: Element | null = document.querySelector(".fc-extra-option #fc-option-checkbox-input-" + params.name.toLowerCase());

        // check if input exists
        if ( input instanceof HTMLInputElement ) {

            // Check if the checkbox should be checked
            if ( (params.hook.fields[params.name] as number) > 0 ) {

                // Check the checkbox
                input.checked = true;

            } else {

                // Uncheck the checkbox
                input.checked = false;

            }

        }

    }

    /**
     * Change value handler
     */
    let changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-extra-option #fc-option-checkbox-input-" + params.name.toLowerCase()) as HTMLInputElement).checked?1:0;

        // Update the fields
        params.hook.setFields(params.hook.fields);

    };

    return (
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-checkbox">
                {
                    <input type="checkbox" name={"fc-option-checkbox-input-" + params.name.toLowerCase()} id={"fc-option-checkbox-input-" + params.name.toLowerCase()} className="fc-option-checkbox-input" onChange={(): void => changeValue()} />
                }
                <label htmlFor={"fc-option-checkbox-input-" + params.name.toLowerCase()}>
                    { getIcon('IconRadioButtonUnchecked', {className: 'fc-option-checkbox-check-icon'}) }
                    { getIcon('IconTaskAlt', {className: 'fc-option-checkbox-checked-icon'}) }
                    <span className="fc-option-checkbox-check-text">
                        { params.data.unchecked }
                    </span>
                    <span className="fc-option-checkbox-checked-text">
                        { params.data.checked }
                    </span>
                </label>
            </div>
        </li>
    );

}

// Export the FieldCheckbox component
export default FieldCheckbox;