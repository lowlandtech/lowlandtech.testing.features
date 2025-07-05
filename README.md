# LowlandTech.Testing.Features

This library provides a clean, composable, and agent-compatible Given-When-Then testing framework for Vylyrian projects.

It supports:
- Pure logic testing without database I/O
- In-memory EF Core integration tests
- BIP-compliant metadata via `NodeId`, `TaskId`, and `UseCaseId` attributes
- Razor-based test templating for plugin use case coverage

---

## 🟢 Usage

After building or downloading the `ltx` console tool, you can generate a Markdown coverage report with:

```bash
ltx report ../MyTests.dll --file Coverage.md --title "My Report Title"
```

✅ **Arguments:**

- `<assembly>`: Path to your compiled test assembly (`.dll`)
- `--file`: Output file to write the Markdown report
- `--title`: Optional report title (defaults to **Feature Coverage Report**)

✅ **Example:**

```bash
ltx report ./bin/Debug/net8.0/MyTests.dll --file ./coverage.md --title "My Project Feature Coverage"
```

This generates `coverage.md`:

```markdown
# My Project Feature Coverage

## Simple scenario title

**Scenario Code:** `VCHIP-4001-SC001`

> **Given:** Given a simple context
> **When:** When an action occurs
> **Then:** Then an outcome is expected

**Test Class:** `ScenarioWithCodeAndTitle`

**Tags:** `VCHIP-4001-SC001`

- ✅ `ShouldBeTrue` — Outcome happened (`VCHIP-4001-UAC001`)
```

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
[Scenario(
    "VCHIP-4001-SC001",
    "Simple scenario title",
    "Given a simple context",
    "When an action occurs",
    "Then an outcome is expected")]
public class ScenarioWithCodeAndTitle : WhenTestingFor<int>
{
    protected override int For() => 0;
    protected override void When() { }
    [Fact]
    [Then("Outcome happened", "VCHIP-4001-UAC001")]
    public void ShouldBeTrue() { 0.ShouldBe(0); }
}

[Scenario("NodeId scenario", "When testing NodeId", "Then NodeId should be included")]
[NodeId("vy.test.nodeid")]
public class ScenarioWithNodeId : WhenTestingFor<int>
{
    protected override int For() => 0;
    protected override void When() { }
    [Fact]
    [Then("NodeId included")]
    public void ShouldIncludeNodeId() { }
}

[Scenario("TaskId scenario", "When testing TaskId", "Then TaskId should be included")]
[TaskId("VCHIP-4001-TK001")]
public class ScenarioWithTaskId : WhenTestingFor<int>
{
    protected override int For() => 0;
    protected override void When() { }
    [Fact]
    [Then("TaskId included")]
    public void ShouldIncludeTaskId() { }
}

[Scenario("UseCaseId scenario", "When testing UseCaseId", "Then UseCaseId should be included")]
[UseCaseId("VCHIP-4001-UC001")]
public class ScenarioWithUseCaseId : WhenTestingFor<int>
{
    protected override int For() => 0;
    protected override void When() { }
    [Fact]
    [Then("UseCaseId included")]
    public void ShouldIncludeUseCaseId() { }
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
