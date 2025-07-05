namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies a unique identifier for a use case associated with a class.
/// </summary>
/// <remarks>This attribute can be applied to a class to associate it with one or more use case identifiers. Use
/// this attribute to document or categorize classes based on their related use cases.</remarks>
/// <param name="useCaseId">The unique identifier for the use case. This value cannot be null or empty.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UseCaseIdAttribute(string useCaseId) : NodeIdAttribute(useCaseId)
{
    /// <summary>
    /// Gets the unique identifier for the use case.
    /// </summary>
    public string UseCaseId { get; } = useCaseId;
}