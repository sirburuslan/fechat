/*
 * @component Field Password
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra password field in the app
 */

'use client'

// Import some React's hooks
import { useState, MouseEventHandler } from 'react';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldPassword component
const FieldPassword = (params: typeField): React.JSX.Element => {

    // Define the state of the password input
    const [passwordInputType, setPasswordInputType] = useState('password');

    // Generate unique id
    const uniqueId: string = "fc-settings-password-input-" + params.name;

    /**
     * Change value handler
     */
    const changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-option-password #" + uniqueId) as HTMLInputElement).value;

        // Update the fields
        params.hook.setFields(params.hook.fields);

    };

    // Detect when the password input type should be changed
    const handleChangePasswordInput: MouseEventHandler<HTMLButtonElement> = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {

        // Get target
        const target = e.target as Element;

        // Check if the password input has the text type
        if (passwordInputType === 'text') {

            // Set type as password
            setPasswordInputType('password');

            // Remove class fc-option-password-show-password-active-btn
            target.classList.remove('fc-option-password-show-password-active-btn');

        } else {

            // Set type as text
            setPasswordInputType('text');

            // Add class fc-option-password-show-password-active-btn
            target.classList.add('fc-option-password-show-password-active-btn');

        }        

    };

    return (
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-password">
                <input
                    type={passwordInputType}
                    placeholder={ params.data.placeholder }
                    defaultValue={ params.hook.fields[params.name] as string }
                    name={uniqueId}
                    id={uniqueId}
                    className="fc-option-password-input"
                    onInput={(): void => changeValue()}
                    autoComplete={uniqueId}
                />
                <button type="button" className="fc-option-show-password-button" onClick={handleChangePasswordInput}>
                    <span className="material-icons-outlined fc-option-password-eye-icon">visibility</span>
                    <span className="material-icons-outlined fc-option-password-eye-hide-icon">visibility_off</span>
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

// Export the FieldPassword component
export default FieldPassword;