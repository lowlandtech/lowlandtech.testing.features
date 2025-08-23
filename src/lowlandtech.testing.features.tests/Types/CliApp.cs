namespace LowlandTech.Testing.Features.Tests.Types;

public sealed class CliApp
{
    public IServiceCollection? Services { get; set; }

    private static readonly SemaphoreSlim AnsiGate = new(1, 1);

    public async Task<CommandAppResult> RunAsync(params string[] args)
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Ansi = false;
        console.Profile.Capabilities.Legacy = false;

        await AnsiGate.WaitAsync();
        var prev = AnsiConsole.Console;
        try
        {
            AnsiConsole.Console = console;
            var app = ReporterTestFactory.Build(Services);
            return await app.RunAsync(args);
        }
        finally
        {
            AnsiConsole.Console = prev;
            AnsiGate.Release();
        }
    }
}
