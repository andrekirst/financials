namespace Camtify.Tests.Common.AutoFixture.Attributes;

/// <summary>
/// InlineAutoData attribute configured with Camtify domain customizations.
/// Use this attribute to combine inline data with auto-generated domain objects.
/// </summary>
/// <example>
/// <code>
/// [Theory]
/// [CamtifyInlineAutoData("DE")]
/// [CamtifyInlineAutoData("FR")]
/// public void Test_WithInlineAndAutoData(string countryCode, Iban iban)
/// {
///     countryCode.ShouldNotBeNullOrEmpty();
///     iban.ShouldNotBeNull();
/// }
/// </code>
/// </example>
public class CamtifyInlineAutoDataAttribute : InlineAutoDataAttribute
{
    public CamtifyInlineAutoDataAttribute(params object[] values)
        : base(new CamtifyAutoDataAttribute(), values)
    {
    }
}
