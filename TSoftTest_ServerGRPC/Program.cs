using TSoftTest_ServerGRPC.Services;

namespace TSoftTest_ServerGRPC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc();
        builder.Services.AddSingleton<BoxRepository>();

        var app = builder.Build();

        app.MapGrpcService<BoxService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Services.GetService(typeof(BoxRepository));
        app.Run();
    }
}