/**
 * IconCloudUpload
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconCloudUpload = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>cloud_upload</span>
    );

}

// Export the function
export default IconCloudUpload;