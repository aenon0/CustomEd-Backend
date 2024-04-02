using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomEd.Assessment.Service.Policies;
public class CreatorOnlyRequirement : IAuthorizationRequirement { }

public class CreatorOnlyPolicy : AuthorizationHandler<CreatorOnlyRequirement>
{
    private readonly IGenericRepository<Classroom> _classRoomRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;

    public CreatorOnlyPolicy(IGenericRepository<Classroom> classRoomRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _classRoomRepository = classRoomRepository;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatorOnlyRequirement requirement)
    {
        var classroomId = (Guid)_httpContextAccessor.HttpContext!.Request.RouteValues["classRoomId"]!;
        var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
        var userId = identityProvider.GetUserId();

        var classroom = await _classRoomRepository.GetAsync(classroomId);
        if (classroom.CreatorId == userId)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

    }
}