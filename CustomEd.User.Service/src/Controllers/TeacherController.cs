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

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/teacher")]
    public class TeacherController : UserController<Model.Teacher>
    {
        public TeacherController(IGenericRepository<Model.Teacher> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IPublishEndpoint publishEndpoint) : base(userRepository, mapper, passwordHasher, jwtService, publishEndpoint)
        {
        }

        [HttpGet("teacher-name")]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> SearchTeacherByName([FromQuery] string name)
        {
            var teacher = await _userRepository.GetAsync(u => u.FirstName!.Contains(name) || u.LastName!.Contains(name));
            var teacherDto = _mapper.Map<TeacherDto>(teacher);
            return Ok(SharedResponse<TeacherDto>.Success(teacherDto, "Teacher retrieved successfully"));
        }
        [HttpPost]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> CreateUser([FromBody] CreateTeacherDto createTeacherDto)
        {

        var createTeacherDtoValidator = new CreateTeacherDtoValidator(_userRepository);
        var validationResult = await createTeacherDtoValidator.ValidateAsync(createTeacherDto);
        if (!validationResult.IsValid)
        {
            
            return BadRequest(SharedResponse<Model.Teacher>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var passwordHash = _passwordHasher.HashPassword(createTeacherDto.Password);
        createTeacherDto.Password = passwordHash;

        var teacher = _mapper.Map<Model.Teacher>(createTeacherDto);
        teacher.Role = Model.Role.Teacher;
        
        await _userRepository.CreateAsync(teacher);
        var teacherCreatedEvent = _mapper.Map<TeacherCreatedEvent>(teacher);
        var dto = _mapper.Map<TeacherDto>(teacher);
        await _publishEndpoint.Publish(teacherCreatedEvent);
        return CreatedAtAction(nameof(GetUserById), new { id = teacher.Id }, SharedResponse<TeacherDto>.Success(dto, "User created successfully"));
            
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SharedResponse<Model.Teacher>>> RemoveUser(Guid id)
        {
            if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
            {
                return BadRequest(SharedResponse<Model.Teacher>.Fail("Invalid Id", new List<string> { "Invalid id" }));
            }

            var identityProvider = new IdentityProvider(HttpContext, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId != id)
            {
                return Unauthorized(SharedResponse<Model.Teacher>.Fail("Unauthorized to delete user", null));
            }

            await _userRepository.RemoveAsync(id);
            var teacherDeletedEvent = new TeacherDeletedEvent{ Id = id};
            await _publishEndpoint.Publish(teacherDeletedEvent);
            return Ok(SharedResponse<Model.Teacher>.Success(null, "User deleted successfully"));
        }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> UpdateUser([FromBody] UpdateTeacherDto updateTeacherDto)
        {
            var updateTeacherDtoValidator = new UpdateTeacherDtoValidator(_userRepository);
            var validationResult = await updateTeacherDtoValidator.ValidateAsync(updateTeacherDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<TeacherDto>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            var identityProvider = new IdentityProvider(HttpContext, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId != updateTeacherDto.Id)
            {
                return Unauthorized(SharedResponse<TeacherDto>.Fail("Unauthorized to update user", null));
            }

            var passwordHash = _passwordHasher.HashPassword(updateTeacherDto.Password);
            updateTeacherDto.Password = passwordHash;

            var user = _mapper.Map<Model.Teacher>(updateTeacherDto);
            user.Role = Model.Role.Teacher;
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
    }
}
