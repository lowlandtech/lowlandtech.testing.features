namespace LowlandTech.Testing.Features.Tests.VCHIP_4002;

[Scenario(
    "VCHIP-4002-SC05",
    "Show report branch help",
    "Given the CLI is available",
    "When running 'report --help'",
    "Then usage lists 'scenario' and 'run'")]
public sealed class SC05_WhenShowingReportHelp: WhenTestingForAsync<CliApp>
{
    private CommandAppResult? _result;
    protected override CliApp For() => new();

    protected override async Task WhenAsync()
        => _result = await Sut.RunAsync("report", "--help");

    [Fact]
    [Then("Shows 'scenario'", "VCHIP-4002-UAC050")]
    public void ShowsScenario() => _result!.Output.Contains("scenario").ShouldBeTrue();

    [Fact]
    [Then("Shows 'run'", "VCHIP-4002-UAC051")]
    public void ShowsRun() => _result!.Output.Contains("run").ShouldBeTrue();
}