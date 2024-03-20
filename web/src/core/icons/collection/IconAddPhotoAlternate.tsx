/**
 * IconAddPhotoAlternate
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconAddPhotoAlternate = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>add_photo_alternate</span>
    );

}

// Export the function
export default IconAddPhotoAlternate;