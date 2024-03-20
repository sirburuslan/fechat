/**
 * IconFileDownload
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconFileDownload = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>file_download</span>
    );

}

// Export the function
export default IconFileDownload;