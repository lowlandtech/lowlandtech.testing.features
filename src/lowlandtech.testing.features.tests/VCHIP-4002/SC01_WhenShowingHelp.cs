namespace LowlandTech.Testing.Features.Tests.VCHIP_4002;

[Scenario(
    "VCHIP-4002-SC01",
    "Show global help",
    "Given the CLI is available",
    "When running --help",
    "Then usage lists top-level commands")]
public sealed class SC01_WhenShowingHelp(ITestOutputHelper output) : WhenTestingForAsync<CliApp>
{
    private CommandAppResult? _result;
    protected override CliApp For() => new();

    protected override async Task WhenAsync()
        => _result = await Sut.RunAsync("--help");

    [Fact]
    [Then("Shows 'coverage' command", "VCHIP-4002-UAC010")]
    public void ShowsCoverage() => _result!.Output.Contains("coverage").ShouldBeTrue();

    [Fact]
    [Then("Shows 'report' command", "VCHIP-4002-UAC011")]
    public void ShowsReport() => _result!.Output.Contains("report").ShouldBeTrue();
}
