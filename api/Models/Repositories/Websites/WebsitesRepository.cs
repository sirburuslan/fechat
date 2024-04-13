/*
 * @class Websites Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used manage the websites
 */

// Namespace for the Websites Repositories
namespace FeChat.Models.Repositories.Websites {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Websites;
    using Models.Entities.Messages;
    using Models.Entities.Websites;
    using Messages;
    using Utils.Configuration;
    using Utils.General;
    using Utils.Interfaces.Repositories.Websites;

    /// <summary>
    /// Websites Repository
    /// </summary>
    public class WebsitesRepository: IWebsitesRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Websites table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Entity Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public WebsitesRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Save a website
        /// </summary>
        /// <param name="websiteDto">Contains the website's data</param>
        /// <returns>Website ID</returns>
        public async Task<ResponseDto<NewWebsiteDto>> SaveAsync(NewWebsiteDto websiteDto) {

            try {

                // Create an entity
                WebsiteEntity websiteEntity = new() {
                    MemberId = websiteDto.MemberId,
                    Name = websiteDto.Name,
                    Url = websiteDto.Url,
                    Domain = websiteDto.Domain,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add the entity
                _context.Websites.Add(websiteEntity);

                // Save the website
                int WebsiteId = await _context.SaveChangesAsync();

                // Verify if website's id exists
                if ( WebsiteId > 0 ) {

                    // Remove the caches for websites group
                    new Cache(_memoryCache).Remove("websites");

                    // Return success response
                    return new ResponseDto<NewWebsiteDto> {
                        Result = new NewWebsiteDto {
                            WebsiteId = WebsiteId
                        },
                        Message = new Strings().Get("WebsiteSaved")
                    };

                } else {

                    // Return error response
                    return new ResponseDto<NewWebsiteDto> {
                        Result = null,
                        Message = new Strings().Get("WebsiteNotSaved")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return error message
                return new ResponseDto<NewWebsiteDto> {
                    Result = null,
                    Message = e.Message
                };
                
            }

        }

        /// <summary>
        /// Update a website
        /// </summary>
        /// <param name="websiteDto">Contains the website's data</param>
        /// <returns>Boolean and error message</returns>
        public async Task<ResponseDto<bool>> UpdateWebsiteAsync(NewWebsiteDto websiteDto) {

            try {

                // Find the item you want to update
                WebsiteEntity? websiteEntity = await _context.Websites.FirstOrDefaultAsync(w => w.WebsiteId == websiteDto.WebsiteId && w.MemberId == websiteDto.MemberId);

                // Verify if the website was found
                if (websiteEntity != null) {

                    // Update the name
                    websiteEntity.Name = websiteDto.Name;
                    
                    // Update the url
                    websiteEntity.Url = websiteDto.Url;

                    // Update the domain
                    websiteEntity.Domain = websiteDto.Domain;

                    // Mark the item as modified
                    _context.Entry(websiteEntity).State = EntityState.Modified;

                    // Save changes to the database
                    int websiteUpdated = await _context.SaveChangesAsync();

                    // Verify if the website was updated
                    if ( websiteUpdated > 0 ) {

                        // Create the cache key
                        string cacheKey = "fc_website_" + websiteDto.WebsiteId;

                        // Delete the cache
                        _memoryCache.Remove(cacheKey);

                        // Remove the caches for websites group
                        new Cache(_memoryCache).Remove("websites");      

                        // Return error response
                        return new ResponseDto<bool> {
                            Result = true,
                            Message = null
                        };

                    }

                }

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = null
                };

            } catch (InvalidOperationException e) {

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Update a website chat
        /// </summary>
        /// <param name="websiteDto">Contains the website's data</param>
        /// <returns>Boolean and error message</returns>
        public async Task<ResponseDto<bool>> UpdateChatAsync(NewWebsiteDto websiteDto) {

            try {

                // Find the item you want to update
                WebsiteEntity? websiteEntity = await _context.Websites.FirstOrDefaultAsync(w => w.WebsiteId == websiteDto.WebsiteId && w.MemberId == websiteDto.MemberId);

                // Verify if the website was found
                if (websiteEntity != null) {

                    // Update the chat status
                    websiteEntity.Enabled = websiteDto.Enabled;

                    // Update the chat header
                    websiteEntity.Header = websiteDto.Header;

                    // Mark the item as modified
                    _context.Entry(websiteEntity).State = EntityState.Modified;

                    // Save changes to the database
                    int websiteUpdated = await _context.SaveChangesAsync();

                    // Verify if the website was updated
                    if ( websiteUpdated > 0 ) {

                        // Create the cache key
                        string cacheKey = "fc_website_" + websiteDto.WebsiteId;

                        // Delete the cache
                        _memoryCache.Remove(cacheKey);

                        // Remove the caches for websites group
                        new Cache(_memoryCache).Remove("websites");      

                        // Return error response
                        return new ResponseDto<bool> {
                            Result = true,
                            Message = null
                        };

                    }

                }

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = null
                };

            } catch (InvalidOperationException e) {

                // Return error response
                return new ResponseDto<bool> {
                    Result = false,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Gets all websites
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with websites</returns>
        public async Task<ResponseDto<ElementsDto<NewWebsiteDto>>> GetWebsitesAsync(int memberId, SearchDto searchDto) {

            try {

                // Prepare the page
                int page = (searchDto.Page > 0)?searchDto.Page:1;

                // Prepare the total results
                int total = 10;

                // Split the keys
                string[] searchKeys = searchDto.Search!.Split(' ');

                // Create the cache key
                string cacheKey = "fc_websites_" + memberId + "_" + string.Join("_", searchKeys) + '_' + searchDto.Page;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out Tuple<List<NewWebsiteDto>, int>? websitesResponse ) ) {

                    // Request the websites without projecting to WebsiteDto yet
                    IQueryable<WebsiteEntity> query = _context.Websites.AsQueryable();

                    // Apply filtering based on searchKeys
                    foreach (string key in searchKeys) {

                        // To avoid closure issue
                        string tempKey = key;

                        // Set where parameters
                        query = query.Where(w =>
                            EF.Functions.Like(w.Name!.ToLower(), $"%{tempKey.ToLower()}%") ||
                            EF.Functions.Like(w.Url!.ToLower(), $"%{tempKey.ToLower()}%")
                        );

                    }

                    // Request the websites
                    List<NewWebsiteDto> websites = await query
                    .Select(w => new NewWebsiteDto {
                            WebsiteId = w.WebsiteId,
                            MemberId = w.MemberId,
                            Enabled = w.Enabled,
                            Name = w.Name,
                            Url = w.Url,
                            Domain = w.Domain,
                            Created = w.Created
                        }
                    )
                    .Where(w => w.MemberId == memberId)
                    .OrderByDescending(w => w.WebsiteId)
                    .Skip((page - 1) * total)
                    .Take(total)
                    .ToListAsync();

                    // Get the total count before pagination
                    int totalCount = await query.Where(w => w.MemberId == memberId).CountAsync();

                    // Add data to website response
                    websitesResponse = new Tuple<List<NewWebsiteDto>, int>(websites, totalCount);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, websitesResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("websites", cacheKey);

                }

                // Verify if websites exists
                if ( (websitesResponse != null) && (websitesResponse.Item1.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<ElementsDto<NewWebsiteDto>> {
                        Result = new ElementsDto<NewWebsiteDto> {
                            Elements = websitesResponse.Item1,
                            Total = websitesResponse.Item2,
                            Page = searchDto.Page
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<ElementsDto<NewWebsiteDto>> {
                        Result = null,
                        Message = new Strings().Get("NoWebsitesFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<ElementsDto<NewWebsiteDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Get website by domain
        /// </summary>
        /// <param name="websiteDto">Website data</param>
        /// <returns>Website if exists</returns>
        public async Task<ResponseDto<NewWebsiteDto>> GetWebsiteByDomainAsync(NewWebsiteDto websiteDto) {

            try {

                // Get website from the database
                NewWebsiteDto? website = await _context.Websites
                .Select(w => new NewWebsiteDto {
                    WebsiteId = w.WebsiteId,
                    MemberId = w.MemberId,
                    Enabled = w.Enabled,
                    Name = w.Name,
                    Url = w.Url,
                    Domain = w.Domain,
                    Created = w.Created
                })
                .Where(w => w.Domain == websiteDto.Domain)
                .FirstOrDefaultAsync();

                // Check if website exists
                if ( website != null ) {

                    // Return response
                    return new ResponseDto<NewWebsiteDto> {
                        Result = website,
                        Message = null
                    };

                } else {

                    // Return response
                    return new ResponseDto<NewWebsiteDto> {
                        Result = null,
                        Message = new Strings().Get("WebsiteNotFound")
                    };

                }

            } catch (InvalidOperationException e) {

                // Return response
                return new ResponseDto<NewWebsiteDto> {
                    Result = null,
                    Message = e.Message
                };                   

            }

        }

        /// <summary>
        /// Get website data
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="websiteId">Website ID</param>
        /// <returns>WebsiteDto with website's data</returns>
        public async Task<ResponseDto<WebsiteDto>> GetWebsiteAsync(int memberId, int websiteId) {

            try {

                // Cache key for website
                string cacheKey = "fc_website_" + websiteId;

                // Verify if the website is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out WebsiteDto? websiteDto ) ) {

                    websiteDto = await _context.Websites
                    .Where(w => w.WebsiteId == websiteId && w.MemberId == memberId)
                    .Select(w => new WebsiteDto {
                        WebsiteId = w.WebsiteId,
                        MemberId = w.MemberId,
                        Enabled = w.Enabled,
                        Name = w.Name,
                        Header = w.Header,
                        Url = w.Url,
                        Domain = w.Domain,
                        Created = w.Created,
                        ProfilePhoto = _context.MembersOptions
                            .Where(opt => opt.OptionName == "ProfilePhoto" && opt.MemberId == w.MemberId)
                            .Select(opt => opt.OptionValue)
                            .FirstOrDefault(),
                        FirstName = _context.Members
                            .Where(m => m.MemberId == w.MemberId)
                            .Select(m => m.FirstName)
                            .FirstOrDefault(),
                        LastName = _context.Members
                            .Where(m => m.MemberId == w.MemberId)
                            .Select(m => m.LastName)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, websiteDto, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("websites", cacheKey);

                }

                // Verify if website exists
                if ( websiteDto != null ) {

                    // Return the website data
                    return new ResponseDto<WebsiteDto> {
                        Result = websiteDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<WebsiteDto> {
                        Result = null,
                        Message = new Strings().Get("WebsiteNotFound")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return the error message
                return new ResponseDto<WebsiteDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

        /// <summary>
        /// Get website data for public
        /// </summary>
        /// <param name="websiteId">Website ID</param>
        /// <returns>WebsiteDto with website's data</returns>
        public async Task<ResponseDto<WebsiteDto>> GetWebsiteInfoAsync(int websiteId) {

            try {

                // Cache key for website
                string cacheKey = "fc_website_public_" + websiteId;

                // Verify if the website is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out WebsiteDto? websiteDto ) ) {

                    websiteDto = await _context.Websites
                    .Where(w => w.WebsiteId == websiteId)
                    .Select(w => new WebsiteDto {
                        WebsiteId = w.WebsiteId,
                        MemberId = w.MemberId,
                        Enabled = w.Enabled,
                        Name = w.Name,
                        Header = w.Header,
                        Url = w.Url,
                        Domain = w.Domain,
                        Created = w.Created,
                        ProfilePhoto = _context.MembersOptions
                            .Where(opt => opt.OptionName == "ProfilePhoto" && opt.MemberId == w.MemberId)
                            .Select(opt => opt.OptionValue)
                            .FirstOrDefault(),
                        FirstName = _context.Members
                            .Where(m => m.MemberId == w.MemberId)
                            .Select(m => m.FirstName)
                            .FirstOrDefault(),
                        LastName = _context.Members
                            .Where(m => m.MemberId == w.MemberId)
                            .Select(m => m.LastName)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, websiteDto, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("websites", cacheKey);

                }

                // Verify if website exists
                if ( websiteDto != null ) {

                    // Return the website data
                    return new ResponseDto<WebsiteDto> {
                        Result = websiteDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<WebsiteDto> {
                        Result = null,
                        Message = new Strings().Get("WebsiteNotFound")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return the error message
                return new ResponseDto<WebsiteDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

        /// <summary>
        /// Delete website
        /// </summary>
        /// <param name="websiteId">Website ID</param>
        /// <param name="memberId">Member ID</param>
        /// <returns>Bool true if the website was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteWebsiteAsync(int websiteId, int memberId) {

            try {

                // Select the website by id
                WebsiteEntity? website = await _context.Websites.FirstAsync(m => m.WebsiteId == websiteId && m.MemberId == memberId);

                // Verify if website exists
                if ( website == null ) {

                    // Return the error message
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = new Strings().Get("WebsiteWasNotDeleted")
                    };

                }

                // Delete the website
                _context.Websites.RemoveRange(website);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Check if the changes were saved
                if ( saveChanges > 0 ) {

                    // Get all threads where website id is current
                    List<ThreadEntity>? threadEntities = await _context.Threads.Where(t => t.WebsiteId == websiteId).ToListAsync();

                    // Verify if the website has the threads
                    if ( (threadEntities != null) && (threadEntities.Count > 0) ) {

                        // Total number of threads
                        int totalThreads = threadEntities.Count;

                        // Get the threads class
                        MessagesRepository messagesRepository = new(_memoryCache, _context);

                        // List the threads
                        for ( int t = 0; t < totalThreads; t++ ) {

                            // Delete the thread
                            await messagesRepository.DeleteThreadAsync(threadEntities[t].ThreadId, threadEntities[t].MemberId);

                        }

                    }

                    // Remove the caches for websites group
                    new Cache(_memoryCache).Remove("websites"); 

                    // Return the success message
                    return new ResponseDto<bool> {
                        Result = true,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<bool> {
                        Result = false,
                        Message = null
                    };
                    
                }

            } catch ( InvalidOperationException ) {

                // Return the error message
                return new ResponseDto<bool> {
                    Result = false,
                    Message = new Strings().Get("WebsiteWasNotDeleted")
                };

            }

        }

        /// <summary>
        /// Delete member websites
        /// </summary>
        /// <param name="memberId">Member ID</param>
        public async Task DeleteMemberWebsitesAsync(int memberId) {

            try {
                
                // Get all member's websites
                List<WebsiteEntity>? websiteEntities = await _context.Websites.Where(w => w.MemberId == memberId).ToListAsync();

                // Verify if websites exists
                if ( (websiteEntities != null) && (websiteEntities.Count > 0) ) {

                    // Total websites
                    int totalWebsites = websiteEntities.Count;

                    // List webites
                    for ( int w = 0; w < totalWebsites; w++ ) {

                        // Delete the threads and messages
                        await DeleteWebsiteAsync(websiteEntities.First().WebsiteId, memberId);

                    }

                }

            } catch ( InvalidOperationException ex ) {

                // Show in Console the error message
                Console.WriteLine("WebsitesDeletionError: " + ex.Message);

            }

        }

    }

}