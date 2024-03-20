/*
 * @component Field Dynamic List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra field dynamic list in the app
 */

// Import the UI Components
import { UiDynamicDropdown } from '@/core/components/general/ui/UiIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldDynamicList component
const FieldDynamicList = (params: typeField): React.JSX.Element => {

    // Dynamic list container
    let dynamic_list: Array<{id: string | number, text: string, url: string}> = [];

    // Button container
    let button: string = params.label;

    // Dropdown items
    let dropdown_items: Array<{text: string, url: string}> = dynamic_list;

    return (
        <li className="fc-extra-option" data-option={ params.name }>
            <div className="relative fc-option-dropdown">
                <UiDynamicDropdown button={button} placeholder={params.placeholder as string} options={ dropdown_items } id={'fc-option-dropdown-' + params.name.toLowerCase()} />
            </div>
        </li>
    );

}

// Export the FieldDynamicList component
export default FieldDynamicList;