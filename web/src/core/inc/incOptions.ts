/*
 * @inc Options
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-16
 *
 * This file contains functions to manage the options
 */

'use client'

// Import functions from react
import { Dispatch, SetStateAction } from 'react';

// Import axios module
import axios, { AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the types
import { typeOptions } from "@/core/types/typesIndex";

/**
 * Update the default options in the page
 * 
 * @param object optionsList
 */
const updateOptions = (optionsList: {success: boolean, options?: typeOptions}, setWebsiteOptions: Dispatch<SetStateAction<{[key: string]: string;}>>, setMemberOptions: Dispatch<SetStateAction<{[key: string]: string;}>>) => {

    // Check if website options exists
    if ( (typeof optionsList.options !== 'undefined') && optionsList.options.website && Object.keys(optionsList.options.website).length > 0 ) {

        // Add website options
        setWebsiteOptions(optionsList.options.website);

    }
    
    // Check if member options exists
    if ( (typeof optionsList.options !== 'undefined') && optionsList.options.member && Object.keys(optionsList.options.member).length > 0 ) {

        // Add member options
        setMemberOptions(optionsList.options.member);

    }

}

/**
 * Get all options
 * 
 * @returns optionsType with options
 */
const getAllOptions = async (): Promise<typeOptions> => {

    // Return a promise
    return new Promise(async (resolve, reject) => {

        try {

            // Set the headers
            let headers = {};  

            // Set the bearer token
            let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');    

            // Verify if token exists
            if ( token !== null ) {
                
                // Set token
                headers = {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                }

            }

            // Get the options for website and member
            let optionsResponse: AxiosResponse = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/settings', headers); 

            // Check if response is successfully
            if ( optionsResponse.data.success ) {

                // Options list
                let optionsList: typeOptions = {
                    website: getWebsiteOptions(),
                    member: getMemberOptions()
                };

                // Update the website default mark
                optionsList.website = Object.assign({}, optionsList.website, {Default: '0'});

                // Verify if options exists
                if ( typeof optionsResponse.data.options !== 'undefined' ) {

                    // Set the website options
                    optionsList.website = Object.assign({}, optionsList.website, optionsResponse.data.options);

                }

                // Update the member default mark
                optionsList.member = Object.assign({}, optionsList.member, {Default: '0'});

                // Verify if member options exists
                if ( typeof optionsResponse.data.memberOptions !== 'undefined' ) {

                    // Set the member options
                    optionsList.member = Object.assign({}, optionsList.member, optionsResponse.data.memberOptions);

                }                

                // Return error
                resolve(optionsList);

            } else {

                // Throw error notification
                throw new Error(optionsResponse.data.message);

            }

        } catch (error: unknown) {

            // Return error
            reject(error);

        }

    });

}

/**
 * Get all options
 * 
 * @returns object with response
 */
const getOptions = async (): Promise<{success: boolean, options?: typeOptions}> => {

    // Get the options
    return getAllOptions()

    // Proccess response
    .then((response: typeOptions) => {
        
        return {
            success: true,
            options: response
        };

    })

    // Catch the error
    .catch((error) => {

        // Display the error in the browser console
        console.error(error);

        return {
            success: false
        };

    });

}

/**
 * Gets default options for member
 * 
 * @returns object with response
 */
const getMemberOptions = (): {[key: string]: string} => {

    return {
        Default: '1',
        MemberId: '0',
        Email: '',
        FirstName: '',
        LastName: '',
        Role: '',
        SidebarStatus: '0',
        MembersChartTime: '1',
        ThreadsChartTime: '1',
        Language: '',
        ProfilePhoto: ''
    }

}

/**
 * Gets default options for website
 * 
 * @returns object with response
 */
const getWebsiteOptions = (): {[key: string]: string} => {

    return {
        Default: '1',
        WebsiteName: '',
        DashboardLogoSmall: '',
        SignInPageLogo: '',
        HomePageLogo: '',
        AnalyticsCode: '',
        RegistrationEnabled: '',
        PrivacyPolicy: '',
        Cookies: '',
        TermsOfService: '',
        DemoVideo: '',
        Ip2LocationEnabled: '',
        Ip2LocationKey: '',
        GoogleMapsEnabled: '',
        GoogleMapsKey: '', 
    }

}

// Return the functions
export {
    updateOptions,
    getOptions,
    getMemberOptions,
    getWebsiteOptions
};