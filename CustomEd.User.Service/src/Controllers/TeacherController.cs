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

        [HttpGet("/teacher")]
        public async Task<ActionResult<SharedResponse<IEnumerable<Model.Teacher>>>> GetTeachers()
        {
            var teachers = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<Model.Teacher>>.Success(teachers, "Teachers retrieved successfully"));
        }
    }
}
