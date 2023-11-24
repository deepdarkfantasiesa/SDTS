using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Text.Json;
using Web.Auth.Application.Commands;
using Web.Auth.Models;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Filters
{
    public class QueryUserFilter : Attribute,IResourceFilter
    {
        private readonly UserManager<User> _userManager;
        public QueryUserFilter(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public async void OnResourceExecuting(ResourceExecutingContext context)
        {
            //var syncIOFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
            //if (syncIOFeature != null)
            //{
            //    syncIOFeature.AllowSynchronousIO = true;
            //}
            //var requestBody = context.HttpContext.Request.Body;
            //StreamReader reader = new StreamReader(requestBody);
            //string body = reader.ReadToEndAsync().GetAwaiter().GetResult();
            //var deserializeBody = JsonSerializer.Deserialize<UserDeserialize>(body);
            //var user = _userManager.Users.First(p => p.UserName == deserializeBody.account);
            //if (user == null)
            //{
            //    context.Result = new ContentResult() { Content = "用户不存在" };
            //}
            //context.HttpContext.Request.Body = requestBody;
        }
    }
}
