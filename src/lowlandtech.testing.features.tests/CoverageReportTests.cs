namespace LowlandTech.Testing.Features.Tests;

public class CoverageReportTests
{
    [Fact]
    public void ItGeneratesCoverageReportContainingAllMetadata()
    {
        // Arrange
        var outputPath = "coverage.md";
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        assembly.GenerateCoverageReport(outputPath, "Test Coverage");

        var report = File.ReadAllText(outputPath);

        // Assert
        report.ShouldContain("VCHIP-4001-SC001");  // Scenario Code
        report.ShouldContain("Simple scenario title");
        report.ShouldContain("Given a simple context");
        report.ShouldContain("Outcome happened");
        report.ShouldContain("VCHIP-4001-UAC001");

        report.ShouldContain("vy.test.nodeid");
        report.ShouldContain("NodeId included");

        report.ShouldContain("VCHIP-4001-TK001");
        report.ShouldContain("TaskId included");

        report.ShouldContain("VCHIP-4001-UC001");
        report.ShouldContain("UseCaseId included");
    }
}
