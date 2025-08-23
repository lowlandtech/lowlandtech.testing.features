namespace LowlandTech.Testing.Features.Tests.VCHIP_4002;

[Scenario(
    "VCHIP-4002-SC02",
    "Generate coverage report from assembly",
    "Given a compiled test assembly exists",
    "When running coverage generate",
    "Then a markdown file is created with a title")]
public sealed class SC02_WhenGeneratingCoverageDoc : WhenTestingForAsync<CliApp>
{
    private CommandAppResult? _result;
    private string? _asm;
    private string? _out;

    protected override CliApp For() => new();

    protected override async Task WhenAsync()
    {
        _asm = typeof(SC02_WhenGeneratingCoverageDoc).Assembly.Location;
        _out = Path.Combine("docs", "coverage.md");
        Directory.CreateDirectory("docs");
        _result = await Sut.RunAsync(["coverage", "generate",
            "--assembly", _asm, "--out", _out, "--title", "Test Coverage"]);
    }

    [Fact]
    [Then("File is created", "VCHIP-4002-UAC020")]
    public void FileCreated() => File.Exists(_out).ShouldBeTrue();

    [Fact]
    [Then("File contains title", "VCHIP-4002-UAC021")]
    public void FileHasTitle() => File.ReadAllText(_out).ShouldContain("Test Coverage");
}
