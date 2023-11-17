using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Auth.Options;
using Web.Auth.RepositoriesAndContexts;
using Web.Auth.SecurityTokenValidators;

namespace Web.Auth.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddOthers(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtopt = configuration.GetSection("JWT").Get<JWTOptions>();
            services.AddSingleton(jwtopt);
            
            return services;
        }

        public static IServiceCollection AddMediatRs(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
            });
            return services;
        }

        public static IServiceCollection AddJWT(this IServiceCollection services,IConfiguration configuration)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ISecurityTokenValidator, CustomTokenValidtor>();//这个生命周期暂定全局唯一
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    var jwtopt = configuration.GetSection("JWT").Get<JWTOptions>();
                    var keyByte = Encoding.UTF8.GetBytes(jwtopt.SigningKey);
                    var secKey = new SymmetricSecurityKey(keyByte);

                    x.TokenValidationParameters = new() { 
                        ValidateIssuer = false,
                        ValidateAudience =false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = secKey
                    };

                    x.SecurityTokenValidators.Clear();//删除自带的token验证逻辑
                    x.SecurityTokenValidators.Add(services.BuildServiceProvider().GetService<ISecurityTokenValidator>());
                });
            return services;
        }

        public static IServiceCollection AddDbOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdDbContext>(options =>
            {
                var connstr = configuration.GetValue<string>("SQLServer");
                options.UseSqlServer(connstr);
            });

            services.AddDataProtection();
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            });

            var idBuilder = new IdentityBuilder(typeof(User), typeof(Role), services);
            idBuilder.AddEntityFrameworkStores<IdDbContext>()
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<Role>>()
                .AddUserManager<UserManager<User>>();

            return services;
        }
    }
}
