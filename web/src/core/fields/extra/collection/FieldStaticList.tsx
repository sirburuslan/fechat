/*
 * @component Field Static List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra static list field in the app
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
    let button: string = params.label;

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
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-dropdown">
                <UiDropdown button={button} options={ dropdown_items } id={'fc-option-dropdown-' + params.name.toLowerCase()} />
            </div>
        </li>
    );

}

// Export the FieldStaticList component
export default FieldStaticList;