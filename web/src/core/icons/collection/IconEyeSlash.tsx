// Import HeroIcons module
import { EyeSlashIcon } from '@heroicons/react/24/outline';

/**
 * IconEyeSlash
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconEyeSlash = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<EyeSlashIcon className={params.className as string} />:<EyeSlashIcon />
    );

}

// Export the function
export default IconEyeSlash;