/*
 * @class Extension for Services
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
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension for Services
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register all services
        /// </summary>
        /// <param name="services">Collection with services</param>
        /// <param name="assembly">Represents an assembly</param>
        public static void RegisterRepositories(this IServiceCollection services, Assembly assembly)
        {

            // Get all repositories
            var repositoryTypes = assembly.GetTypes().Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.GetInterfaces().Any(i => i.Name.EndsWith("Repository")));

            // List all repositories
            foreach (var repositoryType in repositoryTypes)
            {
                // Get the interface for a repository
                var interfaceType = repositoryType.GetInterfaces().First(i => i.Name.EndsWith(repositoryType.Name));

                // Inject the repository as service
                services.AddScoped(interfaceType, repositoryType);
            }

        }

    }

}