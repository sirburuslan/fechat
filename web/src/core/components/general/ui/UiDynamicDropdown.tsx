/*
 * @component Ui Dynamic Dropdown
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component shows the ui dynamic dropdown with search input in the app
 */

'use client'

// Import the React's hooks
import { useRef, useEffect } from 'react';

// Import the Next Link
import Link from 'next/link';

// Import the incs
import { getIcon, unescapeRegexString } from '@/core/inc/incIndex';

// The type for the Dropdown's properties
type DropdownProps = {
    button: React.ReactNode;
    placeholder: string,
    options: Array<{text: string, url: string}>,
    id: string
}

// Create the dropdown
const UiDynamicDropdown: React.FC<DropdownProps> = ({button, placeholder, options, id}): React.JSX.Element => {

    // Register a reference for the dropdown
    let menuRef = useRef<HTMLDivElement>(null);

    // Handle click outside
    let handleClickOutside: (e: MouseEvent) => void = (e: MouseEvent): void => {

        // Verify if the click is outside the menu
        if ( menuRef.current && !menuRef.current.contains(e.target as Node) ) {

            // Change the dropdown status
            menuRef.current.closest('.fc-dropdown')!.setAttribute('data-expand', 'false');

            // Remove the style from dropdown menu
            menuRef.current.closest('.fc-dropdown')!.getElementsByClassName('fc-dropdown-menu')[0].removeAttribute('style');

        }

    }

    // Monitor changes
    useEffect((): () => void => {

        // Register event
        document.addEventListener('mousedown', handleClickOutside);

        return (): void => {

            // Remove event
            document.removeEventListener('mousedown', handleClickOutside);

        }

    }, []);

    // Show menu
    let buttonClick: (e: React.MouseEvent<HTMLElement>) => void = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Get the target
        let target = e.target as HTMLElement;

        // Verify if the dropdown is showed
        if ( target.closest('.fc-dropdown')!.getAttribute('data-expand') === 'false' ) {

            // Change the dropdown status
            target.closest('.fc-dropdown')!.setAttribute('data-expand', 'true');

            // Get menu
            let menu: Element = target.closest('.fc-dropdown')!.getElementsByClassName('fc-dropdown-menu')[0];

            // Get the height
            let height: number = menu.clientHeight;

            // Calculate the height of the button
            let button_height: number = target.offsetHeight;

            // Set transformation
            (menu as HTMLElement).style.transform = `translate3d(0, -${button_height + height}px, 0)`;

        } else {

            // Change the dropdown status
            target.closest('.fc-dropdown')!.setAttribute('data-expand', 'false');

        }

    }

    return (
        <div className="fc-dropdown" id={id}>
            <button type="button" className="fc-button flex justify-between" onClick={buttonClick} data-expand="false">
                <span>
                    {button}
                </span>
                { getIcon('IconExpandMore', {className: 'fc-dropdown-arrow-down-icon'}) }
            </button>
            <div className="fc-dropdown-menu" ref={menuRef}>
                <input type="text" placeholder={ placeholder } className="fc-dropdown-search" />
                <ul>
                {options.map((option: {text: string, url: string}) => (
                    <li key={option.text}>
                        <Link href={option.url}>
                            {unescapeRegexString(option.text)}
                        </Link>
                    </li>
                ))}
                </ul>
            </div>
        </div>
    );

}

// Export the Dropdown
export default UiDynamicDropdown;