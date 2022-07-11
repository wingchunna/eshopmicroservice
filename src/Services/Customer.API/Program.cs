using Common.Logging;
using Customer.API.Persistence;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Customer.API.Repositories.Interfaces;
using Customer.API.Repositories;
using Customer.API.Services.Interfaces;
using Customer.API.Services;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Customer.API.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Starting Customer API");

try
{


    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(connectionString));

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
        .AddScoped<ICustomerService, CustomerService>();



    var app = builder.Build();

    // minimal API

    app.MapGet("/", () => "Welcome to Customer API");
    app.MapCustomersAPI();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swager Customer Minimal API");
    });
    //app.MapGet("/api/customers", async (ICustomerService customerService) => await customerService.GetCustomerAsync());

    //app.MapGet("/api/customers/{username}", async (string username, ICustomerService customerService) => 
    //{ 
    //        var customer = await customerService.GetCustomerByUsernameAsync(username);
    //        return customer != null ? Results.Ok(customer) : Results.NotFound();
    //});


    //app.MapPost("/api/customers", async (Customer.API.Entities.Customer customer, ICustomerRepository customerRepository) =>
    //{
    //    customerRepository.CreateAsync(customer);
    //    customerRepository.SaveChangesAsync();
    //});
    
    //app.MapDelete("/api/customers/{id}", async (int id, ICustomerRepository customerRepository) =>
    //{
    //    var customer = await customerRepository.FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
    //    if(customer == null) return Results.NotFound();
    //    await customerRepository.DeleteAsync(customer);
    //    await customerRepository.SaveChangesAsync();
    //    return Results.NoContent();

    //});

   // app.MapPut("/api/customers/{id}",);


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.SeedCustomerData().Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}