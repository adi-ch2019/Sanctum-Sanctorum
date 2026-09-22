using Microsoft.Azure.Cosmos;
using Sanctum.Api.Endpoints;
using Sanctum.Api.Services;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var conn = cfg["CosmosDb:ConnectionString"]
        ?? throw new InvalidOperationException("CosmosDb:ConnectionString missing");
    return new CosmosClient(conn, new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });
});

builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();   // available at /scalar/v1
}

app.MapHeroEndpoints();
app.MapGet("/health", () => Results.Ok(new { status = "sanctum-online" }));

app.Run();