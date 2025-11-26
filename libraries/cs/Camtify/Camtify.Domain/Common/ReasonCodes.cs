namespace Camtify.Domain.Common;

/// <summary>
/// Common ISO 20022 reason codes for payment status and rejections.
/// </summary>
/// <remarks>
/// Provides static constants for frequently used reason codes across different message types.
/// Codes are grouped by category: Account (AC), Amount (AM), Bank Entity (BE), Date/Time (DT),
/// Financial Flow (FF), Mandate (MD), Regulatory Reporting (RR), Timeout (TM), Technical (TS),
/// and Special codes (NARR, FOCR, FRAD).
/// </remarks>
public static class ReasonCodes
{
    // Account related codes (AC01-AC14)
    
    /// <summary>
    /// IncorrectAccountNumber - Account number is invalid or missing.
    /// </summary>
    public const string AC01 = "AC01";

    /// <summary>
    /// InvalidDebtorAccountNumber - Debtor account number invalid or missing.
    /// </summary>
    public const string AC04 = "AC04";

    /// <summary>
    /// ClosedAccountNumber - Account is closed.
    /// </summary>
    public const string AC06 = "AC06";

    /// <summary>
    /// BlockedAccount - Account is blocked.
    /// </summary>
    public const string AC13 = "AC13";

    /// <summary>
    /// InvalidCreditorAccountNumber - Creditor account number invalid.
    /// </summary>
    public const string AC14 = "AC14";

    // Amount related codes (AM01-AM10)
    
    /// <summary>
    /// ZeroAmount - Transaction amount is zero.
    /// </summary>
    public const string AM01 = "AM01";

    /// <summary>
    /// NotAllowedAmount - Amount is not allowed.
    /// </summary>
    public const string AM02 = "AM02";

    /// <summary>
    /// NotSufficientFunds - Insufficient funds in debtor account.
    /// </summary>
    public const string AM04 = "AM04";

    /// <summary>
    /// Duplication - Transaction is a duplicate.
    /// </summary>
    public const string AM05 = "AM05";

    /// <summary>
    /// TooLowAmount - Amount is below agreed minimum.
    /// </summary>
    public const string AM09 = "AM09";

    /// <summary>
    /// InvalidControlSum - Sum of amounts does not match control total.
    /// </summary>
    public const string AM10 = "AM10";

    // Bank Entity codes (BE01-BE07)
    
    /// <summary>
    /// InconsistentWithEndCustomer - Identifier is inconsistent with associated account.
    /// </summary>
    public const string BE01 = "BE01";

    /// <summary>
    /// UnknownEndCustomer - Creditor or ultimate creditor is unknown.
    /// </summary>
    public const string BE04 = "BE04";

    /// <summary>
    /// MissingCreditorAddress - Creditor address is missing or incorrect.
    /// </summary>
    public const string BE05 = "BE05";

    /// <summary>
    /// UnknownCreditor - Creditor is unknown.
    /// </summary>
    public const string BE06 = "BE06";

    /// <summary>
    /// MissingDebtorAddress - Debtor address is missing or incorrect.
    /// </summary>
    public const string BE07 = "BE07";

    // Date/Time codes (DT01-DT05)
    
    /// <summary>
    /// InvalidDate - Invalid or missing date.
    /// </summary>
    public const string DT01 = "DT01";

    /// <summary>
    /// InvalidCutOffDate - Settlement date/time is past the cut-off time.
    /// </summary>
    public const string DT02 = "DT02";

    /// <summary>
    /// InvalidCreationDate - Invalid creation date and time.
    /// </summary>
    public const string DT03 = "DT03";

    /// <summary>
    /// FutureDateNotSupported - Future date not supported.
    /// </summary>
    public const string DT04 = "DT04";

    /// <summary>
    /// InvalidBankOperationCode - Bank operation code specified is not valid.
    /// </summary>
    public const string DT05 = "DT05";

    // Financial Flow codes (FF01-FF07)
    
    /// <summary>
    /// InvalidFileFormat - File format is incomplete or invalid.
    /// </summary>
    public const string FF01 = "FF01";

    /// <summary>
    /// SyntaxError - Syntactical error detected.
    /// </summary>
    public const string FF02 = "FF02";

    /// <summary>
    /// InvalidMessageIdentification - Message identification is invalid.
    /// </summary>
    public const string FF03 = "FF03";

    /// <summary>
    /// InvalidPaymentTypeInformation - Payment type information is invalid.
    /// </summary>
    public const string FF04 = "FF04";

    /// <summary>
    /// InvalidServiceLevel - Service level code is invalid or not supported.
    /// </summary>
    public const string FF05 = "FF05";

    /// <summary>
    /// InvalidLocalInstrument - Local instrument code is invalid or not supported.
    /// </summary>
    public const string FF06 = "FF06";

    /// <summary>
    /// InvalidCategoryPurpose - Category purpose code is invalid or not supported.
    /// </summary>
    public const string FF07 = "FF07";

    // Mandate codes (MD01-MD07)
    
    /// <summary>
    /// NoMandate - Mandate not found.
    /// </summary>
    public const string MD01 = "MD01";

    /// <summary>
    /// MissingMandatoryInformationInMandate - Mandatory information missing in mandate.
    /// </summary>
    public const string MD02 = "MD02";

    /// <summary>
    /// InvalidDataFormat - Invalid data format in mandate.
    /// </summary>
    public const string MD03 = "MD03";

    /// <summary>
    /// RefundRequestByEndCustomer - Refund request by end customer.
    /// </summary>
    public const string MD06 = "MD06";

    /// <summary>
    /// EndCustomerDeceased - End customer is deceased.
    /// </summary>
    public const string MD07 = "MD07";

    // Regulatory Reporting codes (RR01-RR04)
    
    /// <summary>
    /// MissingRegulatoryReporting - Regulatory reporting information is missing.
    /// </summary>
    public const string RR01 = "RR01";

    /// <summary>
    /// InvalidRegulatoryReporting - Regulatory reporting information is invalid.
    /// </summary>
    public const string RR02 = "RR02";

    /// <summary>
    /// MissingCreditorName - Creditor name is missing.
    /// </summary>
    public const string RR03 = "RR03";

    /// <summary>
    /// RegulatoryReason - Regulatory reason.
    /// </summary>
    public const string RR04 = "RR04";

    // Timeout codes
    
    /// <summary>
    /// Timeout - Cut-off time reached.
    /// </summary>
    public const string TM01 = "TM01";

    // Technical codes (TS01-TS02)
    
    /// <summary>
    /// TechnicalProblem - Technical problem detected.
    /// </summary>
    public const string TS01 = "TS01";

    /// <summary>
    /// SystemTemporarilyUnavailable - System is temporarily unavailable.
    /// </summary>
    public const string TS02 = "TS02";

    // Special codes
    
    /// <summary>
    /// Narrative - Free text narrative reason.
    /// </summary>
    public const string NARR = "NARR";

    /// <summary>
    /// FollowingCancellationRequest - Following cancellation request.
    /// </summary>
    public const string FOCR = "FOCR";

    /// <summary>
    /// FraudulentOrigin - Transaction of fraudulent origin.
    /// </summary>
    public const string FRAD = "FRAD";
}
