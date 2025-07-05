using Xunit.Sdk;

namespace LowlandTech.Testing.Features.Attributes;

/// <summary>
/// Specifies that a method represents a "Then" step in a behavior-driven development (BDD) scenario.
/// </summary>
/// <example>
/// [Fact]
/// [Then("User node should exist")]
/// public void ItShouldHaveAUserNode() => _user.ShouldNotBeNull();
/// 
/// [Fact]
/// [Then("Agent node should exist")]
/// public void ItShouldHaveAnAgentNode() => _agent.ShouldNotBeNull();
/// 
/// [Fact]
/// [Then("User should have mnemonic")]
/// public void UserShouldHaveMnemonic() =>
///     _user!.Properties["Mnemonic"].ShouldNotBeNullOrWhiteSpace();
/// </example>
/// <remarks>This attribute is used to annotate methods that define the expected outcome or result of a test
/// scenario. It is typically used in conjunction with other BDD step attributes, such as "Given" and "When".</remarks>
/// <param name="description">A description of the "Then" step, typically written in natural language to describe the expected outcome.</param>
/// <param name="code">A code to store the User Acceptance Criteria code in the format of VCHIP-XXXX-UACXXX.</param>
[TraitDiscoverer("LowlandTech.Testing.Features.Attributes.ThenDiscoverer", "LowlandTech.Testing.Features")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ThenAttribute(string description, string? code = null) : Attribute, ITraitAttribute
{
    /// <summary>
    /// Gets the description associated with the current instance.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Gets or sets the code to store the User Acceptance Criteria code in the format of VCHIP-XXXX-UACXXX.
    /// </summary>
    public string? Code { get; set; } = code;
}