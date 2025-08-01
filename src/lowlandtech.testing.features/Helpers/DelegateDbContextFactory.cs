namespace LowlandTech.Testing.Features.Helpers;

/// <summary>
/// Provides a factory for creating instances of a specified <see cref="DbContext"/> type.
/// </summary>
/// <remarks>This factory is initialized with a specific instance of the <typeparamref name="T"/> database context
/// and always returns the same instance when <see cref="CreateDbContext"/> is called. It is useful in scenarios where a
/// pre-configured or shared <see cref="DbContext"/> instance is required.</remarks>
/// <typeparam name="T">The type of the <see cref="DbContext"/> to be created by the factory.</typeparam>
public class DelegateDbContextFactory<T> : IDbContextFactory<T> where T : DbContext
{
    private readonly T _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateDbContextFactory{T}"/> class with the specified database
    /// context.
    /// </summary>
    /// <param name="db">The database context instance to be used by the factory. Cannot be null.</param>
    public DelegateDbContextFactory(T db)
    {
        _db = db;
    }

    /// <summary>
    /// Creates and returns an instance of the database context.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/> representing the database context.</returns>
    public T CreateDbContext() => _db;
}