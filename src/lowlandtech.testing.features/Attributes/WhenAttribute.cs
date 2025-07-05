namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies a condition or scenario under which a class or method is applicable.
/// </summary>
/// <remarks>This attribute can be applied to classes or methods to provide descriptive context about  specific
/// conditions, scenarios, or use cases. It is particularly useful for documentation  or testing purposes where
/// additional context is needed to clarify the intended usage.</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class WhenAttribute(string description) : Attribute
{
    /// <summary>
    /// Gets the description associated with the current instance.
    /// </summary>
    public string Description { get; } = description;
}