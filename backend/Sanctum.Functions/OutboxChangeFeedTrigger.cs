using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Sanctum.Functions;

public class HeroCreatedEvent
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("heroId")] public string HeroId { get; set; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("power")] public string Power { get; set; } = string.Empty;
    [JsonPropertyName("eventType")] public string EventType { get; set; } = string.Empty;
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}

public class OutboxChangeFeedTrigger
{
    private readonly ILogger<OutboxChangeFeedTrigger> _logger;

    public OutboxChangeFeedTrigger(ILogger<OutboxChangeFeedTrigger> logger)
    {
        _logger = logger;
    }

    [Function("OutboxChangeFeed")]
    public void Run(
        [CosmosDBTrigger(
            databaseName: "SanctumDb",
            containerName: "Heroes",
            Connection = "CosmosDbConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<HeroCreatedEvent> changes)
    {
        if (changes is null || changes.Count == 0)
        {
            _logger.LogInformation("No changes in this batch.");
            return;
        }

        foreach (var evt in changes)
        {
            if (evt.EventType != "HeroCreated") continue;

            _logger.LogInformation(
                "SANCTUM PORTAL OPENED -> Hero {Name} (id: {HeroId}) materialized at {Timestamp}",
                evt.Name, evt.HeroId, evt.Timestamp);
        }
    }
}