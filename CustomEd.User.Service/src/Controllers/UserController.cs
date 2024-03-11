using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using AutoMapper;
using CustomEd.User.Service.PasswordService.Interfaces;
using CusotmEd.User.Servce.DTOs;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("/api/user")]
    public abstract class UserController<T> : ControllerBase where T : Model.User 
    {
        protected readonly IGenericRepository<T> _userRepository; 
        protected readonly IMapper _mapper;
        protected readonly IPasswordHasher _passwordHasher;
        protected readonly IJwtService _jwtService;

        public UserController(IGenericRepository<T> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
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
        public async Task<ActionResult<SharedResponse<UserDto>>> SignIn([FromBody] LoginRequestDto request)
        {

            var user = await _userRepository.GetAsync(x => x.Email == request.Email);
            if(user == null)
            {
                return BadRequest(SharedResponse<UserDto>.Fail("User not found", null));
            }
            if(!_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return BadRequest(SharedResponse<bool>.Fail("Incorrect Password", null));
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = (Shared.JWT.Contracts.Role) user.Role
            };

            var token = _jwtService.GenerateToken(userDto);
            userDto.Token = token;

            return Ok(SharedResponse<UserDto>.Success(userDto, null));
        }

    }
}
