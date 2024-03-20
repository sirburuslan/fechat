/*
 * @inc Words
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains functions to works with the text in the app
 */

// Import the files with all words
import words from "@/core/language/language.index";

// Import types
import { typeList } from '@/core/types/typesIndex';

/**
 * Get a word by word's name
 * 
 * @param string section
 * @param string wordName
 * @param string customLang
 * 
 * @returns string with word or empty space
 */
const getWord = (section: string, wordName: string, customLang?: string): string => {

    // Default language if none is selected
    let language: string | undefined = (customLang)?customLang:process.env.NEXT_PUBLIC_LANGUAGE;

    // Verify if language exists
    if ( typeof language === 'undefined' ) {
        return '';
    }

    // Get language
    let lang: {[key: string]: object} = words[language];

    // Get the word
    let word: {[key: string]: string} = (lang[section] as {default(): { [key: string]: string;}} ).default();

    // Check if word name exists
    if ( typeof word[wordName] !== 'string' ) {
        return '';
    }

    return word[wordName];

}


/**
 * Get all languages
 * 
 * @param string language
 * 
 * @returns array with available languages
 */
const getLanguages = (language?: string): typeList[] => {

    return [{
        itemId: 'english',
        itemName: getWord('default', 'default_english', language)
    }, {
        itemId: 'romana',
        itemName: getWord('default', 'default_romana', language)
    }];

}

export {
    getWord,
    getLanguages
};