/*
 * @component Field List Manager
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the extra field list manager in the app
 */

// Import the incs
import { getIcon } from '@/core/inc/incIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

// Create the FieldListManager component
const FieldListManager = (params: typeField): React.JSX.Element => {

    return (
        <li>
            <div className="relative fc-option-list-manager">
                <div className="flex justify-between w-full">
                    <h4>{ params.label }</h4>
                    <button type="button" className="fc-option-list-manager-new-item">
                        { getIcon('IconAdd', {className: 'fc-new-icon'}) }
                    </button>
                </div>
                <div className="w-full">
                    <ul></ul>
                </div>                                    
            </div>
        </li>
    );

}

// Export the FieldListManager component
export default FieldListManager;