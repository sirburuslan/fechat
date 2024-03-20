/*
 * @class Profile Options Update Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the members options
 */

// Namespace for User Profile Controllers
namespace FeChat.Controllers.User.Profile {

    // Use the .NET methods to get DTO properties
    using System.Reflection;

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Authentication feature to get the access token
    using Microsoft.AspNetCore.Authentication;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;

    // Use the Dtos for response
    using FeChat.Models.Dtos;

    // Use the Dtos for members
    using FeChat.Models.Dtos.Members;

    // Use the entities
    using FeChat.Models.Entities.Members;

    // Use the general namespace fot strings
    using FeChat.Utils.General;    

    // Use the Repositories for database requests
    using FeChat.Utils.Interfaces.Repositories.Members;

    /// <summary>
    /// Members Options Update Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
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
        /// Gets the member information
        /// </summary>
        /// <param name="optionsDto">Contains the member's options</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Options list or error message</returns>
        [Authorize]
        [HttpPost("options")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Options([FromBody] OptionsDto optionsDto, Member memberInfo, IMembersRepository membersRepository) {

            // Get all members options
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Options to update container
            List<MemberOptionsEntity> optionsUpdate = new();

            // Saved options names
            List<string> optionsSaved = new();

            // Check if options exists
            if ( optionsList.Result != null ) {

                // Get options length
                int optionsLength = optionsList.Result.Count;

                // List the saved options
                for ( int o = 0; o < optionsLength; o++ ) {

                    // Get option's name
                    var optionName = typeof(OptionsDto).GetProperty(optionsList.Result[o].OptionName);

                    // Verify if option's name is not null
                    if ( optionName != null ) {

                        // Get the option's value
                        var optionValue = optionName!.GetValue(optionsDto);

                        // Verify if optionValue is not null
                        if ( optionValue == null ) {
                            continue;
                        }

                        // Save the option's name
                        optionsSaved.Add(optionsList.Result[o].OptionName);

                        // Create the option's params
                        MemberOptionsEntity optionUpdate = new() {
                            OptionId = optionsList.Result[o].OptionId,
                            MemberId = optionsList.Result[o].MemberId,
                            OptionName = optionsList.Result[o].OptionName,
                            OptionValue = optionValue!.ToString() ?? string.Empty
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionUpdate);

                    }
                    
                }

            }

            // Options to save container
            List<MemberOptionsEntity> optionsSave = new();

            // Get all options dto properties
            PropertyInfo[] propertyInfos = typeof(OptionsDto).GetProperties();

            // Get properties length
            int propertiesLength = propertyInfos.Length;

            // List the properties
            for ( int p = 0; p < propertiesLength; p++ ) {

                // If is MemberId continue
                if ( propertyInfos[p].Name == "MemberId" ) {
                    continue;
                }

                // If value is null continue
                if ( propertyInfos[p].GetValue(optionsDto) == null ) {
                    continue;
                }

                // Check if the option is not saved already
                if ( !optionsSaved.Contains(propertyInfos[p].Name) ) {

                    // Create the option
                    MemberOptionsEntity option = new() {
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = propertyInfos[p].Name,
                        OptionValue = propertyInfos[p].GetValue(optionsDto)!.ToString()
                    };

                    // Add option to the save list
                    optionsSave!.Add(option);

                }

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
                var response = new {
                    success = true,
                    message = new Strings().Get("MembersSettingsUpdated")
                };

                // Return a json
                return new JsonResult(response);

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = new Strings().Get("MembersSettingsNotUpdated")
                };

                // Return a json
                return new JsonResult(response);

            } 

        }

        /// <summary>
        /// Update the member profile
        /// </summary>
        /// <param name="memberDto">Contains the received information</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("profile")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberDto memberDto, Member memberInfo, IMembersRepository membersRepository) {     

            // Verify if email exists
            if ( memberDto.Email == null ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("EmailRequired")
                });

            }

            // Get the email
            ResponseDto<MemberDto> member = await membersRepository.GetMemberEmailAsync(memberDto);

            // Verify if a member was found
            if ( member.Result != null ) {

                // Check if the email is owned by another member
                if ( member.Result.MemberId != memberDto.MemberId ) {

                    // Return error response
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("EmailFound")
                    });                    

                }

            } else {

                // Member the member's data
                member = await membersRepository.GetMemberAsync(memberInfo.Info!.MemberId);

                // Verify if member exists
                if ( member.Result == null ) {

                    // Return a json
                    return new JsonResult(new {
                        success = false,
                        message = new Strings().Get("AccountNotFound")
                    });

                }

            }

            // Create the update
            MemberEntity memberData = new() {
                MemberId = memberInfo.Info!.MemberId,
                FirstName = memberDto.FirstName,
                LastName = memberDto.LastName,
                Email = memberDto.Email,
                Role = 1,
                Password = member.Result.Password,
                Created = member.Result.Created
            };

            // Update the member's data
            ResponseDto<bool> UpdateMember = await membersRepository.UpdateMemberAsync(memberData);

            // Verify if member exists
            if ( UpdateMember.Result ) {

                // Options to update container
                List<MemberOptionsEntity> optionsUpdate = new();

                // Options to save container
                List<MemberOptionsEntity> optionsSave = new();                

                // Get the member's settings
                ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info.MemberId);

                // Check if options exists
                if ( optionsList.Result != null ) {

                    // Get the Option ID
                    var memberPhone = optionsList.Result.FirstOrDefault(m => m.OptionName == "Phone");

                    // Check if the phone is saved
                    if ( memberPhone != null ) {

                        // Create the option's params
                        MemberOptionsEntity optionPhoneUpdate = new() {
                            OptionId = memberPhone.OptionId,
                            MemberId = memberInfo.Info.MemberId,
                            OptionName = "Phone",
                            OptionValue = memberDto.Phone
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionPhoneUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity optionPhone = new() {
                            MemberId = memberInfo.Info.MemberId,
                            OptionName = "Phone",
                            OptionValue = memberDto.Phone
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
                            MemberId = memberInfo.Info.MemberId,
                            OptionName = "Language",
                            OptionValue = memberDto.Language
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionLanguageUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity optionLanguage = new() {
                            MemberId = memberInfo.Info.MemberId,
                            OptionName = "Language",
                            OptionValue = memberDto.Language
                        };

                        // Add option to the save list
                        optionsSave!.Add(optionLanguage);

                    }

                    // Get the Option ID
                    var memberPlan = optionsList.Result.FirstOrDefault(m => m.OptionName == "PlanId");

                    // Check if the plan is saved
                    if ( memberPlan != null ) {

                        // Create the option's params
                        MemberOptionsEntity optionPlanUpdate = new() {
                            OptionId = memberPlan.OptionId,
                            MemberId = memberInfo.Info.MemberId,
                            OptionName = "PlanId",
                            OptionValue = memberDto.PlanId.ToString()
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionPlanUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity optionPlan = new() {
                            MemberId = memberInfo.Info.MemberId,
                            OptionName = "PlanId",
                            OptionValue = memberDto.PlanId.ToString()
                        };

                        // Add option to the save list
                        optionsSave!.Add(optionPlan);

                    }

                } else {

                    // Create the option
                    MemberOptionsEntity optionPhone = new() {
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "Phone",
                        OptionValue = memberDto.Phone
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionPhone);

                    // Create the option
                    MemberOptionsEntity optionLanguage = new() {
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "Language",
                        OptionValue = memberDto.Language
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionLanguage);

                    // Create the option
                    MemberOptionsEntity optionPlan = new() {
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "PlanId",
                        OptionValue = memberDto.PlanId.ToString()
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionPlan);                    

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
                        message = new Strings().Get("ChangesSaved")
                    };

                    // Return a json
                    return new JsonResult(updateResponse);

                } else {

                    // Create a error response
                    var saveResponse = new {
                        success = false,
                        message = new Strings().Get("ChangesNotSaved")
                    };

                    // Return a json
                    return new JsonResult(saveResponse);

                } 

            } else {

                // Create a error response
                var response = new {
                    success = false,
                    message = UpdateMember.Message ?? new Strings().Get("ChangesNotSaved")
                };

                // Return a json
                return new JsonResult(response);

            }

        }

        /// <summary>
        /// Update the member password
        /// </summary>
        /// <param name="memberDto">Contains the received information</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("profile/security")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateMemberPassword([FromBody] MemberDto memberDto, Member memberInfo, IMembersRepository membersRepository) {

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
                MemberId = memberInfo.Info!.MemberId,
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
        /// Update the member notifications settings
        /// </summary>
        /// <param name="memberDto">Contains the received information</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        /// <returns>Success or error message</returns>
        [Authorize]
        [HttpPost("notifications")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateNotificationsPassword([FromBody] MemberDto memberDto, Member memberInfo, IMembersRepository membersRepository) {
            
            // Options to update container
            List<MemberOptionsEntity> optionsUpdate = new();

            // Options to save container
            List<MemberOptionsEntity> optionsSave = new();                

            // Get the member's settings
            ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

            // Check if options exists
            if ( optionsList.Result != null ) {

                // Get the Option ID
                var memberNotificationsEnabled = optionsList.Result.FirstOrDefault(m => m.OptionName == "NotificationsEnabled");

                // Check if the option is saved
                if ( memberNotificationsEnabled != null ) {

                    // Create the option's params
                    MemberOptionsEntity optionNotificationsEnabled = new() {
                        OptionId = memberNotificationsEnabled.OptionId,
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "NotificationsEnabled",
                        OptionValue = memberDto.NotificationsEnabled.ToString()
                    };

                    // Add option in the update list
                    optionsUpdate.Add(optionNotificationsEnabled);

                } else {

                    // Create the option
                    MemberOptionsEntity optionNotificationsEnabled = new() {
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "NotificationsEnabled",
                        OptionValue = memberDto.NotificationsEnabled.ToString()
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionNotificationsEnabled);

                }

                // Get the Option ID
                var memberNotificationsEmail = optionsList.Result.FirstOrDefault(m => m.OptionName == "NotificationsEmail");

                // Check if the option is saved
                if ( memberNotificationsEmail != null ) {

                    // Create the option's params
                    MemberOptionsEntity optionNotificationsEmail = new() {
                        OptionId = memberNotificationsEmail.OptionId,
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "NotificationsEmail",
                        OptionValue = memberDto.NotificationsEmail
                    };

                    // Add option in the update list
                    optionsUpdate.Add(optionNotificationsEmail);

                } else {

                    // Create the option
                    MemberOptionsEntity optionNotificationsEmail = new() {
                        MemberId = memberInfo.Info.MemberId,
                        OptionName = "NotificationsEmail",
                        OptionValue = memberDto.NotificationsEmail
                    };

                    // Add option to the save list
                    optionsSave!.Add(optionNotificationsEmail);

                }

            } else {

                // Create the option
                MemberOptionsEntity optionNotificationsEnabled = new() {
                    MemberId = memberInfo.Info.MemberId,
                    OptionName = "NotificationsEnabled",
                    OptionValue = memberDto.NotificationsEnabled.ToString()
                };

                // Add option to the save list
                optionsSave!.Add(optionNotificationsEnabled);

                // Create the option
                MemberOptionsEntity optionNotificationsEmail = new() {
                    MemberId = memberInfo.Info.MemberId,
                    OptionName = "NotificationsEmail",
                    OptionValue = memberDto.NotificationsEmail
                };   
                
                // Add option to the save list
                optionsSave!.Add(optionNotificationsEmail);                               

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
                    message = new Strings().Get("ChangesSaved")
                };

                // Return a json
                return new JsonResult(updateResponse);

            } else {

                // Create a error response
                var saveResponse = new {
                    success = false,
                    message = new Strings().Get("ChangesNotSaved")
                };

                // Return a json
                return new JsonResult(saveResponse);

            } 

        }

        /// <summary>
        /// Update the member profile image
        /// </summary>
        /// <param name="file">Contains the uploaded file</param>
        /// <param name="memberInfo">Member logged information</param>
        /// <param name="membersRepository">Contains an instance to the Members repository</param>
        [Authorize]
        [HttpPost("image")]
        [EnableCors("AllowOrigin")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateMemberImage(IFormFile file, Member memberInfo, IMembersRepository membersRepository) {

            // Try to upload the file
            ResponseDto<StorageDto> uploadImage = await new ImageUpload().UploadAsync(_configuration, file);

            // Check if the file was uploaded
            if ( uploadImage.Result != null ) {

                // Options to update container
                List<MemberOptionsEntity> optionsUpdate = new();

                // Options to save container
                List<MemberOptionsEntity> optionsSave = new();                

                // Get the member's settings
                ResponseDto<List<OptionDto>> optionsList = await membersRepository.OptionsListAsync(memberInfo.Info!.MemberId);

                // Check if options exists
                if ( optionsList.Result != null ) {

                    // Get the Option ID
                    var profilePhoto = optionsList.Result.FirstOrDefault(m => m.OptionName == "ProfilePhoto");

                    // Check if the photo is saved
                    if ( profilePhoto != null ) {

                        // Create the option's params
                        MemberOptionsEntity optionUpdate = new() {
                            OptionId = profilePhoto.OptionId,
                            MemberId = memberInfo.Info!.MemberId,
                            OptionName = "ProfilePhoto",
                            OptionValue = uploadImage.Result.FileUrl
                        };

                        // Add option in the update list
                        optionsUpdate.Add(optionUpdate);

                    } else {

                        // Create the option
                        MemberOptionsEntity option = new() {
                            MemberId = memberInfo.Info!.MemberId,
                            OptionName = "ProfilePhoto",
                            OptionValue = uploadImage.Result.FileUrl
                        };

                        // Add option to the save list
                        optionsSave!.Add(option);

                    }

                } else {

                    // Create the option
                    MemberOptionsEntity option = new() {
                        MemberId = memberInfo.Info!.MemberId,
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
                        memberInfo.Info!.MemberId
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