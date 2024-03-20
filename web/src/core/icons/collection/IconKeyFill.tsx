/**
 * IconKeyFill
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconKeyFill = (params: {[key: string]: string | number}): React.JSX.Element => {

    // Create the icon class
    let icon_class: string = params?.className?params.className as string + ' bi bi-key-fill':'bi bi-key-fill';

    return (
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className={icon_class} viewBox="0 0 16 16">
            <path d="M3.5 11.5a3.5 3.5 0 1 1 3.163-5H14L15.5 8 14 9.5l-1-1-1 1-1-1-1 1-1-1-1 1H6.663a3.5 3.5 0 0 1-3.163 2M2.5 9a1 1 0 1 0 0-2 1 1 0 0 0 0 2"/>
        </svg>
    );

}

// Export the function
export default IconKeyFill;