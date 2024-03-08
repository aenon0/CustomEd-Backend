using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;
using AutoMapper;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Validators;
using FluentValidation;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CustomEd.Shared.JWT;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/teacher")]
    public class TeacherController : UserController<Model.Teacher>
    {
        public TeacherController(IGenericRepository<Model.Teacher> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService) : base(userRepository, mapper, passwordHasher, jwtService)
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
        public async Task<ActionResult<SharedResponse<Teacher>>> CreateUser([FromBody] CreateTeacherDto createTeacherDto)
        {

        var createTeacherDtoValidator = new CreateTeacherDtoValidator(_userRepository);
        var validationResult = await createTeacherDtoValidator.ValidateAsync(createTeacherDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(SharedResponse<Teacher>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var passwordHash = _passwordHasher.HashPassword(createTeacherDto.Password);
        createTeacherDto.Password = passwordHash;
        createTeacherDto.JoinDate = DateTime.Now;

        var teacher = _mapper.Map<Model.Teacher>(createTeacherDto);
        await _userRepository.CreateAsync(teacher);
        return CreatedAtAction(nameof(GetUserById), new { id = teacher.Id }, SharedResponse<Teacher>.Success(teacher, "User created successfully"));
            
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SharedResponse<Teacher>>> RemoveUser(Guid id)
        {
            if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
            {
                return BadRequest(SharedResponse<Teacher>.Fail("Invalid Id", new List<string> { "Invalid id" }));
            }

            var identityProvider = new IdentityProvider(HttpContext, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId != id)
            {
                return Unauthorized(SharedResponse<Teacher>.Fail("Unauthorized to delete user", null));
            }

            await _userRepository.RemoveAsync(id);
            return Ok(SharedResponse<Teacher>.Success(null, "User deleted successfully"));
        }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<Teacher>>> UpdateUser([FromBody] UpdateTeacherDto updateTeacherDto)
        {
            var updateTeacherDtoValidator = new UpdateTeacherDtoValidator(_userRepository);
            var validationResult = await updateTeacherDtoValidator.ValidateAsync(updateTeacherDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<Teacher>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            var identityProvider = new IdentityProvider(HttpContext, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId != updateTeacherDto.Id)
            {
                return Unauthorized(SharedResponse<Teacher>.Fail("Unauthorized to update user", null));
            }

            var passwordHash = _passwordHasher.HashPassword(updateTeacherDto.Password);
            updateTeacherDto.Password = passwordHash;

            var user = _mapper.Map<Model.Teacher>(updateTeacherDto);
            await _userRepository.UpdateAsync(user);
            return Ok(SharedResponse<Teacher>.Success(null, "User updated successfully"));
            
        } 
    }
}
