/**
 * IconCropFree
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconCropFree = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>crop_free</span>
    );

}

// Export the function
export default IconCropFree;