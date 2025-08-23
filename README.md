# LowlandTech.Testing.Features

This library provides a clean, composable, and agent-compatible **Given–When–Then (GWT)** testing framework for Vylyrian/LowlandTech projects.

It supports:

* Pure logic testing without database I/O
* EF Core integration tests with **relational** semantics (SQLite `:memory:`)
* **Result-capturing bases** for sync/async & EF (typed `Result` with a sealed single-`When` site)
* BIP-style traceability via `Scenario`, `UseCaseId`, etc.

---

## 🆕 What’s new (non-breaking)

Three optional base classes that capture a typed **`Result`** while preserving your existing **For / Given / When / Then** pattern:

* `WhenTestingForWithResult<TSut, TResult>` *(sync)*
* `WhenTestingForWithResultAsync<TSut, TResult>` *(async)*
* `WhenUsingDatabaseWithResult<TContext, TResult>` *(EF Core / relational in-memory)*

**Why**

* Reduce per-test boilerplate (no ad‑hoc `_result` fields)
* Enforce a single `When` site (sealed override) without adding a new “Act” verb
* Keep specs readable and aligned with current GWT style

**Breaking changes**: none

---

## 🧩 Key Components

### Core bases

* `WhenTestingFor<T>` — lightweight, no I/O unit test base (sync)
* `WhenTestingForAsync<T>` — async unit test base
* `WhenUsingDatabase<TContext>` — EF Core integration base

### Result-capturing (opt‑in)

* `WhenTestingForWithResult<TSut, TResult>` — sync SUT; `When` returns a value captured in `Result`
* `WhenTestingForWithResultAsync<TSut, TResult>` — async SUT; `When` returns a value captured in `Result`
* `WhenUsingDatabaseWithResult<TContext, TResult>` — EF Core; `When` returns a value captured in `Result`

### EF in‑memory behavior (important)

`WhenUsingDatabase<TContext>` and `WhenUsingDatabaseWithResult<TContext, TResult>` default to **SQLite `:memory:`** via a **single open connection per test instance** (no `cache=shared`). This yields **relational semantics** (FKs, indexes, constraints). Supply a connection string in a derived class to switch to file‑backed SQLite or another provider (e.g., Npgsql/SqlServer).

---

## 🧪 Result‑Capturing Examples (minimal)

### Non‑DB (sync)

```csharp
[Scenario("VCHIP-5001-SC001", "Greet user", "Given a Greeter", "When greeting Wendell", "Then message is correct")]
public sealed class WhenGreetingAUser
  : WhenTestingForWithResult<Greeter, string>
{
    protected override Greeter For() => new Greeter(prefix: "Hello");
    protected override Task GivenAsync() => Task.CompletedTask;

    protected override Task<string> WhenWithResult()
        => Task.FromResult(Sut.Greet("Wendell"));

    [Fact]
    [Then("Returns 'Hello, Wendell!'", "VCHIP-5001-UAC001")]
    public void MessageIsCorrect() => Result.ShouldBe("Hello, Wendell!");
}
```

### Non‑DB (async SUT)

```csharp
[Scenario("VCHIP-5002-SC001", "Lookup", "Given a Repository", "When fetching by id", "Then returns entity")]
public sealed class WhenFetchingById
  : WhenTestingForWithResultAsync<MyRepo, MyEntity>
{
    private InMemoryStore _store = default!;

    protected override MyRepo For() => new MyRepo(_store);

    protected override async Task GivenAsync()
    {
        _store = new InMemoryStore();
        await _store.SeedAsync();
    }

    protected override Task<MyEntity> WhenWithResultAsync()
        => Sut.GetAsync(MyEntityIds.Known);

    [Fact]
    [Then("Returns known entity", "VCHIP-5002-UAC001")]
    public void ReturnsEntity() => Result.Id.ShouldBe(MyEntityIds.Known);
}
```

### EF Core (relational in‑memory)

```csharp
[Scenario(
  "VCHIP-3049-SC009",
  "Build full flow",
  "Given FlowBuilderUseCase is seeded",
  "When building/saving a flow",
  "Then it contains all components")]
public sealed class WhenBuildingAndSavingFlow
  : WhenUsingDatabaseWithResult<FlowContext, FlowScheme>
{
    protected override async Task GivenAsync() => await Db.Use<FlowBuilderUseCase>();

    protected override async Task<FlowScheme> WhenAsyncWithResult()
    {
        new FlowBuilder(context: Db)
          .WithId(FlowBuilderUseCase.FlowId)
          .WithName("Full Flow")
          .WithDescription("Complete test case")
          .AddTrigger(TriggerTypes.ObjectCreated)
          .AddRoutingRule("Workspace", "TemplateId", "abc123")
          .AddStep("Initialize").AddStep("Execute")
          .Save();

        return await Db.Schemes
          .Include(s => s.Triggers)
          .Include(s => s.RoutingRules)
          .Include(s => s.Steps)
          .SingleAsync(s => s.Id == FlowBuilderUseCase.FlowId);
    }

    [Fact]
    [Then("Has 2 steps", "VCHIP-3049-UAC014")]
    public void HasTwoSteps() => Result.Steps.Count.ShouldBe(2);
}
```

---

## 🔧 Installation

```bash
# From repo root
dotnet build
```

---

## 🧪 Example Usage (core bases)

> The examples above show the result‑capturing variants. Below is a classic core‑base example for comparison.

```csharp
[Scenario(
    "VCHIP-4001-SC001",
    "Create node",
    "Given the context is seeded",
    "When creating a node",
    "Then the node exists")]
public sealed class WhenCreatingNode : WhenUsingDatabase<GraphContext>
{
    private Node? _node;

    protected override async Task GivenAsync()
    {
        await Db.Use<SeedNodesUseCase>();
    }

    protected override async Task WhenAsync()
    {
        _node = await Db.Nodes.FindAsync(SeedNodesUseCase.NodeId);
    }

    [Fact]
    [Then("Node exists", "VCHIP-4001-UAC001")]
    public void NodeExists() => _node.ShouldNotBeNull();
}
```

---

## 🧱 Suggested Folder Structure

```
/src/lowlandtech.testing.features/
├── Attributes/
│   ├── ScenarioAttribute.cs
│   ├── GivenAttribute.cs
│   ├── WhenAttribute.cs
│   ├── ThenAttribute.cs
│   ├── NodeIdAttribute.cs
│   ├── TaskIdAttribute.cs
│   └── UseCaseIdAttribute.cs
├── Base/
│   ├── WhenTestingFor.cs
│   ├── WhenTestingForAsync.cs
│   ├── WhenUsingDatabase.cs
│   ├── WhenTestingForWithResult.cs
│   ├── WhenTestingForWithResultAsync.cs
│   └── WhenUsingDatabaseWithResult.cs
```

---

## 📎 Notes & Conventions

* **Single Act**: `When` is sealed in result‑capturing bases to enforce one behavior per scenario.
* **Relational in‑memory**: DB bases use a single open SQLite `:memory:` connection per test instance (no `cache=shared`). Override the provider as needed in a derived class.
* **Traceability**: Tag scenarios with `VCHIP-XXXX-UCYYY/SCZZZ` in attributes to keep doc ↔ test ↔ code alignment.
