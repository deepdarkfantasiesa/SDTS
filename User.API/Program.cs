
using User.API.Application.Behaviors;
using User.API.Extension;
using User.API.Services;
using User.Infrastructure;

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
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });
            builder.Services.AddContexts(builder.Configuration.GetValue<string>("MySQL"));
            builder.Services.AddRepositories(builder.Configuration);
            builder.Services.AddQueries(builder.Configuration);
            builder.Services.AddEventBus(builder.Configuration);
            builder.Services.AddGrpc(options => { options.EnableDetailedErrors = false; });
            

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var dc = scope.ServiceProvider.GetService<UserContext>();
                dc.Database.EnsureCreated();
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

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