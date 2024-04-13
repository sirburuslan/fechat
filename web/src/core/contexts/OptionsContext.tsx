/*
 * @context Options
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-06
 *
 * This file contains functions to manage the options
 */

'use client'

// Import the React modules
import { createContext, useState, useEffect } from 'react';

// Import the types
import { typeOptions } from "@/core/types/typesIndex";

// Import the incs
import { updateOptions, getOptions, getMemberOptions, getWebsiteOptions } from '@/core/inc/incIndex';

/**
 * Create a context for website options
 * 
 * @param object with options
 */
export const WebsiteOptionsContext = createContext<{ websiteOptions: {[key: string]: string}, setWebsiteOptions: React.Dispatch<React.SetStateAction<any>> }>({
  websiteOptions: {},
  setWebsiteOptions: () => {}
});

/**
 * Create a context for member options
 * 
 * @param object with options
 */
export const MemberOptionsContext = createContext<{ memberOptions: {[key: string]: string}, setMemberOptions: React.Dispatch<React.SetStateAction<any>> }>({
  memberOptions: {},
  setMemberOptions: () => {}
});

/**
 * Create a provider for member
 * 
 * @param object with children
 * 
 * @returns React.JSX.Element
 */
export const OptionsProvider = ({ children }: {children: React.ReactNode}): React.JSX.Element => {

  // Create a state for website
  const [websiteOptions, setWebsiteOptions] = useState(getWebsiteOptions());  
  
  // Create a state for member
  const [memberOptions, setMemberOptions] = useState(getMemberOptions());

  // Get all options
  const getOptionsAll = async (): Promise<void> => {

    // Request the options
    const optionsList: {success: boolean, options?: typeOptions} = await getOptions();

    // Update memberOptions
    updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

  }

  // Run for client
  useEffect(() => {

    // Get the options
    getOptionsAll();

  }, []);

  return (
    <WebsiteOptionsContext.Provider value={{ websiteOptions, setWebsiteOptions }}>
      <MemberOptionsContext.Provider value={{ memberOptions, setMemberOptions }}>
        {children}
      </MemberOptionsContext.Provider>      
    </WebsiteOptionsContext.Provider>
  );
  
};