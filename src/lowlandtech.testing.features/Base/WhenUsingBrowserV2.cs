namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Provides a base class for browser-based integration tests using Playwright with proper lifecycle management.
/// </summary>
/// <remarks>
/// This is an improved version of <see cref="WhenUsingBrowser{TEntryPoint}"/> with:
/// - Proper IAsyncLifetime implementation (no blocking in constructor)
/// - CancellationToken support
/// - Proper async disposal of all resources
/// - Better resource cleanup (browser, page, etc.)
/// - Consistent naming conventions
/// </remarks>
/// <typeparam name="TEntryPoint">The entry point class of the application under test.</typeparam>
public abstract class WhenUsingBrowserV2<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private WebApplicationFactory<TEntryPoint>? _factory;
    private IPlaywright? _playwright;

    /// <summary>
    /// Represents the web host used to configure and run the application.
    /// </summary>
    protected IWebHost? Host { get; private set; }

    /// <summary>
    /// Provides an instance of <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    protected HttpClient HttpClient { get; private set; } = null!;

    /// <summary>
    /// Represents the browser instance used for performing browser-based operations.
    /// </summary>
    protected IBrowser Browser { get; private set; } = null!;

    /// <summary>
    /// Represents the current page context within the application.
    /// </summary>
    protected IPage Page { get; private set; } = null!;

    /// <summary>
    /// Provides access to the application's service provider.
    /// </summary>
    protected IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Optional cancellation token for test timeout scenarios.
    /// </summary>
    protected virtual CancellationToken TestCancellation => CancellationToken.None;

    /// <summary>
    /// Controls whether to run the browser in headless mode.
    /// Default is true. Override to false for debugging.
    /// </summary>
    protected virtual bool HeadlessBrowser => true;

    /// <summary>
    /// Allows customization of browser launch options.
    /// </summary>
    protected virtual BrowserTypeLaunchOptions GetBrowserOptions() => new()
    {
        Headless = HeadlessBrowser
    };

    /// <summary>
    /// Allows customization of the WebApplicationFactory.
    /// Override this to configure test services, authentication, etc.
    /// </summary>
    protected virtual WebApplicationFactory<TEntryPoint> CreateFactory()
        => new WebApplicationFactory<TEntryPoint>();

    /// <summary>
    /// Prepares the necessary preconditions or initial state for a test scenario.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task GivenAsync(CancellationToken ct);

    /// <summary>
    /// Executes the action under test.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task WhenAsync(CancellationToken ct);

    /// <summary>
    /// Provides an opportunity to perform additional cleanup before disposal.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task CleanupAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Initializes the test lifecycle (called by xUnit).
    /// Sets up the web application, browser, and executes Given ? When.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        var ct = TestCancellation;

        // Setup web application
        _factory = CreateFactory();
        Host = _factory.Server.Host;
        HttpClient = _factory.CreateClient();
        Services = _factory.Services;

        // Setup Playwright browser
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(GetBrowserOptions());
        Page = await Browser.NewPageAsync();

        // Execute test lifecycle
        await GivenAsync(ct).ConfigureAwait(false);
        await WhenAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Cleanup resources (called by xUnit after all tests in this class complete).
    /// Disposes browser, page, HTTP client, and web application factory.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        await CleanupAsync(TestCancellation).ConfigureAwait(false);

        // Dispose in reverse order of creation
        if (Page != null)
            await Page.CloseAsync().ConfigureAwait(false);

        if (Browser != null)
            await Browser.CloseAsync().ConfigureAwait(false);

        _playwright?.Dispose();

        HttpClient?.Dispose();

        if (_factory != null)
            await _factory.DisposeAsync().ConfigureAwait(false);
    }
}
