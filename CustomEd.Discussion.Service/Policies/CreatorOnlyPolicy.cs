
using CustomEd.Discussion.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomEd.Discussion.Service.Policies
{
    public class CreatorOnlyRequirment : IAuthorizationRequirement { }

    public class CreatorOnlyPolicy : AuthorizationHandler<CreatorOnlyRequirment>
    {
        private readonly IGenericRepository<Message> _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public CreatorOnlyPolicy(IGenericRepository<Message> messageRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _messageRepository = messageRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatorOnlyRequirment requirement)
        {
            var messageIdString = _httpContextAccessor.HttpContext!.Request.RouteValues["messageId"]!.ToString();
            Guid messageId;
            Guid.TryParse(messageIdString, out messageId);
            // Console.WriteLine($"Classrroom Id: {classroomId}");
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var userId = identityProvider.GetUserId();

            var message = await _messageRepository.GetAsync(messageId);
            Console.WriteLine($"AAAAAAAAAAA{message.SenderId} {userId}");
            if (message?.SenderId != null && message.SenderId == userId)
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
