using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Exception thrown when a parser is already registered.
/// </summary>
public class ParserAlreadyRegisteredException : Iso20022Exception
{
    /// <summary>
    /// Gets the message identifier for which a parser is already registered.
    /// </summary>
    public MessageIdentifier MessageId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserAlreadyRegisteredException"/> class.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    public ParserAlreadyRegisteredException(MessageIdentifier messageId)
        : base($"Parser for '{messageId}' is already registered.")
    {
        MessageId = messageId;
    }
}
