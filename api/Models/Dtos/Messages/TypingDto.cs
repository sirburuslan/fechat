/*
 * @class Typing Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This class is used for typing data transfer
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    /// <summary>
    /// Typing Dto
    /// </summary>
    public class TypingDto {

        /// <summary>
        /// Typing ID field
        /// </summary>
        public int Id { get; set; }       

        /// <summary>
        /// Thread ID field
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Member ID field
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Updated Time
        /// </summary>
        public int Updated { get; set; }

    }

}