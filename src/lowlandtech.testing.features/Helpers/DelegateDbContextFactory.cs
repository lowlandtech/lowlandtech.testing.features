namespace LowlandTech.Testing.Features.Helpers;
public class DelegateDbContextFactory<T> : IDbContextFactory<T> where T : DbContext
{
    private readonly T _db;

    public DelegateDbContextFactory(T db)
    {
        _db = db;
    }

    public T CreateDbContext() => _db;
}