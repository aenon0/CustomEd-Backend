using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("/api/user")]
    public abstract class UserController<T> : ControllerBase where T : Model.User
    {
        protected readonly IGenericRepository<T> _userRepository; 

        public UserController(IGenericRepository<T> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public  async Task<ActionResult<SharedResponse<IEnumerable<T>>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<T>>.Success(users, "Users retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<T>>> GetUser(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound(SharedResponse<T>.Fail("User not found", null));
            }
            return Ok(SharedResponse<T>.Success(user, "User retrieved successfully"));
        }
        

        [HttpPost]
        public async Task<ActionResult<SharedResponse<T>>> CreateUser([FromBody] T user)
        {
            await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, SharedResponse<T>.Success(user, "User created successfully"));
            
        }

        [HttpDelete]
        public async Task<ActionResult<SharedResponse<T>>> DeleteUser(Guid id)
        {
            await _userRepository.RemoveAsync(id);
            return Ok(SharedResponse<T>.Success(null, "User deleted successfully"));
        }

        

        
    }
}
