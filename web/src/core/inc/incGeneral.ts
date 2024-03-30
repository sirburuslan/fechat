/*
 * @inc General
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-12
 *
 * This file contains general functions
 */

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getWord, getIcon } from '@/core/inc/incIndex';

// Import the types
import { typeList } from '@/core/types/typesIndex';

/**
 * Calculate time from timestamp
 * 
 * @param number from
 * @param number to
 * @param string lang
 * 
 * @returns string with time
 */
const calculateTime = ( from: number, to: number, lang: string = '' ): string => {

    // Set calculation time
    let calculate: number = to - from;

    // Set after variable
    let after: string = '';

    // Set before variable 
    let before: string = ' ' + getWord('default', 'default_time_ago', lang);

    // Define calc variable
    let calc: number;

    // Verify if time is older than now
    if ( calculate < 0 ) {

        // Set absolute value of a calculated time
        calculate = Math.abs(calculate);

        // Set icon
        after = '';

        // Empty before
        before = '';

    }

    // Calculate time
    if ( calculate < 60 ) {

        // Return just now
        return after + getWord('default', 'default_just_now', lang);

    } else if ( calculate < 3600 ) {

        // Divide to 60
        calc = calculate / 60;

        // Round the number
        calc = Math.round(calc);

        // Return time in minutes
        return after + calc + ' ' + getWord('default', 'default_minutes', lang) + before;

    } else if ( calculate < 86400 ) {

        // Divide to hours
        calc = calculate / 3600;

        // Round the number
        calc = Math.round(calc);

        // Return time in hours
        return after + calc + ' ' + getWord('default', 'default_hours', lang) + before;

    } else if ( calculate >= 86400 ) {

        // Divide time in days
        calc = calculate / 86400;

        // Round the number
        calc = Math.round(calc);

        // Return time in days
        return after + calc + ' '+ getWord('default', 'default_days', lang) + before;

    } else {

        // Return unknown
        return getWord('default', 'default_unknown', lang)
        
    }

};

/**
 * Cancel a member session
 */
const cancelSession = (): void => {

    // Check if jwt token exists
    if ( SecureStorage.getItem('fc_jwt') ) {

        // Remove the jwt token
        SecureStorage.removeItem('fc_jwt');

        // Remove the jwt role
        SecureStorage.removeItem('fc_role');

    }

    // Redirect the user to session expired
    document.location.href = process.env.NEXT_PUBLIC_SITE_URL + '/errors/session-expired';

}

/**
 * Get all currencies
 * 
 * @returns array with available currencies
 */
const getCurrencies = (): typeList[] => {

    return [{
        itemId: 'USD',
        itemName: 'USD'
    }, {
        itemId: 'EUR',
        itemName: 'EUR'
    }, {
        itemId: 'JPY',
        itemName: 'JPY'
    }, {
        itemId: 'GBP',
        itemName: 'GBP'
    }, {
        itemId: 'AUD',
        itemName: 'AUD'
    }, {
        itemId: 'CAD',
        itemName: 'CAD'
    }, {
        itemId: 'CHF',
        itemName: 'CHF'
    }];

}

/**
 * Unescape regex string
 * 
 * @param string text 
 * 
 * @returns string
 */
const unescapeRegexString = (text: string): string => {

    // Check if text is number
    if ( !isNaN(Number(text)) || (typeof text !== 'string') ) {
        return text;
    }

    // Replace \\\
    text = text.replaceAll("\\\\", "");
    
    return text.replaceAll(/\\(u[0-9A-Fa-f]{4})/g, (match: string, group: string): string => {

        return String.fromCharCode(parseInt(group.slice(1), 16));

    }).replaceAll(/\\(.)/g, (match: string, group: string): string => {

        switch (group) {
            case 'n': return '\n';
            case 'r': return '\r';
            default: return group;
        }
        
    });
    
}

// Return the functions
export {
    calculateTime,
    cancelSession,
    getCurrencies,
    unescapeRegexString
};