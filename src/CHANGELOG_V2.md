# Changelog

## [2025.10.1] - 2025-01-XX

### Added - V2 Base Classes (100% Backward Compatible)

#### ?? New V2 Base Classes
- `WhenTestingForV2<T>` - Improved synchronous testing with proper disposal
- `WhenTestingForAsyncV2<T>` - Async testing with IAsyncLifetime (no constructor blocking)
- `WhenTestingForWithResultV2<TSut, TResult>` - Sync testing with result capture
- `WhenTestingForWithResultAsyncV2<TSut, TResult>` - Async testing with result capture
- `WhenUsingDatabaseV2<TContext>` - Database testing with automatic lifecycle management
- `WhenUsingDatabaseWithResultV2<TContext, TResult>` - Database testing with result capture
- `WhenTestingComponentV2<T>` - bUnit component testing with standardized naming (Sut)
- `WhenUsingBrowserV2<TEntryPoint>` - Playwright browser testing with proper async lifecycle

#### ? Key V2 Features
- **No Blocking**: Removed `.GetAwaiter().GetResult()` from constructors
- **IAsyncLifetime**: Proper xUnit v3 implementation returning `ValueTask`
- **CancellationToken**: Full support throughout all async methods
- **Consistent Naming**: All V2 classes use `Sut` (System Under Test)
- **Auto Disposal**: Automatic cleanup of disposable resources
- **Database Lifecycle**: Automatic database creation/deletion with opt-out flags
- **Browser Options**: Configurable headless mode and browser options
- **Cleanup Hooks**: Virtual `CleanupAsync()` methods for custom resource management

#### ?? Documentation
- `MIGRATION_GUIDE_V2.md` - Comprehensive migration guide with before/after examples
- `V2_IMPLEMENTATION_SUMMARY.md` - Technical implementation details
- `V2_QUICK_REFERENCE.md` - Quick reference card for developers

### Changed
- Updated `publish-nuget.ps1` to handle missing icon file gracefully
- Version bumped to 2025.10.1

### Technical Details
- All V2 classes implement proper async patterns with `ConfigureAwait(false)`
- Compatible with xUnit v3 `IAsyncLifetime` interface
- Full support for .NET 9
- Zero breaking changes - all V1 classes remain unchanged

### Migration Path
Three migration strategies provided:
1. **Gradual** (Recommended): Use V2 for new tests, keep V1 for existing
2. **Targeted**: Migrate problematic tests first
3. **Bulk**: Find/replace class names (with minor method signature updates)

### Backward Compatibility
? **100% Backward Compatible**
- All V1 classes unchanged
- All existing tests continue to work
- V2 is purely additive
- No API changes to existing functionality

### Performance Improvements
- No more constructor blocking in async test classes
- Better resource cleanup (no leaks)
- Proper cancellation support for long-running tests

---

## [2025.7.1] - 2025-01-XX (Previous Version)

### Initial Release
- Basic Given-When-Then testing framework
- `WhenTestingFor<T>` - Synchronous testing
- `WhenTestingForAsync<T>` - Asynchronous testing
- `WhenTestingForWithResult<TSut, TResult>` - Result capture pattern
- `WhenTestingForWithResultAsync<TSut, TResult>` - Async result capture
- `WhenUsingDatabase<TContext>` - EF Core integration
- `WhenUsingDatabaseWithResult<TContext, TResult>` - Database with result capture
- `WhenTestingComponent<T>` - bUnit component testing
- `WhenTestingComponentAsync<T>` - Async bUnit component testing
- `WhenUsingBrowser<TEntryPoint>` - Playwright browser testing

### Attributes
- `[Scenario]` - BDD scenario description
- `[Given]` - Precondition description
- `[When]` - Action description
- `[Then]` - Outcome description with UAC code support
- `[NodeId]` - Graph node binding
- `[TaskId]` - BIP task linking
- `[UseCaseId]` - Use case association
- `[SpecificationId]` - Specification reference

### Helpers
- `DelegateDbContextFactory` - Dynamic context creation
- `TestLogger` - Test logging utilities

---

## Migration Notes

### From 2025.7.1 to 2025.10.1

No changes required! The update is 100% backward compatible.

**To adopt V2 features:**
1. For new tests, use V2 base classes (e.g., `WhenTestingForAsyncV2<T>`)
2. Add `CancellationToken ct` parameters to `GivenAsync` and `WhenAsync` methods
3. See `MIGRATION_GUIDE_V2.md` for detailed examples

**Benefits of migrating to V2:**
- No more constructor blocking
- Better resource cleanup
- Timeout support via CancellationToken
- Debugging options (AutoDeleteDatabase, HeadlessBrowser)
- Future-proof for .NET 10+

---

## Known Issues

### V1 Classes (Not Fixed - Use V2 Instead)
- ?? `WhenTestingForAsync<T>` blocks in constructor
- ?? `WhenUsingDatabase<T>` blocks in constructor
- ?? `WhenUsingBrowser<T>` blocks in constructor
- ?? No CancellationToken support
- ?? Manual resource cleanup required

### V2 Classes
- ? All issues resolved

---

## Upgrade Guide

### Step 1: Update Package
```bash
dotnet add package LowlandTech.Testing.Features --version 2025.10.1
```

### Step 2: Choose Your Strategy

#### Option A: Keep Using V1 (No Changes)
```csharp
public class MyTest : WhenTestingFor<MyService>
{
    // No changes needed - continues to work
}
```

#### Option B: Adopt V2 for New Tests
```csharp
public class MyNewTest : WhenTestingForV2<MyService>
{
    // New tests use V2
}
```

#### Option C: Migrate Existing Tests
```csharp
// Before (V1)
public class MyTest : WhenTestingForAsync<MyService>
{
    protected override async Task GivenAsync() { }
    protected override async Task WhenAsync() { }
}

// After (V2)
public class MyTest : WhenTestingForAsyncV2<MyService>
{
    protected override Task GivenAsync(CancellationToken ct) => Task.CompletedTask;
    protected override Task WhenAsync(CancellationToken ct) => Task.CompletedTask;
}
```

---

## Statistics

- **New Classes:** 8
- **New Documentation Files:** 3
- **Lines of Code Added:** ~1,500
- **Breaking Changes:** 0
- **Tests Affected:** 0 (all existing tests continue to work)
- **Performance Impact:** Positive (no more blocking)

---

## Contributors

- @wendellmva - V2 implementation, documentation, backward compatibility

---

## See Also

- [Migration Guide](MIGRATION_GUIDE_V2.md)
- [Implementation Summary](V2_IMPLEMENTATION_SUMMARY.md)
- [Quick Reference](V2_QUICK_REFERENCE.md)
- [README](README.md)
