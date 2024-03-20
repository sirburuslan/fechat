/**
 * IconAddCard
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconAddCard = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>add_card</span>
    );

}

// Export the function
export default IconAddCard;