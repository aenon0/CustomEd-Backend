using CustomEd.Shared.JWT;
using CustomEd.Shared.Model;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

public class TeacherOnlyRequirement : IAuthorizationRequirement
{
}

public class TeacherOnlyRequirementHandler : AuthorizationHandler<TeacherOnlyRequirement>
{
    private readonly IdentityProvider _identityProvider;

    public TeacherOnlyRequirementHandler(IdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TeacherOnlyRequirement requirement)
    {
        var user = context.User;
        var role =  _identityProvider.GetUserRole();
        Console.WriteLine(role);

        if (role == (Role)1)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}