/*
 * @page Transactions
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page transactions in the administrator panel
 */

'use client'

// Import the react hooks
import { useContext  } from 'react';

// Import the incs
import { getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Transactions List component
import TransactionsList from "@/core/components/admin/transactions/TransactionsList";

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext);  

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_transactions', memberOptions['Language']);

    }

    return (
        <>
            <div className="fc-transactions-container">
                <div className="flex mb-3">
                    <div className="w-full">
                        <h2 className="fc-page-title">
                            { getWord('admin', 'admin_transactions', memberOptions['Language'])  }
                        </h2>
                    </div>
                </div>
                <TransactionsList />
            </div>
        </>
    );

};

// Export the page content
export default Page;