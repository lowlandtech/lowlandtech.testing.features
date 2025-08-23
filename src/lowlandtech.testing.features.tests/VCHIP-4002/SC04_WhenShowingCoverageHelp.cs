namespace LowlandTech.Testing.Features.Tests.VCHIP_4002;

[Scenario(
    "VCHIP-4002-SC04",
    "Show coverage branch help",
    "Given the CLI is available",
    "When running 'coverage --help'",
    "Then usage lists 'generate' subcommand")]
public sealed class SC04_WhenShowingCoverageHelp : WhenTestingForAsync<CliApp>
{
    private CommandAppResult? _result;
    protected override CliApp For() => new();

    protected override async Task WhenAsync()
        => _result = await Sut.RunAsync("coverage", "--help");

    [Fact]
    [Then("Shows 'generate' subcommand", "VCHIP-4002-UAC040")]
    public void ShowsGenerate() => _result!.Output.ShouldContain("generate", Case.Insensitive);
}
