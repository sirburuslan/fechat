/*
 * @component Field Textarea
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the general textarea field in the app
 */

// Import the incs
import { unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldTextarea component
const FieldTextarea = (params: typeField): React.JSX.Element => {

    /**
     * Change value handler
     */
    let changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-settings-option #fc-settings-textarea-" + params.name.toLowerCase()) as HTMLInputElement).value;

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
                    <textarea placeholder={ params.data.placeholder } defaultValue={ unescapeRegexString(params.hook.fields[params.name] as string) } name={"fc-settings-textarea-" + params.name.toLowerCase()} id={"fc-settings-textarea-" + params.name.toLowerCase()} className="fc-settings-textarea" onInput={(): void => changeValue()} />
                    <p className={ (typeof params.hook.errors![params.name] !== 'undefined')?'fc-settings-option-error fc-settings-option-error-show':'fc-settings-option-error' }>{ (typeof params.hook.errors![params.name] !== 'undefined')?params.hook.errors![params.name]:'' }</p>
                </div>
            </div>
        </li>
    );

}

// Export the FieldTextarea component
export default FieldTextarea;