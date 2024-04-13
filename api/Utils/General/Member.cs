/*
 * @class Member
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-19
 *
 * This class contains the Member data
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // App Namespaces
    using Models.Dtos.Members;

    /// <summary>
    /// Member Holder
    /// </summary>
    public class Member {

        /// <summary>
        /// Member Data container
        /// </summary>
        public MemberDto? Info { get; set; }

    }

}