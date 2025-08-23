namespace LowlandTech.Testing.Features.Reporter.Infrastructure;

/// <summary>
/// Provides a mechanism for registering and resolving types within a dependency injection container.
/// </summary>
/// <remarks>This class acts as a bridge between a dependency injection container and a type
/// registration/resolution system. It supports registering services with various lifetimes, resolving services, and
/// building a service provider. Once built, the service provider can be used to resolve registered types.</remarks>
/// <param name="services"></param>
public sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar, ITypeResolver, IDisposable
{
    private ServiceProvider? _provider;

    /// <summary>
    /// Builds the service provider and returns the current instance of the type resolver.
    /// </summary>
    /// <remarks>This method finalizes the configuration of the service collection by building the service
    /// provider. After calling this method, the type resolver is ready to resolve dependencies.</remarks>
    /// <returns>The current instance of the type resolver, allowing for method chaining.</returns>
    public ITypeResolver Build()
    {
        _provider = services.BuildServiceProvider();
        return this;
    }

    /// <summary>
    /// Resolves an instance of the specified service type from the service provider.
    /// </summary>
    /// <remarks>This method attempts to retrieve a service of the specified type from the underlying service
    /// provider. If the service provider is <see langword="null"/>, the method will return <see
    /// langword="null"/>.</remarks>
    /// <param name="type">The type of the service to resolve. Must not be <see langword="null"/>.</param>
    /// <returns>An instance of the requested service type if available; otherwise, <see langword="null"/>.</returns>
    public object? Resolve(Type? type) => _provider?.GetService(type!);

    /// <summary>
    /// Registers a transient service with the specified service type and implementation type.
    /// </summary>
    /// <remarks>This method adds the specified service and implementation types to the dependency injection
    /// container with a transient lifetime. Each time the service is requested, a new instance of the implementation
    /// type will be created.</remarks>
    /// <param name="service">The type that represents the service to be registered.</param>
    /// <param name="implementation">The type that implements the service.</param>
    public void Register(Type service, Type implementation)
        => services.AddTransient(service, implementation);

    /// <summary>
    /// Registers a singleton instance of the specified service type.
    /// </summary>
    /// <remarks>The specified <paramref name="implementation"/> will be used as a singleton for the given
    /// <paramref name="service"/> type. Subsequent requests for the service will return the same instance.</remarks>
    /// <param name="service">The type of the service to register. This cannot be <see langword="null"/>.</param>
    /// <param name="implementation">The instance of the service to use. This cannot be <see langword="null"/>.</param>
    public void RegisterInstance(Type service, object implementation)
        => services.AddSingleton(service, implementation);

    /// <summary>
    /// Registers a service with a factory method for lazy instantiation.
    /// </summary>
    /// <remarks>The service is registered with a transient lifetime, meaning a new instance will be created  
    /// each time it is requested.</remarks>
    /// <param name="service">The <see cref="Type"/> of the service to register.</param>
    /// <param name="factory">A factory method that creates an instance of the service when needed.</param>
    public void RegisterLazy(Type service, Func<object> factory)
        => services.AddTransient(service, _ => factory());

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method disposes of the underlying resources managed by the instance.  After calling this
    /// method, the instance should not be used further.</remarks>
    public void Dispose() => _provider?.Dispose();
}