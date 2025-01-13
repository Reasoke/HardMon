using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using server.Repository;

namespace server.Classes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthCookieCheckFilterAttribute : Attribute, IAsyncAuthorizationFilter
{

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var executingEndpoint = context.HttpContext.GetEndpoint();
        if (executingEndpoint != null && executingEndpoint.Metadata.OfType<AllowAnonymousAttribute>().Any())
            return;

        var userId = AuthorizationHelper.ValidateCookie(context.HttpContext.Request.Cookies);

        var userRepository = context.HttpContext.RequestServices.GetService<IUsersRepository>();
        var user = await userRepository.GetUser(userId);
        if (user == null)
            throw new DomainException(HttpStatusCode.Forbidden, "User is not found");

        var identity = new HardMonUserIdentity(user.Id, user.Email, user.IsSysAdmin);
        // identity.AddClaim(new Claim(ClaimTypes.Name, user.Email)); //just example
        context.HttpContext.User = new ClaimsPrincipal(identity);
    }
}