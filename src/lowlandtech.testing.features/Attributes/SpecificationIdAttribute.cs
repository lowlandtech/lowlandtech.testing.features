namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies a unique identifier for a use case associated with a class.
/// </summary>
/// <remarks>This attribute can be applied to a class to associate it with one or more use case identifiers. Use
/// this attribute to document or categorize classes based on their related use cases.</remarks>
/// <param name="specificationId">The unique identifier for the use case. This value cannot be null or empty.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SpecificationIdAttribute(string specificationId) : NodeIdAttribute(specificationId)
{
    /// <summary>
    /// Gets the unique identifier for the use case.
    /// </summary>
    public string SpecificationId { get; } = specificationId;
}