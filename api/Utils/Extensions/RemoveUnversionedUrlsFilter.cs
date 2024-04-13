/*
 * @class Remove Unversioned Urls Filter
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used remove urls without a version
 */

// Namespace for Extensions
namespace FeChat.Utils.Extensions {

    // System Namespaces
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Class used to remove the unversioned urls from Swagger
    /// </summary>
    public class RemoveUnversionedUrlsFilter : IDocumentFilter {

        /// <summary>
        /// Remove unversioned urls in a OpenApi document
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument swaggerDoc - swagger document</param>
        /// <param name="context">DocumentFilterContext context - current context</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            // Create a new list with paths which should be removed
            var paths = new List<string>();

            // List all the paths in the Swagger document
            foreach (var path in swaggerDoc.Paths)
            {

                // Check if the path contains the version
                if (!path.Key.Contains("/v")) {

                    // Add path to remove because should not be allowed
                    paths.Add(path.Key);

                }

            }

            // List the paths which should be removed
            foreach (var path in paths) {

                // Remove the path from the swagger document
                swaggerDoc.Paths.Remove(path);

            }

        }

    }

}