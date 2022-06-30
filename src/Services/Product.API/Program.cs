using Common.Logging;
using Product.API.Extensions;
using Serilog;






var builder = WebApplication.CreateBuilder(args);


Log.Information("Startng Product API");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();

    // Add services to the container.
    builder.Services.AddInfrastructure();

    var app = builder.Build();
    app.UseInfrastructure();

   
  
    app.Run();
}catch(Exception ex)
{
    Log.Fatal(ex, "Unhandle Exception");
}

finally
{
    Log.Information("Shutdown Product API complete");
    Log.CloseAndFlush();
}

