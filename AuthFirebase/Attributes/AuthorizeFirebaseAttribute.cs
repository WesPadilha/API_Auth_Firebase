using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeFirebaseAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string? _requiredRole;

    public AuthorizeFirebaseAttribute(string? requiredRole = null)
    {
        _requiredRole = requiredRole;
    }
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

            if(!string.IsNullOrEmpty(_requiredRole))
            {
                if(!decodedToken.Claims.TryGetValue("role", out var roleValue) || roleValue?.ToString() !=_requiredRole)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
        catch (System.Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}