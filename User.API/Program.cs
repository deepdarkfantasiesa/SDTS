
using User.API.Application.Behaviors;
using User.API.Extension;
using User.API.Services;
using User.Infrastructure;
using Service.Framework.ConsulRegister;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

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
            builder.Services.AddFilters(builder.Configuration);
            builder.Services.AddRepositories(builder.Configuration);
            builder.Services.AddQueries(builder.Configuration);
            builder.Services.AddEventBus(builder.Configuration);
            builder.Services.AddGrpc(options => { options.EnableDetailedErrors = false; });

            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOptions"));
            builder.Services.AddConsulRegister();

            builder.WebHost.ConfigureKestrel(opt =>
            {
                opt.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);//������֮��gRPC����https��http2��ַ�˿ڵ��ã���http�Ļᱨhttp2�޷��������

                //�����ip���д��ʵ�ģ�consul�޷�������飬grpc���Ե��ã�
                //���д127.0.0.1consul���Խ�����飬grpc������https:localhost:5002���󣬵����޷�����ʵip����ͬʱ����Error starting gRPC call. HttpRequestException: The SSL connection could not be established, see inner exception. AuthenticationException: Cannot determine the frame size or a corrupted frame was received.
                //docker�µ�host.docker.internal����
                //opt.Listen(IPAddress.Parse("127.0.0.1"), 5002, listenOptions =>
                //{
                //    listenOptions.UseHttps("./cert.pfx", "MyPassword");
                //    //listenOptions.Protocols=HttpProtocols.Http1AndHttp2;
                //});
                //opt.Listen(IPAddress.Parse("192.168.18.100"), 5002, listenOptions =>
                //{
                //    listenOptions.UseHttps("./cert.pfx", "MyPassword");
                //});
            });

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var dc = scope.ServiceProvider.GetService<UserContext>();
                //dc.Database.EnsureDeleted();//��ṹ�����ı�ʱ��Ҫ����
                dc.Database.EnsureCreated();
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseConsul(app.Lifetime);

            //app.UseHttpsRedirection();//������������У�http���󶼻ᱻ�ض���https����307 Temporary Redirect

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