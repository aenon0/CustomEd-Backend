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

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/student")]
    public class StudentController : UserController<Model.Student>
    {

        public StudentController(IGenericRepository<Otp> otpRepository, IGenericRepository<Model.Student> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor) : base(otpRepository, userRepository, mapper, passwordHasher, jwtService, publishEndpoint, httpContextAccessor)
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
            var createStudentDtoValidator = new CreateStudentDtoValidator(_userRepository);
            var validationResult = await createStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<Model.Student>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;


            var student = _mapper.Map<Model.Student>(studentDto);
            student.Role = Model.Role.Student;

            await _userRepository.CreateAsync(student);

            var studentCreatedEvent = _mapper.Map<StudentCreatedEvent>(student);
            await _publishEndpoint.Publish(studentCreatedEvent);
            return CreatedAtAction(nameof(GetUserById), new { id = student.Id }, SharedResponse<Model.Student>.Success(student, "User created successfully"));

        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> RemoveUser(Guid id)
        {
            if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
            {
                return BadRequest(SharedResponse<StudentDto>.Fail("Invalid Id", new List<string> { "Invalid id" }));
            }
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if(currentUserId != id)
            {
                return Unauthorized(SharedResponse<StudentDto>.Fail("Unauthorized", new List<string> { "Unauthorized" }));
            }


            await _userRepository.RemoveAsync(id);
            var studentDeletedEvent = new StudentDeletedEvent{Id = id};
            await _publishEndpoint.Publish(studentDeletedEvent);
            return Ok(SharedResponse<StudentDto>.Success(null, "User deleted successfully"));
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<SharedResponse<StudentDto>>> UpdateUser([FromBody] UpdateStudentDto studentDto)
        {
            var updateStudentDtoValidator = new UpdateStudentDtoValidator(_userRepository);
            var validationResult = await updateStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<StudentDto>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();

            if(currentUserId != studentDto.Id)
            {
                return Unauthorized(SharedResponse<StudentDto>.Fail("Unauthorized", new List<string> { "Unauthorized" }));
            }

            var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;

            var student = _mapper.Map<Model.Student>(studentDto);
            student.Role = Model.Role.Student;

            await _userRepository.UpdateAsync(student);
            var studentUpdatedEvent = _mapper.Map<StudentCreatedEvent>(student);
            await _publishEndpoint.Publish(studentUpdatedEvent);
            
            return Ok(SharedResponse<StudentDto>.Success(null, "User updated successfully"));

        }

        [HttpPost("login")]
        public override async Task<ActionResult<SharedResponse<UserDto>>> SignIn([FromBody] LoginRequestDto request)
        {

            return await base.SignIn(request);

        }

    }
}
