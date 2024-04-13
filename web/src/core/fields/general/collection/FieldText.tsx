/*
 * @component Field Text
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the general text field in the app
 */

// Import the incs
import { unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldText component
const FieldText = (params: typeField): React.JSX.Element => {

    /**
     * Change value handler
     */
    const changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-settings-option #fc-settings-text-input-" + params.name.toLowerCase()) as HTMLInputElement).value;

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
                    <input type="text" placeholder={ params.data.placeholder } defaultValue={ unescapeRegexString(params.hook.fields[params.name] as string) } name={"fc-settings-text-input-" + params.name.toLowerCase()} id={"fc-settings-text-input-" + params.name.toLowerCase()} className="fc-settings-text-input" onInput={(): void => changeValue()} />
                    <p className={ (typeof params.hook.errors![params.name] !== 'undefined')?'fc-settings-option-error fc-settings-option-error-show':'fc-settings-option-error' }>{ (typeof params.hook.errors![params.name] !== 'undefined')?params.hook.errors![params.name]:'' }</p>
                </div>
            </div>
        </li>
    );

}

// Export the FieldText component
export default FieldText;