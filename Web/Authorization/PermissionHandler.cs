using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CoreAuthz = Core.Interfaces.IAuthorizationService;

namespace Web.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly CoreAuthz _authorizationService;

        public PermissionHandler(CoreAuthz authorizationService)
        {
            _authorizationService = authorizationService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                if (await _authorizationService.UserHasPermissionAsync(userId, requirement.PermissionName))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }
}