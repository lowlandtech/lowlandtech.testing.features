namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Represents an attribute used to annotate a class with a behavior-driven development (BDD) scenario,
/// specifying the scenario code, title, and Given/When/Then steps.
/// </summary>
/// <example>
/// // New usage with scenario code, title, and multiple steps
/// [Scenario(
///     "VCHIP-2061-SC001",
///     "Order processing workflow",
///     "Given a workflow definition with validation and notification",
///     "When the first step is evaluated and dispatched",
///     "Then the state should reflect that the order is processed",
///     "And an email should be sent")]
///
/// // Old usage without code/title
/// [Scenario(
///     "Given a workflow definition with validation and notification",
///     "When the first step is evaluated and dispatched",
///     "Then the state should reflect that the order is processed")]
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ScenarioAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScenarioAttribute"/> class (backward-compatible constructor).
    /// </summary>
    /// <param name="given">The "Given" step describing initial context or preconditions.</param>
    /// <param name="when">The "When" step describing the action or event that triggers the scenario.</param>
    /// <param name="then">The "Then" step describing the expected outcome.</param>
    public ScenarioAttribute(
        string given,
        string? when = null,
        string? then = null)
    {
        if (string.IsNullOrWhiteSpace(given))
            throw new ArgumentException("Given step cannot be null or empty.", nameof(given));

        Given = given;
        When = when;
        Then = then;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScenarioAttribute"/> class with scenario code and title.
    /// </summary>
    /// <param name="specId">A unique scenario code (e.g., VCHIP-2061-SC001).</param>
    /// <param name="title">The title of the scenario.</param>
    /// <param name="given">The "Given" step.</param>
    /// <param name="when">The "When" step.</param>
    /// <param name="then">The "Then" step.</param>
    public ScenarioAttribute(
        string specId,
        string title,
        string given,
        string? when = null,
        string? then = null)
    {
        if (string.IsNullOrWhiteSpace(specId))
            throw new ArgumentException("Scenario code cannot be null or empty.", nameof(specId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Scenario title cannot be null or empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(given))
            throw new ArgumentException("Given step cannot be null or empty.", nameof(given));

        SpecId = specId;
        Title = title;
        Given = given;
        When = when;
        Then = then;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScenarioAttribute"/> class with scenario code, title, and arbitrary steps.
    /// </summary>
    /// <param name="specId">A unique scenario code.</param>
    /// <param name="title">The scenario title.</param>
    /// <param name="steps">All scenario steps (Given/When/Then/And), in order.</param>
    public ScenarioAttribute(
        string specId,
        string title,
        params string[] steps)
    {
        if (string.IsNullOrWhiteSpace(specId))
            throw new ArgumentException("Scenario code cannot be null or empty.", nameof(specId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Scenario title cannot be null or empty.", nameof(title));

        if (steps == null || steps.Length == 0)
            throw new ArgumentException("At least one step must be provided.", nameof(steps));

        SpecId = specId;
        Title = title;
        Steps = steps;
    }

    /// <summary>
    /// Gets the unique scenario code.
    /// </summary>
    public string? SpecId { get; }

    /// <summary>
    /// Gets the scenario title.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Gets the "Given" step.
    /// </summary>
    public string? Given { get; }

    /// <summary>
    /// Gets the optional "When" step.
    /// </summary>
    public string? When { get; }

    /// <summary>
    /// Gets the optional "Then" step.
    /// </summary>
    public string? Then { get; }

    /// <summary>
    /// Gets all scenario steps if the params-based constructor was used.
    /// </summary>
    public string[]? Steps { get; }

    /// <summary>
    /// Returns a string representation that appears in the test runner output.
    /// </summary>
    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(SpecId) && !string.IsNullOrWhiteSpace(Title))
        {
            return $"{SpecId}: {Title}";
        }

        return Given ?? (Steps != null && Steps.Length > 0 ? Steps[0] : string.Empty);
    }
}
