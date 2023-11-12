
using User.API.Application.Behaviors;
using User.API.Extension;
using User.API.Services;
using User.Infrastructure;
using Service.Framework.ConsulRegister;
using Microsoft.Extensions.Options;

namespace User.API
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

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });
            builder.Services.AddRedis(builder.Configuration);
            builder.Services.AddContexts(builder.Configuration);
            builder.Services.AddIntoContainer(builder.Configuration);
            builder.Services.AddRepositories(builder.Configuration);
            builder.Services.AddQueries(builder.Configuration);
            builder.Services.AddEventBus(builder.Configuration);
            builder.Services.AddGrpc(options => { options.EnableDetailedErrors = false; });

            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOptions"));
            builder.Services.AddConsulRegister();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var dc = scope.ServiceProvider.GetService<UserContext>();
                //dc.Database.EnsureDeleted();//表结构发生改变时需要这行
                dc.Database.EnsureCreated();
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseConsul(app.Lifetime);

            //app.UseHttpsRedirection();//如果不禁用这行，http请求都会被重定向到https并报307 Temporary Redirect

            app.UseAuthorization();


            app.MapControllers();
            app.UseRouting();
            app.UseEndpoints(options =>
            {
                options.MapGrpcService<UserService>();
            });

            app.Run();
        }
    }
}