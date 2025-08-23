namespace LowlandTech.Testing.Features.Reporter.Infrastructure.Models;

/// <summary>
/// Represents the result of a scenario execution, including metadata such as status, timing, and identifiers.
/// </summary>
/// <remarks>This record encapsulates the key details of a scenario's execution, such as its unique identifiers,
/// execution status, timestamps, and duration. It is designed to provide a standardized structure for reporting and
/// tracking scenario results.</remarks>
/// <param name="ScenarioId"></param>
/// <param name="Status"></param>
/// <param name="Commit"></param>
/// <param name="RunId"></param>
/// <param name="NaturalKey"></param>
/// <param name="StartedAt"></param>
/// <param name="FinishedAt"></param>
/// <param name="DurationMs"></param>
/// <param name="Runner"></param>
public sealed record ScenarioResultEnvelope(
    string ScenarioId,
    string Status,
    string? Commit,
    string? RunId,
    string NaturalKey,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    long DurationMs,
    string Runner = "manual")
{
    /// <summary>
    /// Creates a new <see cref="ScenarioResultEnvelope"/> for the specified scenario.
    /// </summary>
    /// <remarks>The generated key is a combination of the <paramref name="id"/>, <paramref name="commit"/>,
    /// and <paramref name="runId"/> values. If <paramref name="commit"/> or <paramref name="runId"/> are null, default
    /// values will be used in the key generation.</remarks>
    /// <param name="id">The unique identifier of the scenario. This value cannot be null or empty.</param>
    /// <param name="status">The current status of the scenario. This value cannot be null or empty.</param>
    /// <param name="commit">The commit identifier associated with the scenario. If null, "none" will be used as the default value.</param>
    /// <param name="runId">The run identifier associated with the scenario. If null, the current UTC timestamp in milliseconds will be used
    /// as the default value.</param>
    /// <returns>A <see cref="ScenarioResultEnvelope"/> containing the provided scenario details and a generated key.</returns>
    public static ScenarioResultEnvelope ForScenario(
        string id, string status, string? commit, string? runId)
    {
        var now = DateTimeOffset.UtcNow;
        var key = $"{id}@{commit ?? "none"}:{runId ?? now.ToUnixTimeMilliseconds().ToString()}";
        return new(id, status, commit, runId, key, now, now, 0);
    }
}
