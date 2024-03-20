/*
 * @inc Fields
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains functions to read the app's fields
 */

// Import the general fields index with all fields
import generalFields from '@/core/fields/general/fieldsIndex';

// Import the extra fields index with all fields
import extraFields from '@/core/fields/extra/fieldsIndex';

// Import types
import { typeField } from '@/core/types/typesIndex';

/**
 * Get a field by field's name
 * 
 * @param string fieldType
 * @param string fieldName
 * @param object params?
 * 
 * @returns string with field
 */
const getField = (fieldType: string, fieldName: string, params: typeField): string => {

    // Check if field type is general
    if ( fieldType === 'general' ) {

        // Verify if field exists
        if ( generalFields.hasOwnProperty(fieldName) ) {

            // Get field
            let fieldObj: PropertyDescriptor | undefined = Object.getOwnPropertyDescriptor(generalFields, fieldName);

            // Check if fieldObj exists and has the value property
            if (fieldObj && fieldObj.value && typeof fieldObj.value === 'function') {

                // Call the getfield function
                return fieldObj.value(params);

            } else {

                // Handle case where fieldObj or getfield is not valid
                return '';
                
            }

        } else {

            return '';

        }

    } else {

        // Verify if field exists
        if ( extraFields.hasOwnProperty(fieldName) ) {

            // Get field
            let fieldObj: PropertyDescriptor | undefined = Object.getOwnPropertyDescriptor(extraFields, fieldName);

            // Check if fieldObj exists and has the value property
            if (fieldObj && fieldObj.value && typeof fieldObj.value === 'function') {

                // Call the getfield function
                return fieldObj.value(params);

            } else {

                // Handle case where fieldObj or getfield is not valid
                return '';
                
            }

        } else {

            return '';

        }

    }

}

// Return the field
export default getField;