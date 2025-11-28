using Camtify.Core;
using Camtify.Messages.Pain001.Models.Pain001;
using Camtify.Parsing;

namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Common interface for all pain.001 parsers.
/// </summary>
/// <remarks>
/// This interface provides version-independent access to pain.001 parsers.
/// Use Pain001ParserFactory.CreateAsync() for automatic version detection.
/// Extends the generic IIso20022Parser interface for integration with the central parser factory.
/// Version and Namespace properties are inherited from IIso20022Parser.
/// </remarks>
public interface IPain001Parser : IIso20022Parser<IIso20022Document<CustomerCreditTransferInitiation>>
{
}
