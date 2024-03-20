/*
 * @type Field
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the type for the app's fields
 */

// Import the React features
import { Dispatch, SetStateAction } from 'react';

// Type for list
type typeList = {
    itemId: number | string,
    itemName: number | string
};

// Type for fields
type typeField = {
    name: string,
    label: string,
    description?: string,
    placeholder?: string,
    data: {
        checked?: string,
        unchecked?: string,
        placeholder?: string,
        value?: number | string,
        list?: typeList[]
    },
    hook: {
        fields: {[key: string]: string | number},
        setFields: Dispatch<SetStateAction<any>>,
        errors?: {[key: string]: string},
        setErrors?: Dispatch<SetStateAction<any>>,        
    }
};

// Export the type
export type {
    typeList,
    typeField
};