/*
 * @component Field Static List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the general static list field in the app
 */


// Import the UI Components
import { UiDropdown } from '@/core/components/general/ui/UiIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldStaticList component
const FieldStaticList = (params: typeField): React.JSX.Element => {

    // Static list container
    let static_list: Array<{id: string | number, text: string, url: string}> = [];

    // Button container
    let button: string = '';

    // Verify if list exists
    if ( params.data.list!.length > 0 ) {

        // List the items
        for ( let item of params.data.list! ) {

            // Check if the item is selected
            if ( item.itemId === params.hook.fields[params.name] ) {

                // Replace the button
                button = item.itemName as string;

            }

            // Add item to the list container
            static_list.push({
                id: item.itemId,
                text: item.itemName as string,
                url: '#'
            });

        }

    }

    // Dropdown items
    let dropdown_items: Array<{text: string, url: string}> = static_list;

    return (
        <li className="fc-settings-option" data-option={ params.name }>
            <div className="grid xl:grid-cols-3">
                <div>
                    <h3>{ params.label }</h3>
                    <p>{ params.description }</p>
                </div>
                <div className="xl:col-span-2">
                    <div className="fc-settings-dropdown">
                        <UiDropdown button={ button } options={ dropdown_items } id={ "fc-settings-uidropdown-" + params.name.toLowerCase() } />
                    </div>
                    <p className={ (typeof params.hook.errors![params.name] !== 'undefined')?'fc-settings-option-error fc-settings-option-error-show':'fc-settings-option-error' }>{ (typeof params.hook.errors![params.name] !== 'undefined')?params.hook.errors![params.name]:'' }</p>
                </div>
            </div>
        </li>
    );

}

// Export the FieldStaticList component
export default FieldStaticList;