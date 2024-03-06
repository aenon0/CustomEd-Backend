using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class StudentController : UserController<Model.Student>
    {
        public StudentController(IGenericRepository<Model.Student> userRepository) : base(userRepository)
        {
        }

        [HttpGet("/student")]
        public async Task<ActionResult<SharedResponse<IEnumerable<Model.Student>>>> GetStudents()
        {
            var students = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<Model.Student>>.Success(students, "Students retrieved successfully"));
        }
    }
}
