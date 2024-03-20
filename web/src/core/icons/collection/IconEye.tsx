// Import HeroIcons module
import { EyeIcon } from '@heroicons/react/24/outline';

/**
 * IconEye
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconEye = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<EyeIcon className={params.className as string} />:<EyeIcon />
    );

}

// Export the function
export default IconEye;