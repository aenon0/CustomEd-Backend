using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;
using MongoDB.Driver;
using AutoMapper;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("/api/user")]
    public abstract class UserController<T> : ControllerBase where T : Model.User 
    {
        protected readonly IGenericRepository<T> _userRepository; 
        protected readonly IMapper _mapper;

        public UserController(IGenericRepository<T> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public  async Task<ActionResult<SharedResponse<IEnumerable<T>>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<T>>.Success(users, "Users retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<T>>> GetUserById(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound(SharedResponse<T>.Fail("User not found", null));
            }
            return Ok(SharedResponse<T>.Success(user, "User retrieved successfully"));
        }

    }
}
