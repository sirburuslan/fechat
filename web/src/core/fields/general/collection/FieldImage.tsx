/*
 * @component Field Image
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file shows the general image field in the app
 */


// Import the React hooks
import { useContext } from 'react';

// Import axios module
import axios, { AxiosError, AxiosProgressEvent, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import incs
import { getIcon, getWord, getToken, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeField, typeToken } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the FieldImage component
const FieldImage = (params: typeField): React.JSX.Element => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext); 

    /**
     * Change value handler
     */
    let changeValue = (): void => {

        // Change the input value
        params.hook.fields[params.name] = (document.querySelector(".fc-settings-option #fc-settings-image-input-" + params.name.toLowerCase()) as HTMLInputElement).value;

        // Update the fields
        params.hook.setFields(params.hook.fields);

        // Show the save changes button
        document.getElementsByClassName('fc-settings-actions')[0].classList.add('fc-settings-actions-show');

    };

    /**
     * Images uploader
     * 
     * @param event e
     */
    let uploadImage = async (e: React.ChangeEvent): Promise<void> => {

        // Set the bearer token
        let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Get the files
        let files: FileList | null = (e.target as HTMLInputElement).files;

        // Create an instance for the form data
        let form: FormData = new FormData();
        
        // Set text input
        form.append('file', files![0]);   

        try {

            // Select media
            let mediaContainer: HTMLElement | null = e.currentTarget.closest('.fc-settings-media');

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));

            }

            // Upload the image on the server
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/upload/image', form, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                },
                onUploadProgress: (progressEvent: AxiosProgressEvent) => {

                    // Verify if total is numeric
                    if ( typeof progressEvent.total === 'number' ) {

                        // Calculate the progress percentage
                        let progress: number = (progressEvent.loaded / progressEvent.total) * 100;

                        // Set the progress percentage
                        mediaContainer!.style.cssText = `--width: ${progress.toFixed(2)}%`;

                    }

                }
            })

            // Process the response
            .then((response: AxiosResponse) => {

                // Set a pause
                setTimeout((): void => {

                    // Add class fc-settings-media-uploaded to the media container
                    mediaContainer!.classList.add('fc-settings-media-uploaded');

                    // Reset the progress bar
                    mediaContainer!.style.cssText = `--width: 0`;   

                    // Set a pause
                    setTimeout((): void => {

                        // Remove class fc-settings-media-uploaded from the media container
                        mediaContainer!.classList.remove('fc-settings-media-uploaded');                        

                    }, 1000);

                }, 1000);
                
                // Check if the file was uploaded
                if ( response.data.success ) {

                    // Change the input url
                    (mediaContainer!.getElementsByClassName('fc-settings-image-input')[0] as HTMLInputElement).value = response.data.fileUrl;

                    // Show the save changes button
                    changeValue();

                } else {

                    // Show error notification
                    throw new Error(response.data.message);

                }

            })
            
            // Catch the error message
            .catch((error: AxiosError): void => {

                // Set a pause
                setTimeout((): void => {

                    // Add class fc-settings-media-uploaded to the media container
                    mediaContainer!.classList.add('fc-settings-media-uploaded');

                    // Reset the progress bar
                    mediaContainer!.style.cssText = `--width: 0`;   

                    // Set a pause
                    setTimeout((): void => {

                        // Remove class fc-settings-media-uploaded from the media container
                        mediaContainer!.classList.remove('fc-settings-media-uploaded');                        

                    }, 1000);

                }, 1000);

                // Check if message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);

                }

            });

        } catch (error: unknown) {

            // Check if error is known
            if ( error instanceof Error ) {

                // Show error notification
                showNotification('error', error.message);

            } else {

                // Display in the console the error
                console.log(error);

            }

        }

    };

    return (
        <li className="fc-settings-option" data-option={ params.name }>
            <div className="grid xl:grid-cols-3">
                <div>
                    <h3>{ params.label }</h3>
                    <p>{ params.description }</p>
                </div>
                <div className="xl:col-span-2">
                    <div className="fc-settings-media flex">
                        <input type="text" placeholder={ params.data.placeholder } defaultValue={ params.hook.fields[params.name] } name={"fc-settings-image-input-" + params.name.toLowerCase()} id={"fc-settings-image-input-" + params.name.toLowerCase()} className="fc-settings-image-input" onChange={(): void => changeValue()} onInput={(): void => changeValue()} />
                        <button className="mb-3 fc-option-btn fc-option-green-btn" onClick={(e: React.MouseEvent<HTMLButtonElement>): void => {
                            ((e.target as Element).closest('.fc-settings-media')!.getElementsByClassName('fc-option-upload-file')[0] as HTMLButtonElement).click();
                        }}>
                            { getIcon('IconCloudUpload') }
                        </button> 
                        <form>
                            <input type="file" accept="image/jpeg,image/gif,image/png" name={"fc-settings-file-input-" + params.name.toLowerCase()} id={"fc-settings-file-input-" + params.name.toLowerCase()} className="fc-option-upload-file" onChange={uploadImage} />
                        </form>
                    </div>
                    <p className={ (typeof params.hook.errors![params.name] !== 'undefined')?'fc-settings-option-error fc-settings-option-error-show':'fc-settings-option-error' }>{ (typeof params.hook.errors![params.name] !== 'undefined')?params.hook.errors![params.name]:'' }</p>
                </div>
            </div>
        </li>
    );

}

// Export the FieldImage component
export default FieldImage;