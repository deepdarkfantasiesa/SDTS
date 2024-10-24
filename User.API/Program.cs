
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Service.Framework.ConfigurationCenter.Consul;
using Service.Framework.ServiceRegistry.Consul;
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
			builder.Host.ConfigureConfigurationCenter();

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

			//ע�Ỻ��
			builder.Services.AddCaches(builder.Configuration);

			//ע�����ݿ�������
			builder.Services.AddDbContexts(builder.Configuration);
			builder.Services.AddIntoContainer(builder.Configuration);

			//ע�������
			builder.Services.AddFilters(builder.Configuration);

			//ע��ִ�
			builder.Services.AddRepositories(builder.Configuration);
			builder.Services.AddQueries(builder.Configuration);

			//ע����Ϣ����
			builder.Services.AddEventBus(builder.Configuration);

			//ע��grpc
			builder.Services.AddGrpc(options => { options.EnableDetailedErrors = false; });

			//ע��������
			builder.Services.AddConfigs(builder.Configuration);

			//ע��consul�����ַ���
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