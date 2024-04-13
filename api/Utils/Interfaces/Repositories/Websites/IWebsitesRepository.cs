/*
 * @interface Websites Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This interface is implemented in WebsitesRepository
 */

// Namespace for Websites Repositories
namespace FeChat.Utils.Interfaces.Repositories.Websites {

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Websites;

    /// <summary>
    /// Interface for Websites Repository
    /// </summary>
    public interface IWebsitesRepository {

        /// <summary>
        /// Save a website
        /// </summary>
        /// <param name="websiteDto">Contains the website's data</param>
        /// <returns>Website ID</returns>
        Task<ResponseDto<NewWebsiteDto>> SaveAsync(NewWebsiteDto websiteDto);


        /// <summary>
        /// Update a website
        /// </summary>
        /// <param name="websiteDto">Contains the website's data</param>
        /// <returns>Boolean and error message</returns>
        Task<ResponseDto<bool>> UpdateWebsiteAsync(NewWebsiteDto websiteDto);

        /// <summary>
        /// Update a website chat
        /// </summary>
        /// <param name="websiteDto">Contains the website's data</param>
        /// <returns>Boolean and error message</returns>
        Task<ResponseDto<bool>> UpdateChatAsync(NewWebsiteDto websiteDto);

        /// <summary>
        /// Gets all websites
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with websites</returns>
        Task<ResponseDto<ElementsDto<NewWebsiteDto>>> GetWebsitesAsync(int memberId, SearchDto searchDto);

        /// <summary>
        /// Get website by domain
        /// </summary>
        /// <param name="websiteDto">Website data</param>
        /// <returns>Website if exists</returns>
        Task<ResponseDto<NewWebsiteDto>> GetWebsiteByDomainAsync(NewWebsiteDto websiteDto);

        /// <summary>
        /// Get website data
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="websiteId">Website ID</param>
        /// <returns>WebsiteDto with website's data</returns>
        Task<ResponseDto<WebsiteDto>> GetWebsiteAsync(int memberId, int websiteId);

        /// <summary>
        /// Get website data for public
        /// </summary>
        /// <param name="websiteId">Website ID</param>
        /// <returns>WebsiteDto with website's data</returns>
        Task<ResponseDto<WebsiteDto>> GetWebsiteInfoAsync(int websiteId);

        /// <summary>
        /// Delete website
        /// </summary>
        /// <param name="websiteId">Website ID</param>
        /// <param name="memberId">Member ID</param>
        /// <returns>Bool true if the website was deleted</returns>
        Task<ResponseDto<bool>> DeleteWebsiteAsync(int websiteId, int memberId);

        /// <summary>
        /// Delete member websites
        /// </summary>
        /// <param name="memberId">Member ID</param>
        Task DeleteMemberWebsitesAsync(int memberId);

    }

}