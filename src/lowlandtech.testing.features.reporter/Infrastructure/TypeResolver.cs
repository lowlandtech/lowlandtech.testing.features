namespace LowlandTech.Testing.Features.Reporter.Infrastructure;

/// <summary>
/// Provides a mechanism for resolving instances of types based on a set of registrations or existing instances.
/// </summary>
/// <remarks>This class allows for resolving objects by type, either from a predefined list of instances or by
/// creating new instances based on type mappings. It is designed to be used in scenarios where dependency resolution is
/// required, such as in dependency injection frameworks or service locators.</remarks>
public sealed class TypeResolver : ITypeResolver, IDisposable
{
    /// <summary>
    /// A collection that maps service types to their corresponding implementation types.
    /// </summary>
    /// <remarks>This dictionary is used to store type registrations, where the key represents the service
    /// type and the value represents the implementation type. It is intended for internal use to manage type mappings
    /// within the dependency injection system.</remarks>
    private readonly Dictionary<Type, Type> _registrations;

    /// <summary>
    /// A collection of object instances used internally by the class.
    /// </summary>
    /// <remarks>This field is read-only and is intended for internal use only. It stores a list of object
    /// instances that may be utilized by various operations within the class.</remarks>
    private readonly List<object> _instances;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeResolver"/> class with the specified type registrations and
    /// pre-existing instances.
    /// </summary>
    /// <param name="registrations">A dictionary mapping interface or base types to their corresponding concrete types.  This is used to resolve
    /// types at runtime. Cannot be null.</param>
    /// <param name="instances">A list of pre-existing instances to be used during type resolution.  These instances are prioritized over type
    /// registrations. Cannot be null.</param>
    public TypeResolver(Dictionary<Type, Type> registrations, List<object> instances)
        => (_registrations, _instances) = (registrations, instances);

    /// <summary>
    /// Resolves an instance of the specified type from the available instances or registered implementations.
    /// </summary>
    /// <remarks>This method first searches the existing instances to find one that matches the specified
    /// type.  If no matching instance is found, it attempts to create a new instance using a registered implementation,
    /// if available. If neither is successful, the method returns <see langword="null"/>.</remarks>
    /// <param name="type">The type of the object to resolve. This cannot be <see langword="null"/>.</param>
    /// <returns>An instance of the specified type if found; otherwise, <see langword="null"/>.</returns>
    public object? Resolve(Type type)
        => _instances.FirstOrDefault(x => type.IsInstanceOfType(x))
           ?? (_registrations.TryGetValue(type, out var impl) ? Activator.CreateInstance(impl) : null);

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>Call this method when you are finished using the instance to free up resources.  After
    /// calling <see cref="Dispose"/>, the instance is in an unusable state and should not be used further.</remarks>
    public void Dispose() { }
}
