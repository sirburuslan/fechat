/*
 * @class Imgur Storage
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to upload images on imgur
 */

// Namespace for Storage utilis
namespace FeChat.Utils.Storage {

    // System Namespaces
    using Newtonsoft.Json.Linq;

    // App Namespaces
    using Models.Dtos;
    using Utils.Configuration;
    using Utils.General;    
    using Utils.Interfaces;

    /// <summary>
    /// Imgur Uploader
    /// </summary>
    public class ImgurStorage: IStorage {

        /// <summary>
        /// Upload the file on external storage
        /// </summary>
        /// <param name="storageOptions">Selected storage options</param>
        /// <param name="file">Uploaded file</param>
        /// <returns>Url of the uploaded file or null</returns>
        public async Task<ResponseDto<StorageDto>> UploadAsync(AppSettings.StorageFormat storageOptions, IFormFile file) {

            // Add storage options to the App Settings
            AppSettings.StorageFormat storageFormat = storageOptions;

            // Get the Client Id
            storageFormat.List.TryGetValue("Imgur", out AppSettings.StorageOptions? storageOptions1);

            // Verify if Imgur exists
            if ( storageOptions1 == null ) {

                // Return the message
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = new Strings().Get("StorageNotFound")
                };

            }

            // Set the Imgur's Client ID
            string authorizationHeader = "Client-ID " + storageOptions1.ClientId;

            try {

                // Read the file content into a byte array
                byte[] fileBytes;

                // Use the Memory Stream to read save temporary the file
                using MemoryStream ms = new();

                // Copy the file in memory
                await file.CopyToAsync(ms);

                // Turn to array and release the resources
                fileBytes = ms.ToArray();

                // Convert the byte array to a base64-encoded string
                string base64String = Convert.ToBase64String(fileBytes);

                // Send the request to Imgur using HttpClient
                using HttpClient httpClient = new();

                // Prepare the multipart content(our image)
                MultipartFormDataContent content = new () {

                    // Add the base 64 string to the image key
                    { new StringContent(base64String), "image" }

                };

                // Set the Imgur client id to authorize the access
                httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

                // Send request to Imgur
                var uploadImage = await httpClient.PostAsync("https://api.imgur.com/3/image", content);

                // Check if the image was uploaded
                if (uploadImage.IsSuccessStatusCode) {

                    // Read the response
                    string stringResponse = await uploadImage.Content.ReadAsStringAsync();

                    // Parse the JSON string into a JObject
                    JObject json = JObject.Parse(stringResponse);

                    // Extract the "link" property
                    string link = (string)json["data"]!["link"]!;

                    // Create the response
                    return new ResponseDto<StorageDto> {
                        Result = new StorageDto {
                            FileUrl = link
                        },
                        Message = null
                    };

                } else {

                    // Return the message
                    return new ResponseDto<StorageDto> {
                        Result = null,
                        Message = uploadImage.ReasonPhrase
                    };

                }

            } catch (Exception e) {

                // Return the message
                return new ResponseDto<StorageDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }

}