/*
 * @class Members Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to manage the member's data
 */

// Namespace for Members repositories
namespace FeChat.Models.Repositories.Members {

    // Use the Memory Cache to story the data in cache
    using Microsoft.Extensions.Caching.Memory;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use Dtos for Members
    using FeChat.Models.Dtos.Members;

    // Use the Entities
    using FeChat.Models.Entities.Members;

    // Get the Configuration Utils for connection
    using FeChat.Utils.Configuration;

    // Use the IMembersRepository interface
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Members Repository pattern
    /// </summary>
    public class MembersRepository: IMembersRepository {

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
        public MembersRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        } 

        /// <summary>
        /// Create a member
        /// </summary>
        /// <param name="newMemberDto">Member dto with the member's data</param>
        /// <returns>Response with member data</returns>
        public async Task<ResponseDto<MemberDto>> CreateMemberAsync(NewMemberDto newMemberDto) {

            // Init Create Repository
            Account.CreateRepository createRepository = new(_memoryCache, _context);

            // Create a member and return the response
            return await createRepository.CreateMemberAsync(newMemberDto);

        }

        /// <summary>
        /// Update a member
        /// </summary>
        /// <param name="memberDto">Member entity with the member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdateMemberAsync(MemberEntity memberDto) {

            // Init Update Repository
            Account.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update a member and return the response
            return await updateRepository.UpdateMemberAsync(memberDto);

        }

        /// <summary>
        /// Update a member email
        /// </summary>
        /// <param name="memberDto">Member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdateEmailAsync(MemberDto memberDto) {

            // Init Update Repository
            Account.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update a member's email and return the response
            return await updateRepository.UpdateEmailAsync(memberDto);

        }

        /// <summary>
        /// Update a member password
        /// </summary>
        /// <param name="memberDto">Member's data</param>
        /// <returns>Return response bool and with message if errors is catched</returns>
        public async Task<ResponseDto<bool>> UpdatePasswordAsync(MemberDto memberDto) {

            // Init Update Repository
            Account.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update a member's password and return the response
            return await updateRepository.UpdatePasswordAsync(memberDto);

        }

        /// <summary>
        /// Check if the member and password is correct
        /// </summary>
        /// <param name="memberDto">Member dto with the member's data</param>
        /// <returns>Response with member data</returns>
        public async Task<ResponseDto<MemberDto>> SignIn(MemberDto memberDto) {

            // Init Auth Repository
            Account.AuthRepository authRepository = new(_context);

            // Authentificate member and return the response
            return await authRepository.SignIn(memberDto);

        }

        /// <summary>
        /// Gets all members
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <returns>List with members</returns>
        public async Task<ResponseDto<ElementsDto<MemberDto>>> GetMembersAsync(SearchDto searchDto) {

            // Init Read Repository
            Account.ReadRepository readRepository = new(_memoryCache, _context);

            // Get members and return the response
            return await readRepository.GetMembersAsync(searchDto);

        }

        /// <summary>
        /// Gets all members by time
        /// </summary>
        /// <param name="time">Time filter</param>
        /// <returns>List with members</returns>
        public async Task<ResponseDto<object>> GetMembersByTimeAsync(int time) {

            // Init Read Repository
            Account.ReadRepository readRepository = new(_memoryCache, _context);

            // Gets members by time for graph and return the response
            return await readRepository.GetMembersByTimeAsync(time);

        }

        /// <summary>
        /// Gets all members for export
        /// </summary>
        /// <returns>List with all members</returns>
        public async Task<ResponseDto<List<MemberDto>>> GetMembersForExportAsync() {

            // Init Read Repository
            Account.ReadRepository readRepository = new(_memoryCache, _context);

            // Get all members and return the response
            return await readRepository.GetMembersForExportAsync();

        }

        /// <summary>
        /// Get member data
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>MemberDto with member's data</returns>
        public async Task<ResponseDto<MemberDto>> GetMemberAsync(int memberId) {

            // Init Read Repository
            Account.ReadRepository readRepository = new(_memoryCache, _context);

            // Get member by id and return the response
            return await readRepository.GetMemberAsync(memberId);

        }

        /// <summary>
        /// Get member email
        /// </summary>
        /// <param name="memberDto">Member data</param>
        /// <returns>Member with email if exists</returns>
        public async Task<ResponseDto<MemberDto>> GetMemberEmailAsync(MemberDto memberDto) {

            // Init Read Repository
            Account.ReadRepository readRepository = new(_memoryCache, _context);

            // Get member by email and return the response
            return await readRepository.GetMemberEmailAsync(memberDto);

        }

        /// <summary>
        /// Get member by reset code
        /// </summary>
        /// <param name="code">Reset code</param>
        /// <returns>Member with email if exists</returns>
        public async Task<ResponseDto<MemberDto>> GetMemberByCodeAsync(string code) {

            // Init Read Repository
            Account.ReadRepository readRepository = new(_memoryCache, _context);

            // Get member by reset code and return the response
            return await readRepository.GetMemberByCodeAsync(code);

        }

        /// <summary>
        /// Delete member
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>Bool true if the member was deleted</returns>
        public async Task<ResponseDto<bool>> DeleteMemberAsync(int memberId) {

            // Init Delete Repository
            Account.DeleteRepository deleteRepository = new(_memoryCache, _context);

            // Get member by reset code and return the response
            return await deleteRepository.DeleteMemberAsync(memberId);

        }

        /// <summary>
        /// Save bulk options for Members
        /// </summary>
        /// <param name="optionsList">Members options list</param>
        /// <returns>Boolean response</returns>
        public async Task<bool> SaveOptionsAsync(List<MemberOptionsEntity> optionsList) {

            // Init Create Repository
            Options.CreateRepository createRepository = new(_memoryCache, _context);

            // Save bulk options and return the response
            return await createRepository.SaveOptionsAsync(optionsList);            

        }

        /// <summary>
        /// Update bulk options
        /// </summary>
        /// <param name="optionsList">Members options list</param>
        /// <returns>Boolean response</returns>
        public bool UpdateOptionsAsync(List<MemberOptionsEntity> optionsList) {

            // Init Update Repository
            Options.UpdateRepository updateRepository = new(_memoryCache, _context);

            // Update bulk options and return the response
            return updateRepository.UpdateOptions(optionsList); 

        }

        /// <summary>
        /// Get the options list
        /// </summary>
        /// <param name="MemberId">Member's ID</param>
        /// <returns>Get the options list</returns>
        public async Task<ResponseDto<List<OptionDto>>> OptionsListAsync(int MemberId) {

            // Init Read Repository
            Options.ReadRepository readRepository = new(_memoryCache, _context);

            // Read options and return the response
            return await readRepository.OptionsListAsync(MemberId); 

        }

        /// <summary>
        /// Get Member Option with Google ID
        /// </summary>
        /// <param name="googleId">Google ID</param>
        /// <returns>Option of the user or error message</returns>
        public async Task<ResponseDto<OptionDto>> GetMemberOptionWithGoogle(string googleId) {

            // Init Read Repository
            Options.ReadRepository readRepository = new(_memoryCache, _context);

            // Read option and return the response
            return await readRepository.GetMemberOptionWithGoogle(googleId);

        }

    }

}