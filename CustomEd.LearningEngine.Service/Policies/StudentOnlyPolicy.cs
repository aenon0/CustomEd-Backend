using CustomEd.LearningEngine.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomEd.LearningEngine.Service.Policies
{
    public class StudentOnlyRequirement : IAuthorizationRequirement { }

    public class StudentOnlyPolicy : AuthorizationHandler<StudentOnlyRequirement>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public StudentOnlyPolicy(IGenericRepository<Student> studentRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _studentRepository = studentRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StudentOnlyRequirement requirement)
        {
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var userId = identityProvider.GetUserId();
            if (userId != Guid.Empty)
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