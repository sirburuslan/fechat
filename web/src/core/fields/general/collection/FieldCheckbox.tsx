/*
 * @component Field Checkbox
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the general checkbox field in the app
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

    // Run after component load
    useEffect((): void => {

        // Check if the checkbox should be checked
        if ( (params.hook.fields[params.name] as number) > 0 ) {

            // Check the checkbox
            (document.querySelector(".fc-settings-option #fc-settings-checkbox-input-" + params.name.toLowerCase()) as HTMLInputElement).checked = true;

        } else {

            // Uncheck the checkbox
            (document.querySelector(".fc-settings-option #fc-settings-checkbox-input-" + params.name.toLowerCase()) as HTMLInputElement).checked = false;

        }

    }, [params.hook.fields[params.name]]);

    /**
     * Change value handler
     */
    const changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-settings-option #fc-settings-checkbox-input-" + params.name.toLowerCase()) as HTMLInputElement).checked?1:0;

        // Update the fields
        params.hook.setFields(params.hook.fields);

        // Show the save changes button
        document.getElementsByClassName('fc-settings-actions')[0].classList.add('fc-settings-actions-show');

    };

    return (
        <li className="fc-settings-option" data-option={ params.name }>
            <div className="grid xl:grid-cols-3">
                <div>
                    <h3>{ params.label }</h3>
                    <p>{ params.description }</p>
                </div>
                <div className="xl:col-span-2">
                    <div className="fc-settings-checkbox">
                        {
                            <input type="checkbox" name={"fc-settings-checkbox-input-" + params.name.toLowerCase()} id={"fc-settings-checkbox-input-" + params.name.toLowerCase()} className="fc-settings-checkbox-input" onChange={(): void => changeValue()} />
                        }
                        <label htmlFor={"fc-settings-checkbox-input-" + params.name.toLowerCase()}>
                            { getIcon('IconRadioButtonUnchecked', {className: 'fc-settings-checkbox-check-icon'}) }
                            { getIcon('IconTaskAlt', {className: 'fc-settings-checkbox-checked-icon'}) }
                            <span className="fc-settings-checkbox-check-text">
                                { params.data.unchecked }
                            </span>
                            <span className="fc-settings-checkbox-checked-text">
                                { params.data.checked }
                            </span>
                        </label>
                    </div>
                    <p className={ (typeof params.hook.errors![params.name] !== 'undefined')?'fc-settings-option-error fc-settings-option-error-show':'fc-settings-option-error' }>{ (typeof params.hook.errors![params.name] !== 'undefined')?params.hook.errors![params.name]:'' }</p>
                </div>
            </div>
        </li>
    );

}

// Export the FieldCheckbox component
export default FieldCheckbox;