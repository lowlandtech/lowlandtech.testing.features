namespace LowlandTech.Testing.Features.Tests.VCHIP_4002;

[Scenario(
    "VCHIP-4002-SC03",
    "Report single scenario to disk",
    "Given a normalized payload",
    "When reporting to disk",
    "Then a doc is written with status")]
public sealed class SC03_WhenReportingScenarioToDisk
    : WhenTestingForAsync<CliApp>
{
    private string? _root;
    private string? _file;
    protected override CliApp For() => new();

    protected override async Task WhenAsync()
    {
        _root = Path.Combine("docs");
        Directory.CreateDirectory(_root);
        var id = "VCHIP-0001-SC01";
        _file = Path.Combine(_root, "vchip-0001", $"{id}.md");
        await Sut.RunAsync(["report", "scenario",
            "--id", id, "--status", "passed", "--to", "disk", "--out", _root, "--run-id", "demo"]);
    }

    [Fact]
    [Then("Disk file exists", "VCHIP-4002-UAC030")]
    public void FileExists() => File.Exists(_file).ShouldBeTrue();

    [Fact]
    [Then("Disk file contains status", "VCHIP-4002-UAC031")]
    public void StatusIsPresent()
        => File.ReadAllText(_file!).ShouldContain("status: passed");
}
