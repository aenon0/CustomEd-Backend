using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;
using CustomEd.User.Service.DTOs;
using AutoMapper;
using CustomEd.User.Service.Validators;
using CustomEd.User.Service.PasswordService.Interfaces;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/student")]
    public class StudentController : UserController<Model.Student>
    {
        public StudentController(IGenericRepository<Model.Student> userRepository, IMapper mapper, IPasswordHasher passwordHasher) : base(userRepository, mapper, passwordHasher)
        {
        }

        [HttpGet("student-id")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> SearchStudentBySchoolId([FromQuery] string id)
        {
            var student = await _userRepository.GetAsync(u => u.StudentId == id);
            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(SharedResponse<StudentDto>.Success(studentDto, "Students retrieved successfully"));
        }

        [HttpGet("student-name")]
        public async Task<ActionResult<SharedResponse<IEnumerable<StudentDto>>>> SearchStudentByName([FromQuery] string name)
        {
            var students = await _userRepository.GetAllAsync(u => u.FirstName!.Contains(name) || u.LastName!.Contains(name));
            var studentsDto = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Ok(SharedResponse<IEnumerable<StudentDto>>.Success(studentsDto, "Students retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<SharedResponse<Model.Student>>> CreateUser([FromBody] CreateStudentDto studentDto)
        {

            var createStudentDtoValidator = new CreateStudentDtoValidator();
            var validationResult = await createStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<Model.Student>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }
            var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;


            var student = _mapper.Map<Model.Student>(studentDto);
            
            await _userRepository.CreateAsync(student);
            return CreatedAtAction(nameof(GetUserById), new { id = student.Id }, SharedResponse<Model.Student>.Success(student, "User created successfully"));
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> RemoveUser(Guid id)
        {
            if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
            {
                return BadRequest(SharedResponse<StudentDto>.Fail("Invalid Id", new List<string> { "Invalid id" }));
            }


            await _userRepository.RemoveAsync(id);
            return Ok(SharedResponse<StudentDto>.Success(null, "User deleted successfully"));
        }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<StudentDto>>> UpdateUser([FromBody] UpdateStudentDto studentDto)
        {
            var updateStudentDtoValidator = new UpdateStudentDtoValidator(_userRepository);
            var validationResult = await updateStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<StudentDto>.Fail("Invalid input", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;
            studentDto.JoinDate = DateTime.UtcNow;

            var student = _mapper.Map<Model.Student>(studentDto);
            await _userRepository.UpdateAsync(student);
            return Ok(SharedResponse<StudentDto>.Success(null, "User updated successfully"));
            
        }
        
    }
}
