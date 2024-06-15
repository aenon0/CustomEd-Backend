using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomEd.Announcement.Service.Policies
{
    public class MemberOnlyRequirement : IAuthorizationRequirement { }

    public class MemberOnlyPolicy : AuthorizationHandler<MemberOnlyRequirement>
    {
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public MemberOnlyPolicy(IGenericRepository<ClassRoom> classRoomRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _classRoomRepository = classRoomRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MemberOnlyRequirement requirement)
        {
            var classroomIdString = _httpContextAccessor.HttpContext!.Request.RouteValues["classRoomId"]!.ToString();
            Guid classroomId;
            Guid.TryParse(classroomIdString, out classroomId);
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var userId = identityProvider.GetUserId();
            Console.WriteLine($"SOMETHIGN BOU YOU {classroomIdString}");
            
            var classroom = await _classRoomRepository.GetAsync(x =>x.Id == classroomId);
            Console.WriteLine($"SOMETHIGN BOU YOU {classroom == null}");
            if (classroom != null && ((classroom.MemberIds != null && classroom.MemberIds.Contains(userId)) || classroom.CreatorId == userId))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}