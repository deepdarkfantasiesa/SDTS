using Microsoft.AspNetCore.Mvc.Filters;

namespace User.API.Filters
{
    public class AuthonizationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine(context.HttpContext.Request.Headers["Role"]);
        }
    }
}
