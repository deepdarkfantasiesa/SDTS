using Microsoft.AspNetCore.Identity;

namespace Web.Auth.RepositoriesAndContexts
{
    public class User:IdentityUser<long>
    {
        public DateTime CreateTime { get; set; }
        public string? NickName {  get; set; }
    }
}
