/**
 * IconRadioButtonUnchecked
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconRadioButtonUnchecked = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>radio_button_unchecked</span>
    );

}

// Export the function
export default IconRadioButtonUnchecked;