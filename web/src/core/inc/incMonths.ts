/*
 * @inc Months
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the functions to work with the months
 */

// Import the words
import { getWord } from '@/core/inc/incIndex';

/**
 * Get the month text
 * 
 * @param string month 
 * @param string customLang
 * @returns 
 */
const getMonth = (month: string, customLang?: string): string => {

    // Months list
    let months: {[key: string]: string} = {
        '01': getWord('default', 'default_jan', customLang),
        '02': getWord('default', 'default_feb', customLang),
        '03': getWord('default', 'default_mar', customLang),
        '04': getWord('default', 'default_apr', customLang),
        '05': getWord('default', 'default_may', customLang),
        '06': getWord('default', 'default_may', customLang),
        '07': getWord('default', 'default_jul', customLang),
        '08': getWord('default', 'default_aug', customLang),
        '09': getWord('default', 'default_sep', customLang),
        '10': getWord('default', 'default_oct', customLang),
        '11': getWord('default', 'default_nov', customLang),
        '12': getWord('default', 'default_dec', customLang)    
    };

    return months[month];

}

// Export the functions
export default getMonth;