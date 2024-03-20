/*
 * @interface Members Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This interface is implemented in MembersRepository
 */

// Namespace for the Utilis Interfaces Members Repositories
namespace FeChat.Utils.Interfaces.Repositories.Members {

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use Dtos for Members
    using FeChat.Models.Dtos.Members;

    // Use Entity for Members
    using FeChat.Models.Entities.Members;

    /// <summary>
    /// Members Interface
    /// </summary>
    public interface IMembersRepository {

        /// <summary>
        /// Create a member
        /// </summary>
        /// <param name="newMemberDto">RegistrationDto dto with the member's data</param>
        /// <returns>Response with member data</returns>
        Task<ResponseDto<MemberDto>> CreateMemberAsync(NewMemberDto newMemberDto);

        /// <summary>
        /// Update a member
        /// </summary>
        /// <param name="Member">Member entity with the member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        Task<ResponseDto<bool>> UpdateMemberAsync(MemberEntity Member); 

        /// <summary>
        /// Update a member email
        /// </summary>
        /// <param name="Member">Member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        Task<ResponseDto<bool>> UpdateEmailAsync(MemberDto Member);

        /// <summary>
        /// Update a member password
        /// </summary>
        /// <param name="Member">Member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        Task<ResponseDto<bool>> UpdatePasswordAsync(MemberDto Member);        

        /// <summary>
        /// Check if the member and password is correct
        /// </summary>
        /// <param name="Member">SignInDto dto with the member's data</param>
        /// <returns>Response with member data</returns>
        Task<ResponseDto<MemberDto>> SignIn(MemberDto Member);    

        /// <summary>
        /// Gets all members
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with members</returns>
        Task<ResponseDto<ElementsDto<MemberDto>>> GetMembersAsync(SearchDto searchDto);

        /// <summary>
        /// Gets all members by time
        /// </summary>
        /// <param name="time">Time filter</param>
        /// <returns>List with members</returns>
        Task<ResponseDto<object>> GetMembersByTimeAsync(int time);

        /// <summary>
        /// Gets all members for export
        /// </summary>
        /// <returns>List with all members</returns>
        Task<ResponseDto<List<MemberDto>>> GetMembersForExportAsync();

        /// <summary>
        /// Get member data
        /// </summary>
        /// <param name="MemberId">Member Id</param>
        /// <returns>Member's data if member exists</returns>
        Task<ResponseDto<MemberDto>> GetMemberAsync(int MemberId);

        /// <summary>
        /// Get member email
        /// </summary>
        /// <param name="memberDto">Member data</param>
        /// <returns>Member with email if exists</returns>
        Task<ResponseDto<MemberDto>> GetMemberEmailAsync(MemberDto memberDto);

        /// <summary>
        /// Get member by reset code
        /// </summary>
        /// <param name="code">Reset code</param>
        /// <returns>Member with email if exists</returns>
        Task<ResponseDto<MemberDto>> GetMemberByCodeAsync(string code);

        /// <summary>
        /// Delete member
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>Bool true if the member was deleted</returns>
        Task<ResponseDto<bool>> DeleteMemberAsync(int memberId);

        /// <summary>
        /// Save bulk options for Members
        /// </summary>
        /// <param name="optionsList">Members options list</param>
        /// <returns>Boolean response</returns>
        Task<bool> SaveOptionsAsync(List<MemberOptionsEntity> optionsList);

        /// <summary>
        /// Update bulk options
        /// </summary>
        /// <param name="optionsList">Members options list</param>
        /// <returns>Boolean response</returns>
        bool UpdateOptionsAsync(List<MemberOptionsEntity> optionsList);

        /// <summary>
        /// Get the options list
        /// </summary>
        /// <param name="MemberId">Member's ID</param>
        /// <returns>Get the options list</returns>
        Task<ResponseDto<List<OptionDto>>> OptionsListAsync(int MemberId);

        /// <summary>
        /// Get Member Option with Google ID
        /// </summary>
        /// <param name="googleId">Google ID</param>
        /// <returns>Option of the user or error message</returns>
        Task<ResponseDto<OptionDto>> GetMemberOptionWithGoogle(string googleId);

    }

}