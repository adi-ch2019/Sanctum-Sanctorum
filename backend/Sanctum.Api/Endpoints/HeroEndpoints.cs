using Sanctum.Api.Models;
using Sanctum.Api.Services;

namespace Sanctum.Api.Endpoints;

public static class HeroEndpoints
{
    public static void MapHeroEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/heroes").WithTags("Heroes");

        group.MapGet("/", async (ICosmosDbService db, CancellationToken ct) =>
        {
            var heroes = await db.GetHeroesAsync(ct);
            return Results.Ok(heroes);
        });

        group.MapPost("/", async (Hero hero, ICosmosDbService db, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(hero.Name) || string.IsNullOrWhiteSpace(hero.Power))
                return Results.BadRequest("Name and Power are required.");

            var created = await db.CreateHeroWithOutboxAsync(hero, ct);
            return Results.Created($"/api/heroes/{created.Id}", created);
        });
    }
}