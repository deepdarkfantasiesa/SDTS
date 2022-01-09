using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(o =>
               {
                   o.SaveToken = true;
                   o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                   {

                   };

                   o.SecurityTokenValidators.Clear();
                   o.SecurityTokenValidators.Add(new TokenValidtor());
                   o.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           var accessToken = context.Request.Query["access_token"];

                           var path = context.HttpContext.Request.Path;
                           if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api"))
                           //path.StartsWithSegments("/api/managewards"))
                           {
                               context.Token = accessToken;
                           }
                           return Task.CompletedTask;
                       }
                   };
               });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.Requirements.Add(
                        new PermissionRequirement {Type = "º‡ª§»À" });
                });
            });
            services.AddTransient<IAuthorizationHandler, PermissionHandler>();


            //services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            //{
            //    builder.AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .WithOrigins("http://localhost:5002");
            //}));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
