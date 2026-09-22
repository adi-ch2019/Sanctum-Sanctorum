namespace Sanctum.Api.Models;

public record Hero
{
     public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = string.Empty;
    public string Power { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record HeroCreatedEvent
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string HeroId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Power { get; init; } = string.Empty;
    public string EventType { get; init; } = "HeroCreated";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}