using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Exception thrown when no parser is found for a message type.
/// </summary>
public class ParserNotFoundException : Iso20022Exception
{
    /// <summary>
    /// Gets the message identifier for which no parser was found.
    /// </summary>
    public MessageIdentifier MessageId { get; }

    /// <summary>
    /// Gets the list of available parsers.
    /// </summary>
    public IReadOnlyCollection<MessageIdentifier> AvailableParsers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserNotFoundException"/> class.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="availableParsers">The list of available parsers.</param>
    public ParserNotFoundException(
        MessageIdentifier messageId,
        IReadOnlyCollection<MessageIdentifier> availableParsers)
        : base($"No parser found for message type '{messageId}'. " +
               $"Available parsers: {string.Join(", ", availableParsers)}")
    {
        MessageId = messageId;
        AvailableParsers = availableParsers;
    }
}
