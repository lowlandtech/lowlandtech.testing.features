# LowlandTech.Testing.Features

This library provides a clean, composable, and agent-compatible Given-When-Then testing framework for Vylyrian projects.

It supports:
- Pure logic testing without database I/O
- In-memory EF Core integration tests
- BIP-compliant metadata via `NodeId`, `TaskId`, and `UseCaseId` attributes
- Razor-based test templating for plugin use case coverage

---

## 🧩 Key Components

### ✅ Base Classes

- `WhenTestingFor<T>` — lightweight, no I/O unit test base
- `WhenUsingDatabase<TContext>` — integration test base for EF Core or `GraphContext`

### ✅ Attributes

| Attribute     | Description                                 |
|---------------|---------------------------------------------|
| `[Scenario]`  | Defines `Given`, `When`, `Then` text for documentation and templating |
| `[Given]`     | Marks the setup context                    |
| `[When]`      | Describes the trigger or action            |
| `[Then]`      | Declares an outcome as a separate `[Fact]` |
| `[NodeId]`    | Binds the test to a `vy.usecase` or `vy.test` node |
| `[TaskId]`    | Links the test to a `vy.task` for BIP credit |
| `[UseCaseId]` | Optionally groups the test with a broader use case |

---

## 🔨 Installation

```bash
dotnet add package LowlandTech.Testing.Features
```

---

## 🧪 Example Usage

```csharp
[Scenario("A superadmin user and agent are seeded",
          "The graph is queried for nodes and relationships",
          "The user and agent should be linked with correct properties")]
[NodeId("a5f7...")]
[TaskId("VCHIP-2040-TK01")]
[UseCaseId("VCHIP-2040-UC01")]
public class WhenSeedingSuperadmin : WhenUsingDatabase<GraphContext>
{
    private Node? _user;
    private Node? _agent;

    protected override async Task GivenAsync() => await Db.Use<AdminAgentUseCase>();
    protected override async Task WhenAsync()
    {
        _user = await Db.Nodes.FindAsync(AdminAgentUseCase.UserId);
        _agent = await Db.Nodes.FindAsync(AdminAgentUseCase.AgentId);
    }

    [Fact]
    [Then("User and agent should be linked")]
    public void ItShouldLinkUserToAgent()
    {
        _user.ShouldNotBeNull();
        _agent.ShouldNotBeNull();
        _user!["Role"].ShouldBe("admin");
    }
}
```

---

## 🧱 Folder Structure

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
│   └── WhenUsingDatabase.cs
```

---

## 📚 Related Features

- [`VCHIP-1100` IUseCase Pattern](/docs/VCHIP-1100%20IUseCase%20Pattern.md)
- [`VCHIP-1102` NodeId Attribute Convention](/docs/VCHIP-1102%20NodeId%20Attribute%20Convention.md)
- [`VCHIP-1103` TaskId Attribute Convention](/docs/VCHIP-1103%20TaskId%20Attribute%20Convention.md)
- [`VCHIP-1106` UseCaseId Attribute Convention](/docs/VCHIP-1106%20UseCaseId%20Attribute%20Convention.md)
