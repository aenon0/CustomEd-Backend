using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/student")]
    public class StudentController : UserController<Model.Student>
    {
        public StudentController(IGenericRepository<Model.Student> userRepository) : base(userRepository)
        {
        }

        [HttpGet("student-id")]
        public async Task<ActionResult<SharedResponse<Model.Student>>> SearchStudentBySchoolId([FromQuery] string id)
        {
            var student = await _userRepository.GetAsync(u => u.StudentId == id);
            return Ok(SharedResponse<Model.Student>.Success(student, "Students retrieved successfully"));
        }

        [HttpGet("student-name")]
        public async Task<ActionResult<SharedResponse<IEnumerable<Model.Student>>>> SearchStudentByName([FromQuery] string name)
        {
            var students = await _userRepository.GetAllAsync(u => u.FirstName!.Contains(name) || u.LastName!.Contains(name));
            return Ok(SharedResponse<IEnumerable<Model.Student>>.Success(students, "Students retrieved successfully"));
        }

        
    }
}
