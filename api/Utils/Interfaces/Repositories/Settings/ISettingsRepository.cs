/*
 * @interface Settings Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This interface is implemented in Settings Repository
 */

// Namespace for the Utilis Interfaces Settings Repositories
namespace FeChat.Utils.Interfaces.Repositories.Settings {

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Settings;
    using Models.Entities.Settings;

    /// <summary>
    /// Interface for Settings Repository
    /// </summary>
    public interface ISettingsRepository {

        /// <summary>
        /// Save bulk options
        /// </summary>
        /// <param name="options">Members options list</param>
        /// <returns>Boolean response</returns>
        Task<bool> SaveOptionsAsync(List<SettingsEntity> options);

        /// <summary>
        /// Update bulk options
        /// </summary>
        /// <param name="options">Settings options list</param>
        /// <returns>Boolean response</returns>
        bool UpdateOptionsAsync(List<SettingsEntity> options);

        /// <summary>
        /// Get the settings options
        /// </summary>
        /// <returns>The options list or null</returns>
        Task<ResponseDto<List<OptionDto>>> OptionsListAsync();

    } 

}