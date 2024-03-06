using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("/api/user")]
    public class UserController : ControllerBase
    {
        private readonly IGenericRepository<Model.User> _userRepository; 

        public UserController(IGenericRepository<Model.User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public  async Task<ActionResult<SharedResponse<IEnumerable<Model.User>>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<Model.User>>.Success(users, "Users retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<Model.User>>> GetUser(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound(SharedResponse<Model.User>.Fail("User not found", null));
            }
            return Ok(SharedResponse<Model.User>.Success(user, "User retrieved successfully"));
        }
        

        [HttpPost]
        public async Task<ActionResult<SharedResponse<Model.User>>> CreateUser([FromBody] Model.User user)
        {
            await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, SharedResponse<Model.User>.Success(user, "User created successfully"));
            
        }

        
    }
}
