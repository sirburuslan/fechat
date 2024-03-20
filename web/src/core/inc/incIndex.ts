/*
 * @file Inc Index
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file groups the incs files for easier usage
 */

// Import the inc
import getMonth from "@/core/inc/incMonths";
import getIcon from "@/core/inc/incIcons";
import { getWord, getLanguages } from "@/core/inc/incWords";
import getField from "@/core/inc/incFields";
import getToken from "@/core/inc/incTokens";
import { updateOptions, getOptions, getMemberOptions, getWebsiteOptions } from "@/core/inc/incOptions";
import showNotification from "@/core/inc/incNotification";
import { calculateTime, cancelSession, getCurrencies, unescapeRegexString } from "@/core/inc/incGeneral";

// Export the inc
export {
    getMonth,
    getIcon,
    getWord,
    getLanguages,
    getField,
    getToken,
    updateOptions,
    getOptions,
    getMemberOptions,
    getWebsiteOptions,
    showNotification,
    calculateTime,
    cancelSession,
    getCurrencies,
    unescapeRegexString
}