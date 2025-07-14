namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Provides a base class for integration tests that involve browser interactions using Playwright.
/// </summary>

/// <example>
/// [Given("A user account exists in the system")]
/// [When("The user logs in via the login page")]
/// public class WhenUserLogsIn : WhenUsingBrowser<Program>
/// {
///     protected override string GivenDescription => "A user account exists in the system";
///     protected override string WhenDescription => "The user logs in via the login page";
/// 
///     protected override async Task GivenAsync()
///     {
///         var db = Services.GetRequiredService<MyDbContext>();
///         db.Users.Add(new User { Email = "test@example.com", Password = "123456" });
///         await db.SaveChangesAsync();
///     }
/// 
///     protected override async Task WhenAsync()
///     {
///         await Page.GotoAsync("https://localhost:5001/login");
///         await Page.FillAsync("#email", "test@example.com");
///         await Page.FillAsync("#password", "123456");
///         await Page.ClickAsync("button[type=submit]");
///     }
/// 
///     [Fact]
///     [Then("The user should be redirected to the dashboard")]
///     public async Task ShouldRedirectToDashboard()
///     {
///         await Page.WaitForURLAsync("**/dashboard");
///         (await Page.Url).Should().Contain("/dashboard");
///     }
/// 
///     [Fact]
///     [Then("A welcome message should be visible")]
///     public async Task ShouldShowWelcomeMessage()
///     {
///         var text = await Page.InnerTextAsync("h1");
///         text.Should().Contain("Welcome");
///     }
/// }
/// </example>

/// <remarks>This class sets up a test environment that includes a web application host, an HTTP client,  and a
/// Playwright browser instance. Derived classes must implement the <see cref="GivenAsync"/>  and <see
/// cref="WhenAsync"/> methods to define the test setup and execution logic.</remarks>
/// <typeparam name="TEntryPoint">The entry point class of the application under test, typically the startup class of the web application.</typeparam>
public abstract class WhenUsingBrowser<TEntryPoint>
    where TEntryPoint : class
{
    protected IWebHost? Host;
    protected HttpClient HttpClient = null!;
    protected IBrowser Browser = null!;
    protected IPage Page = null!;
    protected IServiceProvider Services = null!;
    protected abstract Task GivenAsync();
    protected abstract Task WhenAsync();

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenUsingBrowser"/> class.
    /// </summary>
    /// <remarks>This constructor performs asynchronous setup operations synchronously by invoking <see
    /// cref="SetupAsync"/>  and blocking on its result. This may lead to potential deadlocks in certain synchronization
    /// contexts. Consider using an alternative approach if asynchronous initialization is required.</remarks>
    public WhenUsingBrowser()
    {
        SetupAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Sets up the necessary resources and dependencies for the test environment asynchronously.
    /// </summary>
    /// <remarks>This method initializes the web application factory, HTTP client, service provider, and 
    /// Playwright browser and page instances. It also invokes the <c>GivenAsync</c> and <c>WhenAsync</c>  methods to
    /// prepare the test scenario.</remarks>
    /// <returns></returns>
    private async Task SetupAsync()
    {
        var factory = new WebApplicationFactory<TEntryPoint>();
        Host = factory.Server.Host;
        HttpClient = factory.CreateClient();
        Services = factory.Services;

        using var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        Page = await Browser.NewPageAsync();

        await GivenAsync();
        await WhenAsync();
    }
}
