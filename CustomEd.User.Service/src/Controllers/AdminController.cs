using Microsoft.AspNetCore.Mvc;
using CustomEd.User.Service.DTOs;
using AutoMapper;
using CustomEd.User.Service.Validators;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using MassTransit;
using CustomEd.Shared.JWT.Contracts;
using CusotmEd.User.Servce.DTOs;
using CustomEd.User.Student.Events;
using CustomEd.Shared.JWT;
using CustomEd.User.Service.Model;
using src;

namespace CustomEd.User.Service.Controllers;

    [ApiController]
    [Route("api/user/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IGenericRepository<Model.Admin> _adminRepository; 
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AdminController(
            IGenericRepository<Model.Admin> adminRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor)
            {
                _adminRepository = adminRepository;
                _mapper = mapper;
                _passwordHasher = passwordHasher;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
            }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginRequestDto loginDto)
        {
            var user = await _adminRepository.GetAsync(x => (x.Username == loginDto.Username) );
            if(user == null)
            {
                return BadRequest(SharedResponse<UserDto>.Fail("User not found", null));
            }
            if(!_passwordHasher.VerifyPassword(loginDto.Password, user.Password))
            {
                return BadRequest(SharedResponse<bool>.Fail("Incorrect Password", null));
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = (IdentityRole) user.Role
            };

            var token = _jwtService.GenerateToken(userDto);
            userDto.Token = token;
            return Ok(SharedResponse<UserDto>.Success(userDto, null));
        }

        [HttpPost("register")]
        public async Task<ActionResult <SharedResponse<Admin>>>CreateAdmin([FromBody] Admin admin)
        {
            var passwordHash = _passwordHasher.HashPassword(admin.Password);
            admin.Password = passwordHash;
            admin.Role = Model.Role.Admin;
            await _adminRepository.CreateAsync(admin);
            return  SharedResponse<Admin>.Success(admin, "Admin created successfully");
        }
    }

