using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Web.Auth.RepositoriesAndContexts
{
    public class IdDbContext:IdentityDbContext<User,Role,long>
    {
        public IdDbContext(DbContextOptions<IdDbContext> options):base(options) { }
    }
}
