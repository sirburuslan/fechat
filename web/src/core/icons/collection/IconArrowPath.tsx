// Import HeroIcons module
import { ArrowPathIcon } from '@heroicons/react/24/outline';

/**
 * IconArrowPath
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconArrowPath = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<ArrowPathIcon className={params.className as string} />:<ArrowPathIcon />
    );

}

// Export the function
export default IconArrowPath;