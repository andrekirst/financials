namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Helper class providing access to test XML files.
/// </summary>
public static class TestData
{
    private const string TestDataDirectory = "TestData";

    /// <summary>
    /// Gets the pain.001.001.03 test XML.
    /// </summary>
    public static string GetPain001v03Xml()
    {
        return File.ReadAllText(Path.Combine(TestDataDirectory, "pain001_v03_simple.xml"));
    }

    /// <summary>
    /// Gets the pain.001.001.08 test XML.
    /// </summary>
    public static string GetPain001v08Xml()
    {
        return File.ReadAllText(Path.Combine(TestDataDirectory, "pain001_v08_simple.xml"));
    }

    /// <summary>
    /// Gets the pain.001.001.09 test XML.
    /// </summary>
    public static string GetPain001v09Xml()
    {
        return File.ReadAllText(Path.Combine(TestDataDirectory, "pain001_v09_simple.xml"));
    }

    /// <summary>
    /// Gets the pain.001.001.10 test XML.
    /// </summary>
    public static string GetPain001v10Xml()
    {
        return File.ReadAllText(Path.Combine(TestDataDirectory, "pain001_v10_simple.xml"));
    }

    /// <summary>
    /// Gets the pain.001.001.11 test XML.
    /// </summary>
    public static string GetPain001v11Xml()
    {
        return File.ReadAllText(Path.Combine(TestDataDirectory, "pain001_v11_simple.xml"));
    }

    /// <summary>
    /// Gets the pain.001.001.09 test XML with multiple payment information entries.
    /// </summary>
    /// <remarks>
    /// For testing streaming scenarios. Returns the same XML as GetPain001v09Xml()
    /// since the existing test file contains payment information entries.
    /// </remarks>
    public static string GetPain001v09XmlWithMultiplePayments()
    {
        return GetPain001v09Xml();
    }

    /// <summary>
    /// Gets test XML for a specific version.
    /// </summary>
    /// <param name="version">Version string (e.g., "03", "08", "09", "10", "11").</param>
    /// <returns>The test XML content.</returns>
    /// <exception cref="ArgumentException">Thrown when version is not supported.</exception>
    public static string GetXmlForVersion(string version)
    {
        return version switch
        {
            "03" => GetPain001v03Xml(),
            "08" => GetPain001v08Xml(),
            "09" => GetPain001v09Xml(),
            "10" => GetPain001v10Xml(),
            "11" => GetPain001v11Xml(),
            _ => throw new ArgumentException($"Unsupported version: {version}", nameof(version))
        };
    }
}
