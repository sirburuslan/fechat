/*
 * @class Members Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the members information
 */

// Namespace for the Administrator Members Update Composer
namespace FeChat.Controllers.Administrator.Members {

    // Use the Asp Net core MVC
    using Microsoft.AspNetCore.Mvc;

    // Use the Authentication feature to get the access token
    using Microsoft.AspNetCore.Authentication;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the General Utils classes for Strings
    using FeChat.Utils.General;
    
    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use the Plans Dtos
    using FeChat.Models.Dtos.Plans;

    // Use the Dtos for Subscriptions
    using FeChat.Models.Dtos.Subscriptions;

    // Use the Entities for Members
    using FeChat.Models.Entities.Members;

    // Use the Repositories for Members
    using FeChat.Utils.Interfaces.Repositories.Members;

    // Use the Repositories for Plans
    using FeChat.Utils.Interfaces.Repositories.Plans;

    // Use the Repositories for Subscriptions
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Members Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/members")]
    public class UpdateController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public UpdateController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Update the member profile
        /// </summary>
        /// <param name="updateMemberDto">Contains the received information</param>
        /// <param name="MemberId">Contains the member's ID</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <param name="subscriptionsRepository">Contains an instance to the Subscriptions repository</param>
        /// <param name="plansRepository">Contains an instance to the Plans repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("{MemberId}")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateMemberProfile([FromBody] UpdateMemberDto updateMemberDto, int MemberId, Member memberInfo, IMembersRepository membersRepository, ISubscriptionsRepository subscriptionsRepository, IPlansRepository plansRepository) {      

            // Verify if email exists
            if ( updateMemberDto.Email == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("EmailRequired")
                });

            }

            // Create member email object for member request
            MemberDto memberEmail = new() {
                Email = updateMemberDto.Email
            };

            // Get the email
            ResponseDto<MemberDto> member = await membersRepository.GetMemberEmailAsync(memberEmail);

            // Verify if a member was found
            if ( member.Result != null ) {

                // Check if the email is owned by another member
                if ( member.Result.MemberId != updateMemberDto.MemberId ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("EmailFound")
                    });                    

                }

            } else {

                // Member the member's data
                member = await membersRepository.GetMemberAsync(MemberId);

                // Verify if member exists
                if ( member.Result == null ) {

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("AccountNotFound")
                    });

                }

            }

            // Check if the administrator tries to update his account
            if ( MemberId == memberInfo.Info!.MemberId ) {

                // Check if the role is not 0
                if ( updateMemberDto.Role != 0 ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("AdministratorCanNotUpdateRole")
                    });

                }
                
            }

            // Create the update
            MemberEntity memberData = new() {
                MemberId = updateMemberDto.MemberId,
                FirstName = updateMemberDto.FirstName,
                LastName = updateMemberDto.LastName,
                Email = updateMemberDto.Email,
                Role = updateMemberDto.Role,
                Password = member.Result.Password,
                Created = member.Result.Created
            };

            // Update the member's data
            ResponseDto<bool> UpdateMember = await membersRepository.UpdateMemberAsync(memberData);

            // Verify if member exists
            if ( UpdateMember.Result ) {

                // Verify if role is user and plan is not 0
                if ( (updateMemberDto.Role == 1) && (updateMemberDto.PlanId > 0) ) {

                    // Get the plan's data
                    ResponseDto<PlanDto> planDto = await plansRepository.GetPlanAsync(updateMemberDto.PlanId);

                    // Verify if plan exists
                    if ( planDto.Result == null ) {

                        // Return error response
                        return new JsonResult(new {
                            success = false,
                            message = new Strings().Get("PlanNotFound")
                        });

                    }

                    // Get the user's subscription
                    ResponseDto<SubscriptionDto> subscriptionResponse = await subscriptionsRepository.GetSubscriptionByMemberIdAsync(updateMemberDto.MemberId);

                    // Verify if subscription exists
                    if ( subscriptionResponse.Result != null ) {

                        // Verify if the plan is different
                        if ( updateMemberDto.PlanId != subscriptionResponse.Result.PlanId ) {

                            // Get the current date and time
                            DateTimeOffset currentDate = DateTimeOffset.UtcNow;

                            // Add 30 days to the current date
                            DateTimeOffset after30Days = currentDate.AddDays(30);

                            // Create the subscription
                            SubscriptionDto subscriptionDto = new() {
                                SubscriptionId = subscriptionResponse.Result.SubscriptionId,
                                MemberId = updateMemberDto.MemberId,
                                PlanId = updateMemberDto.PlanId,
                                OrderId = "",
                                NetId = "",
                                Source = "",
                                Expiration = (int)after30Days.ToUnixTimeSeconds(),
                                Created = subscriptionResponse.Result.Created
                            };

                            // Update the subscription
                            ResponseDto<bool> updateSubscription = await subscriptionsRepository.UpdateSubscriptionAsync(subscriptionDto);

                            // Check if the subscription wasn't updated successfully
                            if ( updateSubscription.Result == false ) {

                                // Return error response
                                return new JsonResult(new {
                                    success = false,
                                    message = updateSubscription.Message
                                });

                            }

                        }

                    } else {

                        // Get the current date and time
                        DateTimeOffset currentDate = DateTimeOffset.UtcNow;

                        // Add 30 days to the current date
                        DateTimeOffset after30Days = currentDate.AddDays(30);

                        // Create the subscription
                        SubscriptionDto subscriptionDto = new() {
                            MemberId = updateMemberDto.MemberId,
                            PlanId = updateMemberDto.PlanId,
                            OrderId = "",
                            NetId = "",
                            Source = "",
                            Expiration = (int)after30Days.ToUnixTimeSeconds()
                        };

                        // Save the subscription
                        ResponseDto<SubscriptionDto> saveSubscription = await subscriptionsRepository.CreateSubscriptionAsync(subscriptionDto);

                        // Check if the subscription wasn't saved successfully
                        if ( saveSubscription.Result == null ) {

                            // Return error response
                            return new JsonResult(new {
                                success = false,
                                message = saveSubscription.Message
                            });

                        }

                    }

                }

                // Options to update container
                List<MemberOptionsEntity> optionsUpdate = new();

                // Options to save container
                List<MemberOptionsEntity> optionsSave = new();                

                // Get the member's settings
                ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(MemberId);

                // Check if options exists
                if ( optionsList.Result != null ) {

                    // Get the Option ID
                    var memberPhone = optionsList.Result.FirstOrDefault(m => m.OptionName == "Phone");

                    // Check if the phone is saved
                    if ( memberPhone != null ) {

                        // Create the option's params
                        MemberOptionsEntity optionPhoneUpdate = new() {
                            OptionId = memberPhone.OptionId,
                            MemberId = MemberId,
                            OptionName = "Phone",
                            OptionValue = updateMemberDto.Phone
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionPhoneUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity optionPhone = new() {
                            MemberId = MemberId,
                            OptionName = "Phone",
                            OptionValue = updateMemberDto.Phone
                        };

                        // Add option to the save list
                        optionsSave!.Add(optionPhone);

                    }

                    // Get the Option ID
                    var memberLanguage = optionsList.Result.FirstOrDefault(m => m.OptionName == "Language");

                    // Check if the language is saved
                    if ( memberLanguage != null ) {

                        // Create the option's params
                        MemberOptionsEntity optionLanguageUpdate = new() {
                            OptionId = memberLanguage.OptionId,
                            MemberId = MemberId,
                            OptionName = "Language",
                            OptionValue = updateMemberDto.Language
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionLanguageUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity optionLanguage = new() {
                            MemberId = MemberId,
                            OptionName = "Language",
                            OptionValue = updateMemberDto.Language
                        };

                        // Add option to the save list
                        optionsSave!.Add(optionLanguage);

                    }

                } else {

                    // Create the option
                    MemberOptionsEntity optionPhone = new() {
                        MemberId = MemberId,
                        OptionName = "Phone",
                        OptionValue = updateMemberDto.Phone
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionPhone);

                    // Create the option
                    MemberOptionsEntity optionLanguage = new() {
                        MemberId = MemberId,
                        OptionName = "Language",
                        OptionValue = updateMemberDto.Language
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionLanguage);                

                }

                // Errors counter
                int errors = 0;

                // Verify if options for updating exists
                if ( optionsUpdate.Count > 0 ) {

                    // Update options
                    bool update_options = membersRepository.UpdateOptionsAsync(optionsUpdate);

                    // Check if an error has been occurred when the options were updated
                    if ( !update_options ) {
                        errors++;
                    }
                    
                } 

                // Verify if options for saving exists
                if ( optionsSave.Count > 0 ) {

                    // Save options
                    bool save_options = await membersRepository.SaveOptionsAsync(optionsSave);

                    // Check if an error has been occurred when the options were saved
                    if ( !save_options ) {
                        errors++;
                    }

                }

                // Verify if no errors occurred
                if ( errors == 0 ) {

                    // Create a success response
                    var updateResponse = new {
                        success = true,
                        message = new Strings().Get("MemberUpdated")
                    };

                    // Return a json
                    return new JsonResult(updateResponse);

                } else {

                    // Create a error response
                    var saveResponse = new {
                        success = false,
                        message = new Strings().Get("MemberNotUpdated")
                    };

                    // Return a json
                    return new JsonResult(saveResponse);

                } 

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = UpdateMember.Message ?? new Strings().Get("MemberNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            }

        }

        /// <summary>
        /// Update the member password
        /// </summary>
        /// <param name="memberDto">Contains the received information</param>
        /// <param name="MemberId">Contains the member's ID</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPut("{MemberId}")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateMemberPassword([FromBody] MemberDto memberDto, int MemberId, IMembersRepository membersRepository) {

            // Verify if password exists
            if ( memberDto.Password == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PleaseEnterPassword")
                });

            } 
            
            // Verify if repeat password exists
            if ( memberDto.RepeatPassword == null ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PleaseEnterRepeatPassword")
                });

            }  

            // Verify if the password is correct
            if ( memberDto.Password != memberDto.RepeatPassword ) {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("PasswordRepeatPasswordNotMatch")
                });
                
            } 

            // Create the member data
            MemberDto memberData = new() {
                MemberId = MemberId,
                Password = memberDto.Password.Trim()
            };

            // Member the member's data
            ResponseDto<bool> UpdateMember = await membersRepository.UpdatePasswordAsync(memberData);

            // Verify if member exists
            if ( UpdateMember.Result ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    message = new Strings().Get("PasswordWasSaved")
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = UpdateMember.Message ?? new Strings().Get("PasswordWasNotSaved")
                });

            }

        }

        /// <summary>
        /// Update the member profile image
        /// </summary>
        /// <param name="file">Contains the uploaded file</param>
        /// <param name="MemberId">Contains the member's ID</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("image/{MemberId}")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateMemberImage(IFormFile file, int MemberId, IMembersRepository membersRepository) {

            // Try to upload the file
            ResponseDto<StorageDto> uploadImage = await new ImageUpload().UploadAsync(_configuration, file);

            // Check if the file was uploaded
            if ( uploadImage.Result != null ) {

                // Options to update container
                List<MemberOptionsEntity> optionsUpdate = new();

                // Options to save container
                List<MemberOptionsEntity> optionsSave = new();                

                // Get the member's settings
                ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(MemberId);

                // Check if options exists
                if ( optionsList.Result != null ) {

                    // Get the Option ID
                    var profilePhoto = optionsList.Result.FirstOrDefault(m => m.OptionName == "ProfilePhoto");

                    // Check if the photo is saved
                    if ( profilePhoto != null ) {

                        // Create the option's params
                        MemberOptionsEntity optionUpdate = new() {
                            OptionId = profilePhoto.OptionId,
                            MemberId = MemberId,
                            OptionName = "ProfilePhoto",
                            OptionValue = uploadImage.Result.FileUrl
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity option = new() {
                            MemberId = MemberId,
                            OptionName = "ProfilePhoto",
                            OptionValue = uploadImage.Result.FileUrl
                        };

                        // Add option to the save list
                        optionsSave!.Add(option);

                    }

                } else {

                    // Create the option
                    MemberOptionsEntity option = new() {
                        MemberId = MemberId,
                        OptionName = "ProfilePhoto",
                        OptionValue = uploadImage.Result.FileUrl
                    };

                    // Add option to the save list
                    optionsSave!.Add(option);

                }

                // Errors counter
                int errors = 0;

                // Verify if options for updating exists
                if ( optionsUpdate.Count > 0 ) {

                    // Update options
                    bool update_options = membersRepository.UpdateOptionsAsync(optionsUpdate);

                    // Check if an error has been occurred when the options were updated
                    if ( !update_options ) {
                        errors++;
                    }
                    
                } 

                // Verify if options for saving exists
                if ( optionsSave.Count > 0 ) {

                    // Save options
                    bool save_options = await membersRepository.SaveOptionsAsync(optionsSave);

                    // Check if an error has been occurred when the options were saved
                    if ( !save_options ) {
                        errors++;
                    }

                }

                // Verify if no errors occurred
                if ( errors == 0 ) {

                    // Create a success response
                    var updateResponse = new {
                        success = true,
                        message = new Strings().Get("MemberPhotoUpdated"),
                        MemberId
                    };

                    // Return a json
                    return new JsonResult(updateResponse);

                } else {

                    // Create a error response
                    var saveResponse = new {
                        success = false,
                        message = new Strings().Get("MemberPhotoNotUpdated")
                    };

                    // Return a json
                    return new JsonResult(saveResponse);

                }

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = uploadImage.Message
                });

            }

        }

    }

}