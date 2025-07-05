namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies a unique identifier for a task associated with a class.
/// </summary>
/// <remarks>This attribute can be applied to classes to associate them with a specific task identifier. It is not
/// inherited by derived classes and allows multiple instances to be applied to the same class.</remarks>
/// <param name="id">The unique identifier for the task. This value cannot be null or empty.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class TaskIdAttribute(string id) : Attribute
{
    /// <summary>
    /// Gets the unique identifier for the current instance.
    /// </summary>
    public string TaskId { get; } = id;
}