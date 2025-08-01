using Auth.API.Application.Behaviors;
using Auth.API.Extension;
using Auth.API.Services;
using Domain.Abstraction;
using Domain.Abstraction.Mediator;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Service.Framework.ConfigurationCenter.Consul;
using Service.Framework.ServiceRegistry.Consul;

namespace Auth.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureConfigurationCenter();

            //配置Serilog
            builder.Host.UseSerilog((context, logConfig) =>
                logConfig.ReadFrom.Configuration(builder.Configuration));

            // Add services to the container.

            builder.Services.AddControllers()
                .AddStrongTypeIdJsonConverter("Auth.Domain");//注册强类型Id的Json转换器

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCustomSwaggerGen("Auth.Domain");//注册swagger中强类型Id显示为string类型

            //注册强类型id转换器
            builder.Services.AddStrongTypeConverter("Auth.Domain");

            //注册中介者
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                //cfg.AddOpenBehavior(typeof(QueryCacheBehavior<,>));
                cfg.AddOpenBehavior(typeof(QueryReplicaBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionSettingBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            //注册中介者
            builder.Services.AddMediator(
                typeof(QueryCacheBehavior<,>),
                typeof(TransactionBehaviorNext<,>));

            //注册缓存
            builder.Services.AddCaches(builder.Configuration);

            //注册分布式锁
            builder.Services.AddDistributedLock(builder.Configuration);

            //注册consul服务发现服务
            builder.Services.AddConsulRegister();

            //注册数据库上下文
            builder.Services.AddDbContexts(builder.Configuration);

            //注册命令验证者
            builder.Services.AddFluentValidation(builder.Configuration);

            //注册过滤器
            builder.Services.AddFilters(builder.Configuration);

            //注册仓储
            builder.Services.AddRepositories(builder.Configuration);

            //注册消息队列
            builder.Services.AddEventBus(builder.Configuration);

            //注册grpc
            builder.Services.AddGrpc(options => { options.EnableDetailedErrors = false; });

            //注册配置类
            builder.Services.AddConfigs(builder.Configuration);

            //注册追踪者
            builder.Services.AddTracing(builder.Configuration);

            //注册后台任务
            builder.Services.AddBackgroundHosts(builder.Configuration);

            builder.WebHost.ConfigureKestrel(opt =>
            {
                opt.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);//配置了之后gRPC可用https和http2地址端口调用，而http的会报http2无法完成握手

                //这里的ip如果写真实的，consul无法健康检查，grpc可以调用；
                //如果写127.0.0.1consul可以健康检查，grpc可以用https:localhost:5002请求，但是无法用真实ip请求，同时报：Error starting gRPC call. HttpRequestException: The SSL connection could not be established, see inner exception. AuthenticationException: Cannot determine the frame size or a corrupted frame was received.
                //docker下的host.docker.internal待测
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

            app.UseSerilogRequestLogging();

            // 在使用路由、终结点、中间件之前，添加此中间件
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            //using(var scope = app.Services.CreateScope())
            //{
            //	var dc = scope.ServiceProvider.GetService<UserContext>();
            //	//dc.Database.EnsureDeleted();//表结构发生改变时需要这行
            //	dc.Database.EnsureCreated();
            //}

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