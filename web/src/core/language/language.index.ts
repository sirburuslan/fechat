/*
 * @file Language Index
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file groups the words for easier usage
 */

// Import the language files
import * as english_auth from '@/core/language/english/languageAuth';
import * as english_admin from '@/core/language/english/languageAdmin';
import * as english_user from '@/core/language/english/languageUser';
import * as english_default from '@/core/language/english/languageDefault';
import * as english_errors from '@/core/language/english/languageErrors';
import * as english_public from '@/core/language/english/languagePublic';
import * as romana_auth from '@/core/language/romana/languageAuth';
import * as romana_admin from '@/core/language/romana/languageAdmin';
import * as romana_user from '@/core/language/romana/languageUser';
import * as romana_default from '@/core/language/romana/languageDefault';
import * as romana_errors from '@/core/language/romana/languageErrors';
import * as romana_public from '@/core/language/romana/languagePublic';

// Create an object with languages
const Languages: {[key: string]: {[key: string]: object}} = {
    english: {
        auth: english_auth,
        admin: english_admin,
        user: english_user,
        default: english_default,
        errors: english_errors,
        public: english_public
    },
    romana: {
        auth: romana_auth,
        admin: romana_admin,
        user: romana_user,
        default: romana_default,
        errors: romana_errors,
        public: romana_public
    }   
}

// Export the languages
export default Languages;