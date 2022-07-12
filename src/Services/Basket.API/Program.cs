using Basket.API.Extensions;
using Common.Logging;
using Serilog;






var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Startng Basket API");

try
{
    builder.Host.AddAppConfigurations();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandle Exception");
}

finally
{
    Log.Information("Shutdown Basket API complete");
    Log.CloseAndFlush();
}

