# Migration Guide: V1 to V2 Base Classes

## Overview

The V2 base classes provide significant improvements while maintaining backward compatibility with existing tests. All V1 classes remain unchanged and fully functional.

## Key Improvements in V2

### ?? **All V2 Classes**
- ? Proper `IAsyncLifetime` implementation (no blocking `.GetAwaiter().GetResult()` in constructors)
- ? `CancellationToken` support for timeout scenarios
- ? Proper async disposal (`IAsyncDisposable`)
- ? Standardized naming (`Sut` everywhere, no more `Cut`)
- ? Virtual cleanup hooks for custom resource management
- ? Consistent API across all base classes

### ?? **Specific Improvements**

#### `WhenTestingForV2<T>`
- Implements `IDisposable` for proper resource cleanup
- Adds `Cleanup()` virtual method
- Automatically disposes `Sut` if it implements `IDisposable`

#### `WhenTestingForAsyncV2<T>`
- No more blocking in constructor
- Full `CancellationToken` support
- Proper `IAsyncLifetime` + `IAsyncDisposable`

#### `WhenUsingDatabaseV2<TContext>`
- No more blocking in constructor
- Automatic database creation/deletion with opt-out flags
- `CancellationToken` passed to all database operations
- Proper async disposal

#### `WhenUsingBrowserV2<TEntryPoint>`
- No more blocking in constructor
- Configurable headless mode
- Better resource cleanup (page, browser, factory)
- Customizable browser options

---

## Migration Examples

### Example 1: Simple Synchronous Test

#### V1 Code (Still Works!)
```csharp
public class WhenCalculatingSum : WhenTestingFor<Calculator>
{
    private int _result;

    protected override Calculator For() => new Calculator();

    protected override void Given() { }

    protected override void When() => _result = Sut.Add(5, 3);

    [Fact]
    public void ShouldReturn8() => _result.ShouldBe(8);
}
```

#### V2 Code (Improved)
```csharp
public class WhenCalculatingSum : WhenTestingForV2<Calculator>
{
    private int _result;

    protected override Calculator For() => new Calculator();

    protected override void When() => _result = Sut.Add(5, 3);

    [Fact]
    [Then("Result should be 8")]
    public void ShouldReturn8() => _result.ShouldBe(8);

    // Bonus: Automatic cleanup if Calculator implements IDisposable
}
```

---

### Example 2: Async Test

#### V1 Code (Blocks in Constructor ??)
```csharp
public class WhenFetchingUser : WhenTestingForAsync<UserService>
{
    private User? _result;

    protected override UserService For() => new UserService(_httpClient);

    protected override async Task GivenAsync()
    {
        await _database.SeedUserAsync("test@example.com");
    }

    protected override async Task WhenAsync()
    {
        _result = await Sut.GetUserAsync("test@example.com");
    }

    [Fact]
    public void ShouldReturnUser() => _result.ShouldNotBeNull();
}
```

#### V2 Code (Proper IAsyncLifetime ?)
```csharp
public class WhenFetchingUser : WhenTestingForAsyncV2<UserService>
{
    private User? _result;

    protected override UserService For() => new UserService(_httpClient);

    protected override Task GivenAsync(CancellationToken ct)
        => _database.SeedUserAsync("test@example.com", ct);

    protected override async Task WhenAsync(CancellationToken ct)
    {
        _result = await Sut.GetUserAsync("test@example.com", ct);
    }

    [Fact]
    [Then("User should be returned")]
    public void ShouldReturnUser() => _result.ShouldNotBeNull();

    // Bonus: Can override TestCancellation for custom timeouts
}
```

---

### Example 3: Result Capture Pattern

#### V1 Code
```csharp
public class WhenFetchingUser : WhenTestingForWithResultAsync<UserService, User>
{
    protected override UserService For() => new UserService(_httpClient);

    protected override async Task GivenAsync()
    {
        await _database.SeedUserAsync("test@example.com");
    }

    protected override Task<User> WhenWithResultAsync()
        => Sut.GetUserAsync("test@example.com");

    [Fact]
    public void ShouldReturnUser() => Result.ShouldNotBeNull();
}
```

#### V2 Code (With CancellationToken)
```csharp
public class WhenFetchingUser : WhenTestingForWithResultAsyncV2<UserService, User>
{
    protected override UserService For() => new UserService(_httpClient);

    protected override Task GivenAsync(CancellationToken ct)
        => _database.SeedUserAsync("test@example.com", ct);

    protected override Task<User> WhenWithResultAsync(CancellationToken ct)
        => Sut.GetUserAsync("test@example.com", ct);

    [Fact]
    [Then("User should be returned")]
    public void ShouldReturnUser() => Result.ShouldNotBeNull();
}
```

---

### Example 4: Database Test

#### V1 Code (Blocks in Constructor ??)
```csharp
public class WhenSeedingAdmin : WhenUsingDatabase<GraphContext>
{
    private Node? _user;

    protected override async Task GivenAsync()
    {
        await Db.Use<AdminAgentUseCase>();
    }

    protected override async Task WhenAsync()
    {
        _user = await Db.Nodes.FindAsync(AdminAgentUseCase.UserId);
    }

    [Fact]
    public void UserExists() => _user.ShouldNotBeNull();
}
```

#### V2 Code (Proper IAsyncLifetime ?)
```csharp
public class WhenSeedingAdmin : WhenUsingDatabaseV2<GraphContext>
{
    private Node? _user;

    protected override Task GivenAsync(CancellationToken ct)
        => Db.Use<AdminAgentUseCase>(ct);

    protected override async Task WhenAsync(CancellationToken ct)
    {
        _user = await Db.Nodes.FindAsync(new[] { AdminAgentUseCase.UserId }, ct);
    }

    [Fact]
    [Then("User exists")]
    public void UserExists() => _user.ShouldNotBeNull();

    // Bonus: Can disable auto-cleanup for debugging
    // protected override bool AutoDeleteDatabase => false;
}
```

---

### Example 5: Database with Result Capture

#### V1 Code
```csharp
public class WhenSeedingAdmin : WhenUsingDatabaseWithResult<GraphContext, SeededAdmin>
{
    protected override async Task GivenAsync()
    {
        await Db.Use<AdminAgentUseCase>();
    }

    protected override async Task<SeededAdmin> WhenAsyncWithResult()
    {
        var user = await Db.Nodes.FindAsync(AdminAgentUseCase.UserId);
        var agent = await Db.Nodes.FindAsync(AdminAgentUseCase.AgentId);
        return new SeededAdmin(user, agent);
    }

    [Fact]
    public void UserExists() => Result.User.ShouldNotBeNull();
}
```

#### V2 Code (With CancellationToken)
```csharp
public class WhenSeedingAdmin : WhenUsingDatabaseWithResultV2<GraphContext, SeededAdmin>
{
    protected override Task GivenAsync(CancellationToken ct)
        => Db.Use<AdminAgentUseCase>(ct);

    protected override async Task<SeededAdmin> WhenWithResultAsync(CancellationToken ct)
    {
        var user = await Db.Nodes.FindAsync(new[] { AdminAgentUseCase.UserId }, ct);
        var agent = await Db.Nodes.FindAsync(new[] { AdminAgentUseCase.AgentId }, ct);
        return new SeededAdmin(user, agent);
    }

    [Fact]
    [Then("User exists", "VCHIP-4001-UAC001")]
    public void UserExists() => Result.User.ShouldNotBeNull();
}
```

---

### Example 6: Browser Test

#### V1 Code (Blocks in Constructor ??)
```csharp
public class WhenUserLogsIn : WhenUsingBrowser<Program>
{
    protected override async Task GivenAsync()
    {
        var db = Services.GetRequiredService<MyDbContext>();
        db.Users.Add(new User { Email = "test@example.com" });
        await db.SaveChangesAsync();
    }

    protected override async Task WhenAsync()
    {
        await Page.GotoAsync("https://localhost:5001/login");
        await Page.FillAsync("#email", "test@example.com");
        await Page.ClickAsync("button[type=submit]");
    }

    [Fact]
    public async Task ShouldRedirect()
    {
        await Page.WaitForURLAsync("**/dashboard");
    }
}
```

#### V2 Code (Proper IAsyncLifetime ?)
```csharp
public class WhenUserLogsIn : WhenUsingBrowserV2<Program>
{
    protected override async Task GivenAsync(CancellationToken ct)
    {
        var db = Services.GetRequiredService<MyDbContext>();
        db.Users.Add(new User { Email = "test@example.com" });
        await db.SaveChangesAsync(ct);
    }

    protected override async Task WhenAsync(CancellationToken ct)
    {
        await Page.GotoAsync("https://localhost:5001/login");
        await Page.FillAsync("#email", "test@example.com");
        await Page.ClickAsync("button[type=submit]");
    }

    [Fact]
    [Then("Should redirect to dashboard")]
    public async Task ShouldRedirect()
    {
        await Page.WaitForURLAsync("**/dashboard");
    }

    // Bonus: Can run with visible browser for debugging
    // protected override bool HeadlessBrowser => false;
}
```

---

## Migration Strategy

### Option 1: Gradual Migration (Recommended)
1. Keep all existing tests using V1 classes
2. Write all new tests using V2 classes
3. Migrate existing tests to V2 as you touch them

### Option 2: Targeted Migration
1. Identify tests that have issues (timeouts, flakiness)
2. Migrate those specific tests to V2
3. Leave stable tests on V1

### Option 3: Bulk Migration
Use find/replace:
- `WhenTestingFor<` ? `WhenTestingForV2<`
- `WhenTestingForAsync<` ? `WhenTestingForAsyncV2<`
- `WhenUsingDatabase<` ? `WhenUsingDatabaseV2<`
- `WhenUsingBrowser<` ? `WhenUsingBrowserV2<`
- `WhenTestingComponent<` ? `WhenTestingComponentV2<`
- Add `CancellationToken ct` parameters to `GivenAsync` and `WhenAsync` methods

---

## V1 vs V2 Quick Reference

| Feature | V1 | V2 |
|---------|----|----|
| Constructor blocking | ?? Yes (async classes) | ? No |
| IAsyncLifetime | ? No | ? Yes |
| CancellationToken | ? No | ? Yes |
| Naming | Mixed (Cut/Sut) | ? Sut everywhere |
| Disposal | ?? Manual | ? Automatic |
| Cleanup hooks | ? No | ? Yes |
| ConfigureAwait | ? Missing | ? Everywhere |
| Database auto-cleanup | ? No | ? Yes (configurable) |
| Browser options | ? Fixed | ? Configurable |

---

## Breaking Changes

**None!** All V1 classes remain unchanged. V2 classes are additive only.

### Minor API Differences (V2 only)

1. **CancellationToken parameters**
   - V1: `Task GivenAsync()`
   - V2: `Task GivenAsync(CancellationToken ct)`

2. **Property names**
   - V1: `Cut` (WhenTestingComponent)
   - V2: `Sut` (WhenTestingComponentV2)

3. **Result method names**
   - V1: `Task<TResult> WhenAsyncWithResult()`
   - V2: `Task<TResult> WhenWithResultAsync(CancellationToken ct)`

---

## Best Practices

### ? DO
- Use V2 for all new tests
- Pass `CancellationToken` to async methods that support it
- Override `CleanupAsync` for custom resource cleanup
- Use `AutoDeleteDatabase = false` when debugging database tests
- Use `HeadlessBrowser = false` when debugging browser tests

### ? DON'T
- Don't migrate working V1 tests unless you have a reason
- Don't ignore cancellation tokens in long-running operations
- Don't forget to call base cleanup if overriding disposal

---

## Troubleshooting

### Issue: Tests hang on initialization
**Solution:** You're probably calling blocking async in a constructor. Migrate to V2.

### Issue: Database not cleaning up
**Solution:** V2 has `AutoDeleteDatabase` flag. Check if it's overridden.

### Issue: Browser tests fail intermittently
**Solution:** V2 has better resource cleanup. Also check timeouts.

### Issue: Need to debug a test
**V1:** Add breakpoints and step through
**V2:** Set `HeadlessBrowser = false` or `AutoDeleteDatabase = false` for inspection

---

## Questions?

See the examples in the test project or refer to the XML documentation in each V2 class.
