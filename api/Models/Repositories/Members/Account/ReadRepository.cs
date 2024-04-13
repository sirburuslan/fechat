/*
 * @class Members Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to read members
 */

// Namespace for Members Account repositories
namespace FeChat.Models.Repositories.Members.Account {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Members;
    using Models.Entities.Members;
    using Utils.Configuration;
    using Utils.General;

    /// <summary>
    /// Members Read Repository pattern
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Members table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Members Read Repository constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Database connection</param>
        public ReadRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Gets all members
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with members</returns>
        public async Task<ResponseDto<ElementsDto<MemberDto>>> GetMembersAsync(SearchDto searchDto) {

            try {

                // Prepare the page
                int page = (searchDto.Page > 0)?searchDto.Page:1;

                // Prepare the total results
                int total = 24;

                // Split the keys
                string[] searchKeys = searchDto.Search!.Split(' ');

                // Create the cache key
                string cacheKey = "fc_members_" + string.Join("_", searchKeys) + '_' + searchDto.Page;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out Tuple<List<MemberDto>, int>? membersResponse ) ) {

                    // Request the members without projecting to MemberDto yet
                    IQueryable<MemberEntity> query = _context.Members.AsQueryable();

                    // Apply filtering based on searchKeys
                    foreach (string key in searchKeys) {

                        // To avoid closure issue
                        string tempKey = key;

                        // Set where parameters
                        query = query.Where(m =>
                            EF.Functions.Like(m.FirstName!.ToLower(), $"%{tempKey.ToLower()}%") ||
                            EF.Functions.Like(m.LastName!.ToLower(), $"%{tempKey.ToLower()}%") ||
                            EF.Functions.Like(m.Email!.ToLower(), $"%{tempKey.ToLower()}%")
                        );

                    }

                    // Request the members
                    List<MemberDto> members = await query
                    .GroupJoin(
                        _context.MembersOptions.Where(opt => opt.OptionName == "ProfilePhoto"),
                        member => member.MemberId,
                        option => option.MemberId,
                        (member, options) => new { member, options }
                    )
                    .SelectMany(
                        x => x.options.DefaultIfEmpty(),
                        (x, option) => new MemberDto {
                            MemberId = x.member.MemberId,
                            FirstName = x.member.FirstName,
                            LastName = x.member.LastName,
                            Email = x.member.Email,
                            Role = x.member.Role,
                            Created = x.member.Created,
                            ProfilePhoto = option != null ? option.OptionValue : null
                        }
                    )
                    .OrderByDescending(m => m.MemberId)
                    .Skip((page - 1) * total)
                    .Take(total)
                    .ToListAsync();

                    // Get the total count before pagination
                    int totalCount = await query.CountAsync();

                    // Add data to member response
                    membersResponse = new Tuple<List<MemberDto>, int>(members, totalCount);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, membersResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("members", cacheKey);

                }

                // Verify if members exists
                if ( (membersResponse != null) && (membersResponse.Item1.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<ElementsDto<MemberDto>> {
                        Result = new ElementsDto<MemberDto> {
                            Elements = membersResponse.Item1,
                            Total = membersResponse.Item2,
                            Page = searchDto.Page
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<ElementsDto<MemberDto>> {
                        Result = null,
                        Message = new Strings().Get("NoMembersFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<ElementsDto<MemberDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Gets all members by time
        /// </summary>
        /// <param name="time">Time filter</param>
        /// <returns>List with members</returns>
        public async Task<ResponseDto<object>> GetMembersByTimeAsync(int time) {

            try {

                // Create the cache key
                string cacheKey = "fc_members_" + time;
                
                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out var membersResponse ) ) {

                    // Calculate the requested time
                    int requestedTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (time * 86400);

                    // Request the members
                    var members = await _context.Members
                    .Select(m => new {
                        Member = new MemberDto {
                            MemberId = m.MemberId,
                            Created = m.Created
                        },
                        CreatedDay = DateTimeOffset.FromUnixTimeSeconds(m.Created).DateTime.Year + "/" + DateTimeOffset.FromUnixTimeSeconds(m.Created).DateTime.Month + "/" + DateTimeOffset.FromUnixTimeSeconds(m.Created).DateTime.Day
                    })
                    .Where(m => m.Member.Created > requestedTime)
                    .OrderByDescending(m => m.Member.MemberId)
                    .ToListAsync();

                    // Verify if members exists
                    if ( members != null ) {

                        // Group the members
                        membersResponse = members.GroupBy(m => m.CreatedDay).Select(group => new {group.Key, Count = group.Count()}).OrderBy(g => int.Parse(g.Key.Replace("/", "")));

                    } else {

                        // Group the members
                        membersResponse = null;              

                    }
                    
                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, membersResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("members", cacheKey);

                }

                // Verify if members exists
                if ( membersResponse != null ) {

                    // Return the response
                    return new ResponseDto<object> {
                        Result = membersResponse,
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<object> {
                        Result = null,
                        Message = new Strings().Get("NoMembersFound")
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
        /// Gets all members for export
        /// </summary>
        /// <returns>List with all members</returns>
        public async Task<ResponseDto<List<MemberDto>>> GetMembersForExportAsync() {

            try {

                // Create the cache key
                string cacheKey = "fc_members_all";
                
                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out List<MemberDto>? membersEntities ) ) {

                    // Request the members
                    membersEntities = await _context.Members
                    .GroupJoin(
                        _context.MembersOptions.Where(opt => opt.OptionName == "ProfilePhoto"),
                        member => member.MemberId,
                        option => option.MemberId,
                        (member, options) => new { member, options }
                    )
                    .SelectMany(
                        x => x.options.DefaultIfEmpty(),
                        (x, option) => new MemberDto {
                            MemberId = x.member.MemberId,
                            FirstName = x.member.FirstName,
                            LastName = x.member.LastName,
                            Email = x.member.Email,
                            Role = x.member.Role,
                            Created = x.member.Created,
                            ProfilePhoto = option != null ? option.OptionValue : null
                        }
                    )
                    .OrderByDescending(m => m.MemberId)
                    .ToListAsync();
                    
                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, membersEntities, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("members", cacheKey);

                }

                // Verify if members exists
                if ( (membersEntities != null) && (membersEntities.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<List<MemberDto>> {
                        Result = membersEntities,
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<List<MemberDto>> {
                        Result = null,
                        Message = new Strings().Get("NoMembersFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<List<MemberDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Get member data
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>MemberDto with member's data</returns>
        public async Task<ResponseDto<MemberDto>> GetMemberAsync(int memberId) {

            try {

                // Cache key for member
                string cacheKey = "fc_member_" + memberId;

                // Verify if the member is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out MemberDto? member ) ) {

                    // Get the member by id
                    member = await _context.Members
                    .Select(m => new MemberDto {
                        MemberId = m.MemberId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Email = m.Email,
                        Role = m.Role,
                        Created = m.Created
                    })
                    .GroupJoin(
                        _context.MembersOptions.Where(o => o.OptionName == "Language"),
                        member => member.MemberId,
                        option => option.MemberId,
                        (member, options) => new {member, options}
                    )
                    .SelectMany(
                        x => x.options.DefaultIfEmpty(),
                        (x, option) => new MemberDto {
                            MemberId = x.member.MemberId,
                            FirstName = x.member.FirstName,
                            LastName = x.member.LastName,
                            Email = x.member.Email,
                            Role = x.member.Role,
                            Created = x.member.Created,
                            Language = option != null ? option.OptionValue : null               
                        }
                    )
                    .FirstAsync(u => u.MemberId == memberId); 

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // So long because i'm using the key to clear when the user's data is updated
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, member, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("members", cacheKey);

                }

                // Verify if member exists
                if ( member != null ) {

                    // Return the member data
                    return new ResponseDto<MemberDto> {
                        Result = member,
                        Message = null
                    };

                } else {

                    // Return the error message
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("AccountNotFound")
                    };
                    
                }

            } catch ( Exception e ) {

                // Return the error message
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = e.Message
                };

            }

        }

        /// <summary>
        /// Get member email
        /// </summary>
        /// <param name="memberDto">Member data</param>
        /// <returns>Member with email if exists</returns>
        public async Task<ResponseDto<MemberDto>> GetMemberEmailAsync(MemberDto memberDto) {

            try {

                // Cache key for member
                string cacheKey = "fc_member_" + memberDto.Email;

                // Verify if the member is saved in the cache
                if ( !_memoryCache.TryGetValue(cacheKey, out MemberDto? member ) ) {

                    // Get email from the database
                    member = await _context.Members
                    .Select(m => new MemberDto {
                        MemberId = m.MemberId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Role = m.Role,
                        Email = m.Email,
                        Password = m.Password,
                        ResetCode = m.ResetCode,
                        ResetTime = m.ResetTime,
                        Created = m.Created
                    })
                    .Where(m => m.Email == memberDto.Email)
                    .FirstOrDefaultAsync();

                    // Create the options for cache storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // So long because i'm using the key to clear when the user's data is updated
                    };

                    // Create the cache
                    _memoryCache.Set(cacheKey, member, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("members", cacheKey);

                }

                // Check if member exists
                if ( member != null ) {

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = member,
                        Message = null
                    };

                } else {

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("EmailNotFound")
                    };

                }

            } catch (InvalidOperationException e) {

                // Return response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = e.Message
                };                   

            }

        }

        /// <summary>
        /// Get member by reset code
        /// </summary>
        /// <param name="code">Reset code</param>
        /// <returns>Member with email if exists</returns>
        public async Task<ResponseDto<MemberDto>> GetMemberByCodeAsync(string code) {

            try {

                // Get email from the database
                MemberDto? member = await _context.Members
                .Select(m => new MemberDto {
                    MemberId = m.MemberId,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Role = m.Role,
                    Email = m.Email,
                    Password = m.Password,
                    ResetCode = m.ResetCode,
                    ResetTime = m.ResetTime,
                    Created = m.Created
                })
                .Where(m => m.ResetCode == code)
                .OrderByDescending(m => m.ResetTime)
                .FirstOrDefaultAsync();

                // Check if member exists
                if ( member != null ) {

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = member,
                        Message = null
                    };

                } else {

                    // Return response
                    return new ResponseDto<MemberDto> {
                        Result = null,
                        Message = new Strings().Get("ResetCodeNotvalid")
                    };

                }

            } catch (InvalidOperationException e) {

                // Return response
                return new ResponseDto<MemberDto> {
                    Result = null,
                    Message = e.Message
                };                   

            }

        }

    }

}