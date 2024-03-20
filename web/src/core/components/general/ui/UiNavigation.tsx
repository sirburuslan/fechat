/*
 * @component Ui Navgation
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component shows the ui navigation in the app
 */

'use client'

// Import the Next Link
import Link from 'next/link';

// Import the incs
import { getIcon } from '@/core/inc/incIndex';

// Create the Navigation
const UiNavigation: React.FC<{scope: string, page: number, total: number, limit: number}> = ({scope, page, total, limit}): React.JSX.Element => {

    // Count pages
    let totalPages: number = Math.ceil(total / limit) + 1;

    // Generate the pages
    let pagesList = (): React.JSX.Element[] => {

        // Calculate start page
        let from: number = (page > 2) ? (page - 2) : 1;

        // Initialize an array to store the pages JSX elements
        let pages: React.JSX.Element[] = [];
        
        // List all pages
        for ( let p: number = from; p < totalPages; p++ ) {
            
            // Verify if p is equal to current page
            if (p === page) {

                // Add current page
                pages.push(<li key={p} className="page-item active">
                    <Link href="#" className="page-link" data-page={p}>
                        {p}
                    </Link>
                </li>);

            } else if ((p < page + 3) && (p > page - 3)) {

                // Add page number
                pages.push(<li key={p} className="page-item">
                    <Link href="#" className="page-link" data-page={p}>
                        {p}
                    </Link>
                </li>);

            } else if ((p < 6) && (totalPages > 5) && ((page === 1) || (page === 2))) {

                // Add page number
                pages.push(<li key={p} className="page-item">
                    <Link href="#" className="page-link" data-page={p}>
                        {p}
                    </Link>
                </li>);

            } else {
                break;
            }

        }

        // Render the pages
        return pages;

    }

    return (
        <ul className="flex" data-scope={scope}>
            {( page > 1 )? (
                <li className="page-item">
                    <Link href="#" className="page-link" data-page={(page - 1)}>
                        { getIcon('IconNavigateBefore') }
                    </Link>
                </li>
            ): (
                <li className="page-item disabled">
                    <Link href="#" className="page-link">
                        { getIcon('IconNavigateBefore') }
                    </Link>
                </li>
            )}
            { pagesList() }
            {((page + 1) < totalPages)?(
                <li className="page-item">
                    <Link href="#" className="page-link" data-page={(page + 1)}>
                        { getIcon('IconNavigateNext') }
                    </Link>
                </li>
            ):(
                <li className="page-item disabled">
                    <Link href="#" className="page-link">
                        { getIcon('IconNavigateNext') }
                    </Link>
                </li>
            )}
        </ul>
    );

}

// Export the navigation
export default UiNavigation;