/**
 * IconFileDownloadDone
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconFileDownloadDone = (params: {[key: string]: string | number}): React.JSX.Element => {

    // Create the icon class
    const icon_class: string = params?.className?params.className as string + ' material-icons-outlined':'material-icons-outlined';

    return (
        <span className={icon_class}>file_download_done</span>
    );

}

// Export the function
export default IconFileDownloadDone;