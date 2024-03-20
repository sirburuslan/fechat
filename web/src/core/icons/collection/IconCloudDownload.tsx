/**
 * IconCloudDownload
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconCloudDownload = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>cloud_download</span>
    );

}

// Export the function
export default IconCloudDownload;