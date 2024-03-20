/*
 * @class Plans Restrictions Dto Model Binder
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-16
 *
 * This scope of this model binder is to help the administrator to understand which restriction has wrong value
 */

// Namespace for Binders Models
namespace FeChat.Models.Binders {
    
    // Use the Json classes for deserialization
    using Newtonsoft.Json;

    // Use the Model Binding of Asp Net Core
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    // Use the Plans Dtos
    using FeChat.Models.Dtos.Plans;

    // Use the General classes for Strings
    using FeChat.Utils.General;

    /// <summary>
    /// Model Binder for RestrictionsDto
    /// </summary>
    public class RestrictionsDtoBinder: IModelBinder {

        /// <summary>
        /// Handle the data from an incoming request
        /// </summary>
        /// <param name="bindingContext">Contains incomed information</param>
        public async Task BindModelAsync(ModelBindingContext bindingContext) {

            // Check if bindingContext has no data
            if (bindingContext == null) {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            try {

                // Read the data from the body
                using StreamReader reader = new (bindingContext.HttpContext.Request.Body);

                // Read the data from the body
                string body = await reader.ReadToEndAsync();

                // Deserialize the request body to the target model
                RestrictionsDto restrictionsDto = JsonConvert.DeserializeObject<RestrictionsDto>(body)!;

                // Mark result as success and set model
                bindingContext.Result = ModelBindingResult.Success(restrictionsDto);

            } catch ( JsonReaderException ex ) {

                // Mark result as failed
                bindingContext.Result = ModelBindingResult.Failed();

                // Message container
                string message = "";

                // Verify if the path is supported
                if ( ex.Path == "Websites" ) {

                    // Set message
                    message = new Strings().Get("WebsitesRestrictionWrongValue");

                } else {

                    // Set message
                    message = ex.Message;
                    
                }

                // Set error message
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, message);

            } catch ( Exception ex ) {

                // Mark result as failed
                bindingContext.Result = ModelBindingResult.Failed();

                // Set error message
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);

            }

        }
        
    }

}