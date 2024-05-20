using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Validators;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CustomEd.Shared.JWT;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using CustomEd.User.Service.Model;
using MassTransit;
using CustomEd.User.Contracts;
using CustomEd.Shared.JWT.Contracts;
using CusotmEd.User.Servce.DTOs;
using CustomEd.User.Teacher.Events;
using CustomEd.User.Contracts.Teacher.Events;
using CustomEd.Contracts.OtpService.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using CustomEd.User.Service.Services;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/teacher")]
    public class TeacherController : UserController<Model.Teacher>
    {
        public TeacherController(CloudinaryService cloudinaryService,IGenericRepository<ForgotPasswordOtp> forgotPasswordOtpRepository, IGenericRepository<Otp> otpRepository, IGenericRepository<Model.Teacher> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor) : base(cloudinaryService,forgotPasswordOtpRepository ,otpRepository, userRepository, mapper, passwordHasher, jwtService, publishEndpoint, httpContextAccessor)
        {
            
        }

        [HttpGet("teacher-name")]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> SearchTeacherByName([FromQuery] string name)
        {
            var teacher = await _userRepository.GetAsync(u => u.IsVerified && (u.FirstName!.Contains(name) || u.LastName!.Contains(name)));
            var teacherDto = _mapper.Map<TeacherDto>(teacher);
            return Ok(SharedResponse<TeacherDto>.Success(teacherDto, "Teacher retrieved successfully"));
        }
        [HttpPost]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> CreateUser([FromBody] CreateTeacherDto createTeacherDto)
        {
            var httpClient = new HttpClient();
            var url = $"https://customed-schoolmock.onrender.com/schooldb/getTeacherInfo?email={createTeacherDto.Email}";
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject(responseContent);
            var jsonData = (JObject)jsonResponse!;
            Console.WriteLine(jsonData == null);
            var fetchedTeacherInfo = jsonData!["data"];
            if(fetchedTeacherInfo!.Type == JTokenType.Null)
            {
                return BadRequest(SharedResponse<Model.Teacher>.Fail("Unallowed user", new List<string>()));
            }
            var name = fetchedTeacherInfo["name"]!.ToString().Split();
            var createTeacherDtoValidator = new CreateTeacherDtoValidator(_userRepository);
            var validationResult = await createTeacherDtoValidator.ValidateAsync(createTeacherDto);
            if (!validationResult.IsValid)
            {
                
                return BadRequest(SharedResponse<Model.Teacher>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            var passwordHash = _passwordHasher.HashPassword(createTeacherDto.Password);
            createTeacherDto.Password = passwordHash;
            var teacher = new Model.Teacher{
                Id = Guid.NewGuid(),
                Email = createTeacherDto.Email,
                Password = createTeacherDto.Password,
                FirstName = name[0], 
                LastName = name[1], 
                DateOfBirth = DateOnly.Parse(fetchedTeacherInfo["dateOfBirth"]!.ToString()), 
                Department = Enum.Parse<Department>(fetchedTeacherInfo["department"]!.ToString()), 
                PhoneNumber = fetchedTeacherInfo["phone"]!.ToString(), 
                JoinDate = DateOnly.Parse(fetchedTeacherInfo["joinDate"]!.ToString()),
            };
            teacher.Role = Model.Role.Teacher;
            
            await _userRepository.CreateAsync(teacher);
            var dto = _mapper.Map<TeacherDto>(teacher);
            var sendOtpEvent = new SendOtpEvent();
            sendOtpEvent.Email = teacher.Email;
            await _publishEndpoint.Publish(sendOtpEvent);
            return CreatedAtAction(nameof(GetUserById), new { id = teacher.Id }, SharedResponse<TeacherDto>.Success(dto, "User created successfully"));
            
        }

        // [Authorize]
        // [HttpDelete("{id}")]
        // public async Task<ActionResult<SharedResponse<Model.Teacher>>> RemoveUser(Guid id)
        // {
        //     if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
        //     {
        //         return BadRequest(SharedResponse<Model.Teacher>.Fail("Invalid Id", new List<string> { "Invalid id" }));
        //     }

        //     var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
        //     var currentUserId = identityProvider.GetUserId();
        //     if (currentUserId != id)
        //     {
        //         return Unauthorized(SharedResponse<Model.Teacher>.Fail("Unauthorized to delete user", null));
        //     }

        //     await _userRepository.RemoveAsync(id);
        //     var teacherDeletedEvent = new TeacherDeletedEvent{ Id = id};
        //     await _publishEndpoint.Publish(teacherDeletedEvent);
        //     return Ok(SharedResponse<Model.Teacher>.Success(null, "User deleted successfully"));
        // }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> UpdatePhoneAndDepartment([FromBody] UpdateTeacherDto updateTeacherDto)
        {
            ///FETCH THE DEPARTMENT OF THE STUDENT HERE
            var updateTeacherDtoValidator = new UpdateTeacherDtoValidator(_userRepository);
            var validationResult = await updateTeacherDtoValidator.ValidateAsync(updateTeacherDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<TeacherDto>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId  == Guid.Empty)
            {
                return Unauthorized(SharedResponse<TeacherDto>.Fail("Unauthorized to update user", null));
            }
            var user = await _userRepository.GetAsync(currentUserId);
            user.PhoneNumber = updateTeacherDto.PhoneNumber;
            await _userRepository.UpdateAsync(user);
            var teacherUpdatedEvent =_mapper.Map<TeacherUpdatedEvent>(user);
            await _publishEndpoint.Publish(teacherUpdatedEvent);
            var dto = _mapper.Map<TeacherDto>(user);
            return Ok(SharedResponse<TeacherDto>.Success(dto, "User updated successfully"));
            
        } 

        [HttpPost("login")]
        public override async Task<ActionResult<SharedResponse<UserDto>>> SignIn([FromBody] LoginRequestDto request)
        {
            return await base.SignIn(request);
        }

        // [HttpPost("sendOtpForForgetPasswordVerification")]
        // public async Task<ActionResult<SharedResponse<ForgotPasswordOtp>>> SendOtpForForgetPasswordVerification([FromBody] string Email)
        // {
        //     var teacher = _userRepository.GetAsync(x => x.Email == Email);
        //     if(teacher == null)
        //     {
        //          return BadRequest(SharedResponse<ForgotPasswordOtp>.Fail("Invalid input", new List<string>()));
        //     }
        //     var sendOtpEvent = new SendOtpEvent();
        //     sendOtpEvent.Email = Email;
        //     await _publishEndpoint.Publish(sendOtpEvent);
        //     return Ok(SharedResponse<ForgotPasswordOtp>.Success(new ForgotPasswordOtp(), "Otp code has been sent successfully." ));

        // }

        [HttpPost("sendOtpForForgotPassword")]
        public async Task<ActionResult<SharedResponse<bool>>> SendOtpForForgotPassword([FromBody] string Email)
        {
            var teacher = await _userRepository.GetAsync(t => t.Email == Email);
            if (teacher == null || teacher.IsVerified == false)
            {
                return BadRequest(SharedResponse<bool>.Fail("Unauthorized user", new List<string>()));
            }
            
            await _publishEndpoint.Publish(new SendOtpEvent{
                Email = Email
            });
            return Ok(SharedResponse<bool>.Success(true, $"Otp code is sent to {Email}. Verify before 30 mins." ));
        }

        [HttpPost("verifyOtpForForgotPassword")]
        public async Task<ActionResult<SharedResponse<ForgotPasswordOtp>>> VerifyOtpForForgotPassword([FromBody] VerifyPasswordForForgotPasswordDto verifyPasswordForForgotPasswordDto)
        {
            ///FETCH THE DEPARTMENT OF THE STUDENT HERE
            var verifyPasswordForForgotPasswordDtoValidator = new VerifyPasswordForForgotPasswordDtoValidator(_forgotPasswordOtpRepository);
            var validationResult = await verifyPasswordForForgotPasswordDtoValidator.ValidateAsync(verifyPasswordForForgotPasswordDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<ForgotPasswordOtp>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            
            var forgotPasswordItem = await _forgotPasswordOtpRepository.GetAsync(x => x.Email == verifyPasswordForForgotPasswordDto.Email);
            if (forgotPasswordItem.OtpCode == verifyPasswordForForgotPasswordDto.OtpCode && DateTime.UtcNow.Subtract(forgotPasswordItem.UpdatedAt).TotalMinutes <= 30)
            {
                forgotPasswordItem.Allowed = true;
                await _forgotPasswordOtpRepository.UpdateAsync(forgotPasswordItem);
                return Ok(SharedResponse<ForgotPasswordOtp>.Success(forgotPasswordItem, "Otp code verified successfully. You can now change your password." ));
            }
            return BadRequest(SharedResponse<ForgotPasswordOtp>.Fail("Invalid OTP code or expired", new List<string> { "Invalid OTP code or expired" }));
        }
        [HttpPut("changePassword")]
        public async Task<ActionResult<SharedResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            ///FETCH THE DEPARTMENT OF THE STUDENT HERE
            var verifyPasswordForForgotPasswordDtoValidator = new ChangePasswordRequestDtoValidator(_forgotPasswordOtpRepository);
            var validationResult = await verifyPasswordForForgotPasswordDtoValidator.ValidateAsync(changePasswordRequestDto);
            var forgotPasswordItem = await _forgotPasswordOtpRepository.GetAsync(x => x.Email == changePasswordRequestDto.Email);
            if (!validationResult.IsValid || DateTime.UtcNow.Subtract(forgotPasswordItem.UpdatedAt).TotalMinutes > 30 || forgotPasswordItem.Allowed == false)
            {
                return BadRequest(SharedResponse<bool>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            forgotPasswordItem.Allowed = false;
            await _forgotPasswordOtpRepository.UpdateAsync(forgotPasswordItem);
            var passwordHash = _passwordHasher.HashPassword(changePasswordRequestDto.NewPassword);
            changePasswordRequestDto.NewPassword = passwordHash;
            var teacher = await _userRepository.GetAsync(t => t.Email == changePasswordRequestDto.Email);
            teacher.Password = changePasswordRequestDto.NewPassword;
            await _userRepository.UpdateAsync(teacher);
            return Ok(SharedResponse<bool>.Success(true, "Your password has been changed."));

        }

        [Authorize]
        [HttpPost("uploadImage")]
        public async Task<ActionResult<SharedResponse<String?>>> UploadImage(IFormFile image)
        {
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return NotFound(SharedResponse<String?>.Fail("User not found", null));
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest(SharedResponse<String?>.Fail("Invalid file", null));
            }
            if(user.ImageUrl != null)
            {
                await _cloudinaryService.DeleteImage(user.ImageUrl);
            }
            var imageUrl=await _cloudinaryService.UploadImage(image);
            if(imageUrl == null)
            {
                BadRequest(SharedResponse<String?>.Fail("Failed to upload to the cloud", null));
            }

            user.ImageUrl = imageUrl;
            await _userRepository.UpdateAsync(user);
            return Ok(SharedResponse<String>.Success(imageUrl, "Your picture is uploaded successfully.")); 
        }
        
        [Authorize]
        [HttpDelete("deleteImage")]
        public async Task<ActionResult<SharedResponse<bool>>> DeleteImage()
        {
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return NotFound(SharedResponse<bool>.Fail("User not found", null));
            } 
            if (user.ImageUrl == null)
            {
                return Ok(SharedResponse<bool>.Success(false, "No picture to delete.")); 
            }
            await _cloudinaryService.DeleteImage(user.ImageUrl);
            user.ImageUrl = null;
            await _userRepository.UpdateAsync(user);
            return Ok(SharedResponse<bool>.Success(true, "Your picture is deleted successfully.")); 
        }

        [HttpGet("getPicture/{userId}")]
        public async Task<ActionResult<SharedResponse<String?>>> GetPicture(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                BadRequest(SharedResponse<String?>.Fail("User doesn't exist", null));
            }
            return Ok(SharedResponse<String>.Success(user.ImageUrl, "Image url fetched successfully.")); 
        }

    }
}
