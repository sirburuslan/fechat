/**
 * IconPersonCheckFill
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconPersonCheckFill = (params: {[key: string]: string | number}): React.JSX.Element => {

    // Create the icon class
    let icon_class: string = params?.className?params.className as string + ' bi bi-person-check-fill':'bi bi-person-check-fill';

    return (
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className={icon_class} viewBox="0 0 16 16">
            <path fillRule="evenodd" d="M15.854 5.146a.5.5 0 0 1 0 .708l-3 3a.5.5 0 0 1-.708 0l-1.5-1.5a.5.5 0 0 1 .708-.708L12.5 7.793l2.646-2.647a.5.5 0 0 1 .708 0"/>
            <path d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6"/>
        </svg>
    );

}

// Export the function
export default IconPersonCheckFill;