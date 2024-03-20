/**
 * IconCropOriginal
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconCropOriginal = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>crop_original</span>
    );

}

// Export the function
export default IconCropOriginal;