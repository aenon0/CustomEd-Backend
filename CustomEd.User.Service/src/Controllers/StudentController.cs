using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.DTOs;
using AutoMapper;
using CustomEd.User.Service.Validators;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using MassTransit;
using CustomEd.Shared.JWT.Contracts;
using CusotmEd.User.Servce.DTOs;
using CustomEd.User.Student.Events;
using CustomEd.Shared.JWT;
using CustomEd.User.Service.Model;
using CustomEd.Contracts.OtpService.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomEd.User.Service.Services;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/student")]
    public class StudentController : UserController<Model.Student>
    {

        public StudentController(CloudinaryService cloudinaryService, IGenericRepository<ForgotPasswordOtp> forgotPasswordOtpRepository, IGenericRepository<Otp> otpRepository, IGenericRepository<Model.Student> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor) : base(cloudinaryService, forgotPasswordOtpRepository, otpRepository, userRepository, mapper, passwordHasher, jwtService, publishEndpoint, httpContextAccessor)
        {
        }


        [HttpGet("student-id")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> SearchStudentBySchoolId([FromQuery] string id)
        {
            var student = await _userRepository.GetAsync(u => u.StudentId == id && u.IsVerified == true) ;
            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(SharedResponse<StudentDto>.Success(studentDto, "Students retrieved successfully"));
        }

        [HttpGet("student-name")]
        public async Task<ActionResult<SharedResponse<IEnumerable<StudentDto>>>> SearchStudentByName([FromQuery] string name)
        {
            var students = await _userRepository.GetAllAsync(u => u.IsVerified && (u.FirstName!.Contains(name) || u.LastName!.Contains(name)));
            var studentsDto = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Ok(SharedResponse<IEnumerable<StudentDto>>.Success(studentsDto, "Students retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<SharedResponse<Model.Student>>> CreateUser([FromBody] CreateStudentDto studentDto)
        {
            var httpClient = new HttpClient();
            var url = $"https://customed-schoolmock.onrender.com/schooldb/getStudentInfo?email={studentDto.Email}";
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject(responseContent);
            var jsonData = (JObject)jsonResponse!;
            Console.WriteLine(jsonData == null);
            var fetchedStudentInfo = jsonData!["data"];
            if(fetchedStudentInfo!.Type == JTokenType.Null)
            {
                return BadRequest(SharedResponse<Model.Teacher>.Fail("Unallowed user", new List<string>()));
            }
            var name = fetchedStudentInfo["name"]!.ToString().Split();
            Console.WriteLine(fetchedStudentInfo);
            var createStudentDtoValidator = new CreateStudentDtoValidator(_userRepository);
            var validationResult = await createStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<Model.Student>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;

            var student = new Model.Student{
                Id = Guid.NewGuid(),
                Email = studentDto.Email,
                Password = studentDto.Password,
                StudentId = fetchedStudentInfo["studentId"]!.ToString(),
                FirstName =  name[0], 
                LastName =  name[1], 
                DateOfBirth = DateOnly.Parse(fetchedStudentInfo["dateOfBirth"]!.ToString()), 
                Department = Enum.Parse<Department>(fetchedStudentInfo["department"]!.ToString()), 
                PhoneNumber = fetchedStudentInfo["phone"]!.ToString(),
                JoinDate = DateOnly.Parse(fetchedStudentInfo["joinDate"]!.ToString()), 
                Year = int.Parse(fetchedStudentInfo["year"]!.ToString()), 
                Section = fetchedStudentInfo["section"]!.ToString()
            };
            student.Role = Model.Role.Student;

            await _userRepository.CreateAsync(student);

            var sendOtpEvent = new SendOtpEvent();
            sendOtpEvent.Email = student.Email;
            await _publishEndpoint.Publish(sendOtpEvent);
            return CreatedAtAction(nameof(GetUserById), new { id = student.Id }, SharedResponse<Model.Student>.Success(student, "User created successfully"));
        }


        // [Authorize]
        // [HttpDelete("{id}")]
        // public async Task<ActionResult<SharedResponse<StudentDto>>> RemoveUser(Guid id)
        // {
        //     if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
        //     {
        //         return BadRequest(SharedResponse<StudentDto>.Fail("Invalid Id", new List<string> { "Invalid id" }));
        //     }
        //     var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
        //     var currentUserId = identityProvider.GetUserId();
        //     if(currentUserId != id)
        //     {
        //         return Unauthorized(SharedResponse<StudentDto>.Fail("Unauthorized", new List<string> { "Unauthorized" }));
        //     }


        //     await _userRepository.RemoveAsync(id);
        //     var studentDeletedEvent = new StudentDeletedEvent{Id = id};
        //     await _publishEndpoint.Publish(studentDeletedEvent);
        //     return Ok(SharedResponse<StudentDto>.Success(null, "User deleted successfully"));
        // }

        [Authorize]
        [HttpPut("updatePhoneAndDepartment")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> UpdatePhoneAndDepartment([FromBody] UpdateStudentDto studentDto)
        {
            ///FETCH THE DEPARTMENT OF THE STUDENT HERE
            var updateStudentDtoValidator = new UpdateStudentDtoValidator(_userRepository);
            var validationResult = await updateStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<StudentDto>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if(currentUserId == Guid.Empty)
            {
                return Unauthorized(SharedResponse<StudentDto>.Fail("Unauthorized", new List<string> { "Unauthorized" }));
            }
            var student = await _userRepository.GetAsync(currentUserId);
            student.PhoneNumber = studentDto.PhoneNumber;
            await _userRepository.UpdateAsync(student);
            var studentUpdatedEvent = _mapper.Map<StudentCreatedEvent>(student);
            await _publishEndpoint.Publish(studentUpdatedEvent);
            return Ok(SharedResponse<StudentDto>.Success(null, "User updated successfully"));
        }

        [HttpPost("login")]
        public override async Task<ActionResult<SharedResponse<CustomEd.User.Service.DTOs.Common.LoginResponseDto>>> SignIn([FromBody] LoginRequestDto request)
        {
            return await base.SignIn(request);
        }


        [HttpPost("sendOtpForForgotPassword")]
        public async Task<ActionResult<SharedResponse<bool>>> SendOtpForForgotPassword([FromBody] string Email)
        {
            var student = await _userRepository.GetAsync(t => t.Email == Email);
            if (student == null
            || student.IsVerified == false)
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
            var student = await _userRepository.GetAsync(t => t.Email == changePasswordRequestDto.Email);
            student.Password = changePasswordRequestDto.NewPassword;
            await _userRepository.UpdateAsync(student);
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
            return Ok(SharedResponse<String>.Success(user!.ImageUrl, "Image url fetched successfully.")); 
        }

        


    }
}
