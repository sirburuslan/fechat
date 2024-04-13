/*
 * @interface Events Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This interface is implemented in EventsRepository
 */

// Namespace for the Events Repositories
namespace FeChat.Utils.Interfaces.Repositories.Events {

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Events;

    /// <summary>
    /// Interface for Events Repository
    /// </summary>
    public interface IEventsRepository {

        /// <summary>
        /// Create an event
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <param name="typeId">Type ID</param>
        Task CreateEventAsync(int memberId, int typeId);

        /// <summary>
        /// Gets events from the database
        /// </summary>
        /// <param name="memberId">Member Id</param>
        /// <param name="time">Time for events reading</param>
        /// <returns>List with events or error message</returns>
        Task<ResponseDto<List<EventDto>>> GetEventsAsync(int memberId, int time);

        /// <summary>
        /// Delete member events
        /// </summary>
        /// <param name="memberId">Member ID</param>
        Task DeleteMemberEventsAsync(int memberId);

    }

}