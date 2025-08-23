namespace LowlandTech.Testing.Features.Tests.VCHIP_4001;

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
