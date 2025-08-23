namespace LowlandTech.Testing.Features.Tests;

[Scenario(
    "VCHIP-4001-SC009",
    "Generate Markdown Coverage Report",
    "Given a compiled test assembly",
    "When the coverage report is generated",
    "Then the report should contain all expected scenario metadata")]
public class WhenGeneratingCoverageReport : WhenTestingFor<string>
{
    protected override string For()
    {
        var outputPath = "coverage.md";
        var assembly = Assembly.GetExecutingAssembly();
        assembly.GenerateCoverageReport(outputPath, "Test Coverage");
        return File.ReadAllText(outputPath);
    }

    [Fact]
    [Then("The report should contain Scenario Code VCHIP-4001-SC001", "VCHIP-4001-UAC010")]
    public void ShouldContainScenarioCode()
    {
        Sut.ShouldContain("VCHIP-4001-SC001");
    }

    [Fact]
    [Then("The report should contain Simple scenario title", "VCHIP-4001-UAC011")]
    public void ShouldContainScenarioTitle()
    {
        Sut.ShouldContain("Simple scenario title");
    }

    [Fact]
    [Then("The report should contain the Given step", "VCHIP-4001-UAC012")]
    public void ShouldContainGivenStep()
    {
        Sut.ShouldContain("Given a simple context");
    }

    [Fact]
    [Then("The report should contain Outcome happened", "VCHIP-4001-UAC013")]
    public void ShouldContainOutcomeStep()
    {
        Sut.ShouldContain("Outcome happened");
    }

    [Fact]
    [Then("The report should contain UAC code VCHIP-4001-UAC001", "VCHIP-4001-UAC014")]
    public void ShouldContainUacCode()
    {
        Sut.ShouldContain("VCHIP-4001-UAC001");
    }

    [Fact]
    [Then("The report should contain NodeId vy.test.nodeid", "VCHIP-4001-UAC015")]
    public void ShouldContainNodeId()
    {
        Sut.ShouldContain("vy.test.nodeid");
    }

    [Fact]
    [Then("The report should contain NodeId included", "VCHIP-4001-UAC016")]
    public void ShouldContainNodeIdIncluded()
    {
        Sut.ShouldContain("NodeId included");
    }

    [Fact]
    [Then("The report should contain TaskId VCHIP-4001-TK001", "VCHIP-4001-UAC017")]
    public void ShouldContainTaskId()
    {
        Sut.ShouldContain("VCHIP-4001-TK001");
    }

    [Fact]
    [Then("The report should contain TaskId included", "VCHIP-4001-UAC018")]
    public void ShouldContainTaskIdIncluded()
    {
        Sut.ShouldContain("TaskId included");
    }

    [Fact]
    [Then("The report should contain UseCaseId VCHIP-4001-UC001", "VCHIP-4001-UAC019")]
    public void ShouldContainUseCaseId()
    {
        Sut.ShouldContain("VCHIP-4001-UC001");
    }

    [Fact]
    [Then("The report should contain UseCaseId included", "VCHIP-4001-UAC020")]
    public void ShouldContainUseCaseIdIncluded()
    {
        Sut.ShouldContain("UseCaseId included");
    }
}
