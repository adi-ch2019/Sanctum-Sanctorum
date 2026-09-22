using Microsoft.Azure.Cosmos;
using Sanctum.Api.Models;

namespace Sanctum.Api.Services;
public interface ICosmosDbService
{
    Task<Hero> CreateHeroWithOutboxAsync(Hero hero, CancellationToken ct = default);
    Task<IReadOnlyList<Hero>> GetHeroesAsync(CancellationToken ct = default);
}

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(CosmosClient client, IConfiguration config)
    {
        var dbName = config["CosmosDb:DatabaseName"] ?? "SanctumDb";
        var containerName = config["CosmosDb:ContainerName"] ?? "Heroes";
        _container = client.GetContainer(dbName, containerName);
    }

    public async Task<Hero> CreateHeroWithOutboxAsync(Hero hero, CancellationToken ct = default)
    {
        var outboxEvent = new HeroCreatedEvent
        {
            HeroId = hero.Id,
            Name = hero.Name,
            Power = hero.Power
        };

        // Same partition key -> atomic transaction
        var batch = _container.CreateTransactionalBatch(new PartitionKey(hero.Id))
            .CreateItem(hero)
            .CreateItem(outboxEvent);

        using var response = await batch.ExecuteAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Outbox transaction failed: {response.ErrorMessage}");

        return hero;
    }

    public async Task<IReadOnlyList<Hero>> GetHeroesAsync(CancellationToken ct = default)
    {
        var query = _container.GetItemQueryIterator<Hero>(
            new QueryDefinition("SELECT * FROM c WHERE c.EventType = undefined OR NOT IS_DEFINED(c.EventType)"));

        var results = new List<Hero>();
        while (query.HasMoreResults)
        {
            var page = await query.ReadNextAsync(ct);
            results.AddRange(page);
        }
        return results;
    }
}