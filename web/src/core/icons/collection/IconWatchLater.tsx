/**
 * IconWatchLater
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconWatchLater = (params: {[key: string]: string | number}): React.JSX.Element => {

    // Create the icon class
    let icon_class: string = params?.className?params.className as string + ' material-icons-outlined':'material-icons-outlined';

    return (
        <span className={icon_class}>watch_later</span>
    );

}

// Export the function
export default IconWatchLater;