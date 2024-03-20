/*
 * @inc Icons
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains functions to read the app's icons
 */

// Import the icons index with all icons
import icons from '@/core/icons/iconsIndex';

/**
 * Get a icon by icon's name
 * 
 * @param string iconName
 * @param object params?
 * 
 * @returns string with icon
 */
const getIcon = (iconName: string, params?: {[key: string]: string | number}): string => {

    // Verify if icon exists
    if ( icons.hasOwnProperty(iconName) ) {

        // Get icon
        let iconObj: PropertyDescriptor | undefined = Object.getOwnPropertyDescriptor(icons, iconName);

        // Check if iconObj exists and has the value property
        if (iconObj && iconObj.value && typeof iconObj.value === 'function') {

            // Call the getIcon function
            return iconObj.value(params);

        } else {

            // Handle case where iconObj or getIcon is not valid
            return '';
            
        }

    } else {

        return '';

    }

}

export default getIcon;