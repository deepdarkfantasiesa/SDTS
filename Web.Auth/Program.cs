
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Auth.Extensions;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbOptions(builder.Configuration);
            builder.Services.AddJWT(builder.Configuration);
            

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dc = scope.ServiceProvider.GetService<IdDbContext>();
                //dc.Database.EnsureDeleted();//表结构发生改变时需要这行
                dc.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}