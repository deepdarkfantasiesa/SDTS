using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        public string Type { get; set; }

    }


    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //var age = context.User.Claims.First(p => p.Type.Equals("Age")).Value;
            var type= context.User.Claims.First(p => p.Type.Equals("Type")).Value;
            if (type.Equals(requirement.Type.ToString()))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
