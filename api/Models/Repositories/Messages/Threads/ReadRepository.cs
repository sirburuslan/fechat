/*
 * @class Threads Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the threads
 */

// Namespace for Messages Threads Repositories
namespace FeChat.Models.Repositories.Messages.Threads {

    // Use the Entity Framework
    using Microsoft.EntityFrameworkCore;

    // Use the catching memory
    using Microsoft.Extensions.Caching.Memory;

    // Use the general Dtos for Response
    using FeChat.Models.Dtos;

    // Use the Dtos for messages
    using FeChat.Models.Dtos.Messages;

    // Use Messages entities
    using FeChat.Models.Entities.Messages;

    // Use the Configuration utils for Db connector
    using FeChat.Utils.Configuration;

    // Use General Utils
    using FeChat.Utils.General;

    /// <summary>
    /// Threads Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Threads Read Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public ReadRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Gets all threads
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="memberId">Member Id</param>
        /// <param name="websiteId">Website ID</param>
        /// <returns>List with threads</returns>
        public async Task<ResponseDto<ElementsDto<ResponseThreadDto>>> GetThreadsAsync(SearchDto searchDto, int memberId, int? websiteId) {

            try {

                // Prepare the page
                int page = (searchDto.Page > 0)?searchDto.Page:1;

                // Prepare the total results
                int total = 10;

                // Split the keys
                string[] searchKeys = searchDto.Search!.Split(' ');

                // Create the cache key
                string cacheKey = "fc_threads_" + memberId + "_" + string.Join("_", searchKeys) + '_' + searchDto.Page;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out Tuple<List<ResponseThreadDto>, int>? threadsResponse ) ) {

                    // Request the messages
                    IQueryable<MessageEntity> query = _context.Messages.AsQueryable();

                    // Verify if search keys exists
                    if ( searchDto.Search != "" ) {

                        // Apply filtering based on searchKeys
                        foreach (string key in searchKeys) {

                            // To avoid closure issue
                            string tempKey = key;

                            // Set where parameters
                            query = query.Where(m =>
                                EF.Functions.Like(m.Message!.ToLower(), $"%{tempKey.ToLower()}%")
                            );

                        }

                    }

                    // Request the threads
                    List<ResponseThreadDto> threads = await query
                    .GroupBy(m => m.ThreadId)
                    .Select(group => new ResponseThreadDto {
                        ThreadId = group.Key,
                        MessageId = group.FirstOrDefault()!.MessageId,
                        Message = group.FirstOrDefault()!.Message,
                        MessageSeen = group.FirstOrDefault()!.Seen,
                        MessageCreated = group.Max(m => m.Created),
                        TotalMessages = group.Count()
                    })
                    .Join(
                        _context.Threads,
                        m => m.ThreadId,
                        t => t.ThreadId,
                        (m, thread) => new {
                            thread.ThreadId,
                            Thread = thread,
                            Message = m
                        }
                    )
                    .Join(
                        _context.Guests,
                        t => t.Thread.GuestId,
                        g => g.GuestId,
                        (t, guest) => new ResponseThreadDto {
                            ThreadId = t.Thread.ThreadId,
                            MemberId = t.Thread.MemberId,
                            WebsiteId = t.Thread.WebsiteId,
                            GuestId = guest.GuestId,
                            GuestName = guest.Name,
                            GuestEmail = guest.Email,
                            GuestIp = guest.Ip,
                            GuestLatitude = guest.Latitude,
                            GuestLongitude = guest.Longitude,
                            Created = guest.Created,
                            MessageId = t.Message.MessageId,
                            Message = t.Message.Message,
                            MessageCreated = t.Message.MessageCreated,
                            MessageSeen = t.Message.MessageSeen,
                            TotalMessages = t.Message.TotalMessages
                        }
                    )
                    .Where(m => m.MemberId == memberId && (websiteId == null || m.WebsiteId == websiteId))
                    .OrderByDescending(m => m.MessageCreated)
                    .Skip((page - 1) * total)
                    .Take(total)
                    .ToListAsync();

                    // Get the total count before pagination
                    int totalCount = await query.Select(message => message.ThreadId).Distinct().Join(
                        _context.Threads,
                        threadId => threadId,
                        t => t.ThreadId,
                        (threadId, thread) => new ThreadDto {
                            ThreadId = threadId,
                            Created = thread.Created
                        }
                    ).CountAsync();

                    // Add data to thread response
                    threadsResponse = new Tuple<List<ResponseThreadDto>, int>(threads, totalCount);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, threadsResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("threads", cacheKey);

                }

                // Verify if threads exists
                if ( (threadsResponse != null) && (threadsResponse.Item1.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<ElementsDto<ResponseThreadDto>> {
                        Result = new ElementsDto<ResponseThreadDto> {
                            Elements = threadsResponse.Item1,
                            Total = threadsResponse.Item2,
                            Page = searchDto.Page
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<ElementsDto<ResponseThreadDto>> {
                        Result = null,
                        Message = new Strings().Get("NoThreadsFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<ElementsDto<ResponseThreadDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Gets last 5 updated threads
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>List with threads</returns>
        public async Task<ResponseDto<ElementsDto<ResponseThreadDto>>> GetHotThreadsAsync(int memberId) {

            try {

                // Create the cache key
                string cacheKey = "fc_hot_threads_w" + memberId;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out List<ResponseThreadDto>? threads) ) {

                    // Request the threads
                    threads = await _context.Messages
                    .GroupBy(m => m.ThreadId)
                    .Select(group => new ResponseThreadDto {
                        ThreadId = group.Key,
                        MessageId = group.FirstOrDefault()!.MessageId,
                        Message = group.FirstOrDefault()!.Message,
                        MessageSeen = group.FirstOrDefault()!.Seen,
                        MessageCreated = group.Max(m => m.Created),
                        TotalMessages = group.Count()
                    })
                    .Join(
                        _context.Threads,
                        m => m.ThreadId,
                        t => t.ThreadId,
                        (m, thread) => new {
                            thread.ThreadId,
                            Thread = thread,
                            Message = m
                        }
                    )
                    .Join(
                        _context.Guests,
                        t => t.Thread.GuestId,
                        g => g.GuestId,
                        (t, guest) => new ResponseThreadDto {
                            ThreadId = t.Thread.ThreadId,
                            MemberId = t.Thread.MemberId,
                            GuestId = guest.GuestId,
                            GuestName = guest.Name,
                            GuestEmail = guest.Email,
                            GuestIp = guest.Ip,
                            Created = guest.Created,
                            MessageId = t.Message.MessageId,
                            Message = t.Message.Message,
                            MessageSeen = t.Message.MessageSeen,
                            MessageCreated = t.Message.MessageCreated,
                            TotalMessages = t.Message.TotalMessages
                        }
                    )
                    .Where(m => m.MemberId == memberId)
                    .OrderByDescending(m => m.MessageCreated)
                    .Skip(0)
                    .Take(5)
                    .ToListAsync();

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, threads, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("threads", cacheKey);

                }

                // Verify if threads exists
                if ( (threads != null) && (threads.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<ElementsDto<ResponseThreadDto>> {
                        Result = new ElementsDto<ResponseThreadDto> {
                            Elements = threads
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<ElementsDto<ResponseThreadDto>> {
                        Result = null,
                        Message = new Strings().Get("NoThreadsFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<ElementsDto<ResponseThreadDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Gets all threads by time
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="time">Time filter</param>
        /// <returns>List with threads</returns>
        public async Task<ResponseDto<object>> GetThreadsByTimeAsync(int memberId, int time) {

            try {

                // Create the cache key
                string cacheKey = "fc_threads_" + time + "_" + memberId;
                
                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out var threadsResponse ) ) {

                    // Calculate the requested time
                    int requestedTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (time * 86400);

                    // Request the threads
                    var threads = await _context.Threads
                    .Select(t => new {
                        Thread = new ThreadDto {
                            ThreadId = t.ThreadId,
                            MemberId = t.MemberId,
                            Created = t.Created
                        },
                        CreatedDay = DateTimeOffset.FromUnixTimeSeconds(t.Created).DateTime.Year + "/" + DateTimeOffset.FromUnixTimeSeconds(t.Created).DateTime.Month + "/" + DateTimeOffset.FromUnixTimeSeconds(t.Created).DateTime.Day
                    })
                    .Where(t => t.Thread.MemberId == memberId && t.Thread.Created > requestedTime)
                    .OrderByDescending(t => t.Thread.ThreadId)
                    .ToListAsync();

                    // Verify if threads exists
                    if ( threads != null ) {

                        // Group the threads
                        threadsResponse = threads.GroupBy(m => m.CreatedDay).Select(group => new {
                            group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(g => int.Parse(g.Key.Replace("/", "")));

                    } else {

                        // Group the threads
                        threadsResponse = null;              

                    }
                  
                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, threadsResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("threads", cacheKey);

                }

                // Verify if threads exists
                if ( threadsResponse != null ) {

                    // Return the response
                    return new ResponseDto<object> {
                        Result = new {
                            Threads = threadsResponse
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<object> {
                        Result = null,
                        Message = new Strings().Get("NoThreadsFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<object> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Get thread by website's id and thread secret
        /// </summary>
        /// <param name="threadDto">Thread information</param>
        /// <returns>Response with thread or error message</returns>
        public async Task<ResponseDto<ThreadDto>> GetThreadByWebsiteIdAsync(ThreadDto threadDto) {

            try {

                // Create the cache key
                string cacheKey = "fc_threads_" + threadDto.WebsiteId + "_" + threadDto.ThreadSecret;

                // Verify if the thread is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out ThreadDto? thread) ) {

                    // Get the thread
                    thread = await _context.Threads
                    .Select(t => new ThreadDto {
                        ThreadId = t.ThreadId,
                        ThreadSecret = t.ThreadSecret,
                        WebsiteId = t.WebsiteId,
                        MemberId = t.MemberId,
                        Created = t.Created
                    })
                    .FirstAsync(t => t.WebsiteId == threadDto.WebsiteId && t.ThreadSecret == threadDto.ThreadSecret);

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, thread, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("threads", cacheKey);

                }

                // Verify if thread exists
                if ( thread != null ) {

                    return new ResponseDto<ThreadDto> {
                        Result = thread,
                        Message = null
                    };

                } else {

                    return new ResponseDto<ThreadDto> {
                        Result = null,
                        Message = new Strings().Get("ThreadNotFound")
                    };

                }

            } catch (Exception ex) {

                return new ResponseDto<ThreadDto> {
                    Result = null,
                    Message = ex.Message
                };                

            }

        }

        /// <summary>
        /// Get thread data
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="memberId">Member ID which is owner of the thread</param>
        /// <returns>ThreadDto with thread's data</returns>
        public async Task<ResponseDto<ThreadDto>> GetThreadAsync(int threadId, int memberId) {

            try {

                // Cache key for thread
                string cacheKey = "fc_thread_" + threadId;

                // Verify if the thread is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out ThreadDto? threadDto ) ) {

                    // Get the thread by id
                    threadDto = await _context.Threads
                    .Where(t => t.ThreadId == threadId && _context.Websites.Any(w => w.MemberId == memberId))
                    .Join(
                        _context.Guests,
                        t => t.GuestId,
                        g => g.GuestId,
                        (t, guest) => new ThreadDto {
                            ThreadId = t.ThreadId,
                            WebsiteId = t.WebsiteId,
                            Created = t.Created,                            
                            GuestId = guest.GuestId,
                            GuestName = guest.Name,
                            GuestEmail = guest.Email,
                            GuestIp = guest.Ip,
                            GuestLatitude = guest.Latitude,
                            GuestLongitude = guest.Longitude
                        }
                    )
                    .FirstOrDefaultAsync(w => w.ThreadId == threadId); 

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, threadDto, cacheOptions);

                }

                // Verify if thread exists
                if ( threadDto != null ) {

                    // Return the thread data
                    return new ResponseDto<ThreadDto> {
                        Result = threadDto,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<ThreadDto> {
                        Result = null,
                        Message = new Strings().Get("ThreadNotFound")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return the error message
                return new ResponseDto<ThreadDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

    }
    
}