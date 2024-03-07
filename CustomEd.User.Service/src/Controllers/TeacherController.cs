using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/teacher")]
    public class TeacherController : UserController<Model.Teacher>
    {
        public TeacherController(IGenericRepository<Model.Teacher> userRepository) : base(userRepository)
        {
        }

        [HttpGet("teacher-name")]
        public async Task<ActionResult<SharedResponse<Model.Teacher>>> SearchTeacherByName([FromQuery] string name)
        {
            var teacher = await _userRepository.GetAsync(u => u.FirstName!.Contains(name) || u.LastName!.Contains(name));
            return Ok(SharedResponse<Model.Teacher>.Success(teacher, "Teacher retrieved successfully"));
        }
    }
}
