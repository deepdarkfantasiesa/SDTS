
using Service.Framework.ConsulRegister;
using user_rpcservices;
using Web.User.HttpAggregator.Extensions;
using Web.User.HttpAggregator.LoadBalancer;
using Service.Framework.ConfigurationCenter;
namespace Web.User.HttpAggregator
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

            builder.Services.AddSingleton<IRoundRobin, RoundRobin>();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);//让gPRC支持http
            builder.Services.AddGrpcClient<UserGrpc.UserGrpcClient>(options =>
            {
                options.Address = new Uri("http://localhost:5000");
                //options.Address = new Uri("https://localhost:5001");
                //options.Address = new Uri("http://localhost:5002");
            }).ConfigurePrimaryHttpMessageHandler(provider =>//服务端如果用了证书，则需要这一段
            {
                var handler = new SocketsHttpHandler();
                handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true;
                return handler;
            });


            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOptions"));
            builder.Services.AddConsulRegister();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseConsul(app.Lifetime);

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}