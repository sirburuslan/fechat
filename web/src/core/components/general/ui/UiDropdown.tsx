/*
 * @component Ui Dropdown
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component shows the ui dropdown in the app
 */

'use client'

// Import the React's hooks
import { useRef, useEffect } from 'react';

// Import the Link
import Link from 'next/link';

// Import the incs
import { getIcon } from '@/core/inc/incIndex';

// Get usePathname from the navigation
import { usePathname } from 'next/navigation';

// The type for the Dropdown's properties
type DropdownProps = {
    button: React.ReactNode;
    options: Array<{id?: number | string, text: string, url: string}>,
    id: string,
    menuPosition?: string
}

// Create the dropdown
const UiDropdown: React.FC<DropdownProps> = ({button, options, id, menuPosition}): React.JSX.Element => {

    // Register a reference for the dropdown
    const menuRef = useRef<HTMLDivElement>(null);

    // Get the current path name
    const pathname: string = usePathname();

    // Handle click outside
    const handleClickOutside: (e: MouseEvent) => void = (e: MouseEvent): void => {

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

        };

    }, []);

    // Show menu
    const buttonClick: (e: React.MouseEvent<HTMLElement>) => void = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Get the target
        const target = e.target as HTMLElement;

        // Verify if the dropdown is showed
        if ( target.closest('.fc-dropdown')!.getAttribute('data-expand') === 'false' ) {

            // Change the dropdown status
            target.closest('.fc-dropdown')!.setAttribute('data-expand', 'true');

            // Get menu
            const menu: Element = target.closest('.fc-dropdown')!.getElementsByClassName('fc-dropdown-menu')[0];

            // Get the height
            const height: number = menu.clientHeight;

            // Calculate the height of the button
            const button_height: number = target.offsetHeight;

            // Set transformation
            (menu as HTMLElement).style.transform = `translate3d(0, -${button_height + height}px, 0)`;

        } else {

            // Change the dropdown status
            target.closest('.fc-dropdown')!.setAttribute('data-expand', 'false');

        }

    }

    return (
        <div className="fc-dropdown" id={id} data-expand="false">
            <button type="button" className="fc-button flex justify-between" onClick={buttonClick}>
                <span>
                    {button}
                </span>
                { getIcon('IconExpandMore', {className: 'fc-dropdown-arrow-down-icon'}) }
            </button>
            <div className={(menuPosition !== undefined)?menuPosition + ' fc-dropdown-menu':'fc-dropdown-menu'} ref={menuRef}>
                <ul>
                {options.map((option: {text: string, url: string, id?: number | string}) => (
                    <li key={option.text}>
                        <Link href={option.url} className={(option.url === pathname)?'fc-active':''} data-id={(typeof option.id !== 'undefined')?option.id:''} scroll={false}>
                            {option.text}
                        </Link>
                    </li>
                ))}
                </ul>
            </div>
        </div>
    );

}

// Export the Dropdown
export default UiDropdown;