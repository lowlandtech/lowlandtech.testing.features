namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies a unique identifier for a class, typically used to associate metadata or provide a reference to the class
/// in external systems.
/// </summary>
/// <remarks>This attribute is applied to classes and is not inherited by derived classes. The identifier is
/// represented as a GUID and must be provided as a valid string format when the attribute is instantiated.</remarks>
/// <param name="id"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NodeIdAttribute(string id) : Attribute
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public string NodeId { get; } = id;
}