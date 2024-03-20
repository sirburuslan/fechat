/**
 * IconShoppingBasket
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconShoppingBasket = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>shopping_basket</span>
    );

}

// Export the function
export default IconShoppingBasket;