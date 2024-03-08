using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Response;
using MongoDB.Driver;
using AutoMapper;
using CustomEd.User.Service.PasswordService.Interfaces;
using CusotmEd.User.Servce.DTOs;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("/api/user")]
    public abstract class UserController<T> : ControllerBase where T : Model.User 
    {
        protected readonly IGenericRepository<T> _userRepository; 
        protected readonly IMapper _mapper;
        protected readonly IPasswordHasher _passwordHasher;

        public UserController(IGenericRepository<T> userRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
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

        [HttpPost("login")]
        public async Task<ActionResult<SharedResponse<bool>>> SignIn([FromBody] LoginRequestDto request)
        {
            var user = await _userRepository.GetAsync(x => x.Email == request.Email);
            if(user == null)
            {
                return BadRequest(SharedResponse<bool>.Fail(null, null));
            }
            if(!_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return BadRequest(SharedResponse<bool>.Fail(null, null));
            }
            return Ok(SharedResponse<bool>.Success(true, null));
        }

    }
}
