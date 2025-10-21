# V2 Base Classes - Quick Reference Card

## ?? When to Use Which Class

| Scenario | Use This V2 Class |
|----------|-------------------|
| Testing pure logic (sync) | `WhenTestingForV2<T>` |
| Testing pure logic (async) | `WhenTestingForAsyncV2<T>` |
| Testing logic with result (sync) | `WhenTestingForWithResultV2<TSut, TResult>` |
| Testing logic with result (async) | `WhenTestingForWithResultAsyncV2<TSut, TResult>` |
| Testing with EF Core database | `WhenUsingDatabaseV2<TContext>` |
| Testing database with result | `WhenUsingDatabaseWithResultV2<TContext, TResult>` |
| Testing Blazor components | `WhenTestingComponentV2<T>` |
| Testing with browser (Playwright) | `WhenUsingBrowserV2<TEntryPoint>` |

---

## ?? Basic Template Patterns

### 1. Simple Async Test
```csharp
public class WhenDoingSomething : WhenTestingForAsyncV2<MyService>
{
    protected override MyService For() => new MyService();
    
    protected override Task GivenAsync(CancellationToken ct)
    {
        // Setup preconditions
        return Task.CompletedTask;
    }
    
    protected override Task WhenAsync(CancellationToken ct)
    {
        // Execute action under test
        return Sut.DoSomethingAsync(ct);
    }
    
    [Fact]
    [Then("Should do something")]
    public void ShouldDoSomething()
    {
        // Assert
    }
}
```

### 2. Async Test with Result Capture
```csharp
public class WhenFetching : WhenTestingForWithResultAsyncV2<MyService, MyResult>
{
    protected override MyService For() => new MyService();
    
    protected override Task GivenAsync(CancellationToken ct)
    {
        // Setup
        return Task.CompletedTask;
    }
    
    protected override Task<MyResult> WhenWithResultAsync(CancellationToken ct)
        => Sut.FetchAsync(ct);
    
    [Fact]
    [Then("Result should not be null")]
    public void ResultShouldNotBeNull()
    {
        Result.ShouldNotBeNull();
    }
}
```

### 3. Database Test
```csharp
public class WhenSaving : WhenUsingDatabaseV2<MyDbContext>
{
    protected override Task GivenAsync(CancellationToken ct)
    {
        // Seed data
        Db.Entities.Add(new Entity { Name = "Test" });
        return Db.SaveChangesAsync(ct);
    }
    
    protected override async Task WhenAsync(CancellationToken ct)
    {
        // Execute action
        var entity = await Db.Entities.FirstAsync(ct);
        entity.Name = "Updated";
        await Db.SaveChangesAsync(ct);
    }
    
    [Fact]
    [Then("Should update entity")]
    public async Task ShouldUpdate()
    {
        var entity = await Db.Entities.FirstAsync();
        entity.Name.ShouldBe("Updated");
    }
}
```

### 4. Database Test with Result
```csharp
public class WhenQuerying : WhenUsingDatabaseWithResultV2<MyDbContext, MyEntity>
{
    protected override Task GivenAsync(CancellationToken ct)
    {
        Db.Entities.Add(new MyEntity { Name = "Test" });
        return Db.SaveChangesAsync(ct);
    }
    
    protected override Task<MyEntity> WhenWithResultAsync(CancellationToken ct)
        => Db.Entities.FirstAsync(ct);
    
    [Fact]
    [Then("Should return entity")]
    public void ShouldReturnEntity()
    {
        Result.Name.ShouldBe("Test");
    }
}
```

### 5. Browser Test
```csharp
public class WhenLoggingIn : WhenUsingBrowserV2<Program>
{
    protected override async Task GivenAsync(CancellationToken ct)
    {
        // Seed test user
        var db = Services.GetRequiredService<MyDbContext>();
        db.Users.Add(new User { Email = "test@test.com" });
        await db.SaveChangesAsync(ct);
    }
    
    protected override async Task WhenAsync(CancellationToken ct)
    {
        await Page.GotoAsync("https://localhost:5001/login");
        await Page.FillAsync("#email", "test@test.com");
        await Page.FillAsync("#password", "password");
        await Page.ClickAsync("button[type=submit]");
    }
    
    [Fact]
    [Then("Should redirect to dashboard")]
    public async Task ShouldRedirect()
    {
        await Page.WaitForURLAsync("**/dashboard");
    }
}
```

---

## ?? Common Customizations

### Custom Timeout
```csharp
protected override CancellationToken TestCancellation
{
    get
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        return cts.Token;
    }
}
```

### Database Debugging (Don't Delete DB)
```csharp
protected override bool AutoDeleteDatabase => false;
```

### Browser Debugging (Visible Browser)
```csharp
protected override bool HeadlessBrowser => false;
```

### Custom Cleanup
```csharp
protected override async Task CleanupAsync(CancellationToken ct)
{
    // Your cleanup code
    await base.CleanupAsync(ct);
}
```

### Custom Database Context Creation
```csharp
protected override MyDbContext CreateContext()
{
    var options = new DbContextOptionsBuilder<MyDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
    return new MyDbContext(options);
}
```

---

## ? Quick Tips

### DO's ?
- Use V2 for all new tests
- Pass `CancellationToken` to methods that support it
- Use result capture patterns when testing async operations
- Override `CleanupAsync` for custom resource cleanup
- Use `AutoDeleteDatabase = false` when debugging DB tests
- Use `HeadlessBrowser = false` when debugging browser tests

### DON'Ts ?
- Don't use V1 classes for new tests (unless you have a specific reason)
- Don't ignore `CancellationToken` in long-running operations
- Don't forget to await async operations
- Don't block on async code (no `.Result` or `.Wait()`)

---

## ?? V1 to V2 Quick Conversion

### Method Signature Changes

| V1 | V2 |
|----|-----|
| `Task GivenAsync()` | `Task GivenAsync(CancellationToken ct)` |
| `Task WhenAsync()` | `Task WhenAsync(CancellationToken ct)` |
| `Task<TResult> WhenAsyncWithResult()` | `Task<TResult> WhenWithResultAsync(CancellationToken ct)` |

### Property Name Changes

| V1 | V2 |
|----|-----|
| `Cut` (WhenTestingComponent) | `Sut` |
| `Sut` (other classes) | `Sut` ? (no change) |

---

## ?? Troubleshooting

### "Cannot resolve TestCancellation"
Add property:
```csharp
protected override CancellationToken TestCancellation => CancellationToken.None;
```

### "Database not being cleaned up"
Check if `AutoDeleteDatabase` is overridden:
```csharp
protected override bool AutoDeleteDatabase => true; // Should be true
```

### "Test hangs forever"
- Make sure you're passing `ct` to async methods
- Check for deadlocks in async code
- Add timeout via `TestCancellation`

### "Browser test fails"
- Check if Playwright browsers are installed: `pwsh bin/Debug/net9.0/playwright.ps1 install`
- Try with `HeadlessBrowser = false` to see what's happening

---

## ?? Further Reading

- See `MIGRATION_GUIDE_V2.md` for detailed migration examples
- See `V2_IMPLEMENTATION_SUMMARY.md` for technical details
- Check XML documentation in IntelliSense for method details

---

## ?? Training Examples

### Example 1: Calculator Test
```csharp
public class WhenAdding : WhenTestingForWithResultV2<Calculator, int>
{
    protected override Calculator For() => new Calculator();
    protected override int WhenWithResult() => Sut.Add(2, 3);
    
    [Fact] public void ShouldBe5() => Result.ShouldBe(5);
}
```

### Example 2: Async Repository Test
```csharp
public class WhenGettingUser : WhenTestingForWithResultAsyncV2<UserRepository, User>
{
    protected override UserRepository For() => new UserRepository(_db);
    
    protected override Task GivenAsync(CancellationToken ct)
    {
        _db.Users.Add(new User { Id = 1, Name = "Test" });
        return _db.SaveChangesAsync(ct);
    }
    
    protected override Task<User> WhenWithResultAsync(CancellationToken ct)
        => Sut.GetByIdAsync(1, ct);
    
    [Fact] public void ShouldReturnUser() => Result.Name.ShouldBe("Test");
}
```

---

**Last Updated:** $(Get-Date -Format "yyyy-MM-dd")
**Version:** 2025.10.1
