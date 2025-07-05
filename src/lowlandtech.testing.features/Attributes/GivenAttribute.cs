namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies a precondition or context for a test scenario.
/// </summary>
/// <remarks>This attribute is used to annotate test classes or methods with a description of the  given
/// precondition or context that must be satisfied before the test is executed.  It supports multiple instances on the
/// same class or method to describe multiple preconditions.</remarks>
/// <param name="description">A brief description of the precondition or context for the test. This value cannot be null or empty.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class GivenAttribute(string description) : Attribute
{
    /// <summary>
    /// Gets the description associated with the current instance.
    /// </summary>
    public string Description { get; } = description;
}