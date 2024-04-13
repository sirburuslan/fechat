/**
 * IconFeature
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconFeature = (params: {[key: string]: string | number}): React.JSX.Element => {

    // Create the icon class
    const icon_class: string = params?.className?params.className as string + ' bi bi-tag-fill':'bi bi-tag-fill';

    return (
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className={icon_class} viewBox="0 0 16 16">
            <path d="M2 1a1 1 0 0 0-1 1v4.586a1 1 0 0 0 .293.707l7 7a1 1 0 0 0 1.414 0l4.586-4.586a1 1 0 0 0 0-1.414l-7-7A1 1 0 0 0 6.586 1zm4 3.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0"/>
        </svg>
    );

}

// Export the function
export default IconFeature;