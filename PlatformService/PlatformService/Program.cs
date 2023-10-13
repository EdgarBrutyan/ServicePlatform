using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using static PlatformService.Data.PrepDb;
using PlatformService.SyncDataService.Http;
using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataService;
using PlatformService.SyncDataService.Grpc;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("We are in Development Environment, so we use InMemory");
    builder.Services.AddDbContext<AddDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
} 
else
{
    Console.WriteLine("We are in Production Environment, so we use SQL Server");
    builder.Services.AddDbContext<AddDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConnection")));
}

builder.Services.AddScoped<IPlatformRepo,  PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

PrepPopulation(app, builder.Environment.IsProduction());


//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints => 
{ 
    endpoints.MapControllers();
    endpoints.MapGrpcService<GrpcPlatformService>();

    endpoints.MapGet("/protos/platforms.proto", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
    });
}); 

app.MapControllers();

app.Run();
