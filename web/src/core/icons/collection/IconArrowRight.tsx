// Import HeroIcons module
import { ArrowSmallRightIcon } from '@heroicons/react/24/outline';

/**
 * IconArrowSmallRight
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconArrowRight = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<ArrowSmallRightIcon className={params.className as string} />:<ArrowSmallRightIcon />
    );

}

// Export the function
export default IconArrowRight;