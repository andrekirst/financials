using Camtify.Core;

namespace Camtify.Tests.Common.AutoFixture.Builders;

/// <summary>
/// Specimen builder for creating valid MessageIdentifier instances.
/// Uses a pool of valid ISO 20022 message identifiers.
/// </summary>
public class MessageIdentifierSpecimenBuilder : ISpecimenBuilder
{
    private static readonly string[] ValidIdentifiers =
    [
        "pain.001.001.09",  // Customer Credit Transfer Initiation
        "pain.002.001.10",  // Payment Status Report
        "camt.052.001.08",  // Bank to Customer Account Report
        "camt.053.001.08",  // Bank to Customer Statement
        "camt.054.001.08",  // Bank to Customer Debit Credit Notification
        "pacs.008.001.08",  // FI to FI Customer Credit Transfer
        "pacs.002.001.10",  // FI to FI Payment Status Report
        "head.001.001.02",  // Business Application Header
        "acmt.007.001.03",  // Account Opening Request
        "reda.016.001.01"   // Party Query
    ];

    private int _currentIndex;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(MessageIdentifier))
        {
            return new NoSpecimen();
        }

        var identifier = ValidIdentifiers[_currentIndex % ValidIdentifiers.Length];
        _currentIndex++;
        return new MessageIdentifier(identifier);
    }
}
