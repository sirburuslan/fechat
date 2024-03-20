/**
 * IconShoppingCartCheckout
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconShoppingCartCheckout = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>shopping_cart_checkout</span>
    );

}

// Export the function
export default IconShoppingCartCheckout;