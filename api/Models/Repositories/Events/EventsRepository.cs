/*
 * @class Events Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This class is used manage the system events
 */

// Namespace for Events Repositories
namespace FeChat.Models.Repositories.Events {

    // Use the Entity Framework Core
    using Microsoft.EntityFrameworkCore;

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;

    // Use the Events entities
    using FeChat.Models.Entities.Events;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Events Dtos
    using FeChat.Models.Dtos.Events;

    // Get the Configuration Utils for connection
    using FeChat.Utils.Configuration;
    
    // Use General Utils
    using FeChat.Utils.General;

    // Use the interfaces for Events Repositories
    using FeChat.Utils.Interfaces.Repositories.Events;

    /// <summary>
    /// Events Repository
    /// </summary>
    public class EventsRepository: IEventsRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Members table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Members repository constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Database connection</param>
        public EventsRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Create an event
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="typeId">Type ID</param>
        public async Task CreateEventAsync(int memberId, int typeId) {

            try {

                // Create an event entity
                EventEntity eventEntity = new() {
                    MemberId = memberId,
                    TypeId = typeId,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add entity to the list
                _context.Events.Add(eventEntity);

                // Save changes
                int saveChanges = await _context.SaveChangesAsync();

                // Verify if the changes were saved
                if ( saveChanges > 0 ) {

                    // Delete the events cache
                    new Cache(_memoryCache).Remove("events");

                }

            } catch (Exception ex) {

                // Event Creation Error
                Console.WriteLine("CreateEventError: " + ex.Message);

            }

        }

        /// <summary>
        /// Gets events from the database
        /// </summary>
        /// <param name="memberId">Member Id</param>
        /// <param name="time">Time for events reading</param>
        /// <returns>List with events or error message</returns>
        public async Task<ResponseDto<List<EventDto>>> GetEventsAsync(int memberId, int time) {

            try {

                // Create the cache key
                string cacheKey = "fc_events_" + memberId + "_" + time;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out List<EventDto>? eventsResponse ) ) {

                    // Events entities
                    IQueryable<EventEntity> eventEntities = _context.Events.AsQueryable();

                    // Verify if member id is not 0
                    if ( memberId > 0 ) {

                        // Add member id condition
                        eventEntities = eventEntities.Where(e => e.MemberId == memberId);

                    }

                    // Request the events
                    eventsResponse = await eventEntities
                    .Select(e => new EventDto {
                        EventId = e.EventId,
                        MemberId = e.MemberId,
                        TypeId = e.TypeId,
                        Created = e.Created
                    })
                    .GroupJoin(
                        _context.MembersOptions.Where(opt => opt.OptionName == "ProfilePhoto"),
                        e => e.MemberId,
                        o => o.MemberId,
                        (e, options) => new { Event = e, Options = options.DefaultIfEmpty() }
                    )
                    .SelectMany(
                        eo => eo.Options,
                        (eo, o) => new { eo.Event, Option = o }
                    )
                    .Join(
                        _context.Members,
                        eo => eo.Event.MemberId,
                        m => m.MemberId,
                        (eo, m) => new EventDto {
                            EventId = eo.Event.EventId,
                            MemberId = eo.Event.MemberId,
                            TypeId = eo.Event.TypeId,
                            Created = eo.Event.Created,
                            FirstName = m.FirstName,
                            LastName = m.LastName,
                            Email = m.Email,
                            ProfilePhoto = eo.Option != null ? eo.Option.OptionValue : null
                        }
                    )
                    .Where(e => e.Created >= time && e.Created <= (time + 86400))
                    .OrderByDescending(e => e.EventId)
                    .ToListAsync();

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, eventsResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("events", cacheKey);

                }

                // Verify if events exists
                if ( (eventsResponse != null) && (eventsResponse.Count > 0) ) {

                    return new ResponseDto<List<EventDto>> {
                        Result = eventsResponse,
                        Message = null
                    };

                } else {

                    return new ResponseDto<List<EventDto>> {
                        Result = null,
                        Message = new Strings().Get("NoEventsFound")
                    };

                }

            } catch ( Exception ex ) {

                return new ResponseDto<List<EventDto>> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

        /// <summary>
        /// Delete member events
        /// </summary>
        /// <param name="memberId">Member ID</param>
        public async Task DeleteMemberEventsAsync(int memberId) {

            try {

                // Select events by member id
                List<EventEntity>? eventEntities = await _context.Events.Where(e => e.MemberId == memberId).ToListAsync();

                // Verify if events exists
                if ( (eventEntities != null) && (eventEntities.Count > 0) ) {

                    // Delete bulk events
                    _context.Events.RemoveRange(eventEntities);

                    // Save changes
                    int save = await _context.SaveChangesAsync();   

                    // Verify if the changes were saved
                    if ( save > 0 ) {

                        // Delete the events cache
                        new Cache(_memoryCache).Remove("events");    

                    }             

                }

            } catch (Exception ex) {

                // Events Deletion Error
                Console.WriteLine("DeleteEventsError: " + ex.Message);

            }

        }

    }

}