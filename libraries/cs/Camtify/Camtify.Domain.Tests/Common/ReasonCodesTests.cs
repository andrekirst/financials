using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class ReasonCodesTests
{
    [Theory]
    [InlineData(ReasonCodes.AC01, "AC01")]
    [InlineData(ReasonCodes.AC04, "AC04")]
    [InlineData(ReasonCodes.AC06, "AC06")]
    [InlineData(ReasonCodes.AC13, "AC13")]
    [InlineData(ReasonCodes.AC14, "AC14")]
    public void AccountCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.AM01, "AM01")]
    [InlineData(ReasonCodes.AM02, "AM02")]
    [InlineData(ReasonCodes.AM04, "AM04")]
    [InlineData(ReasonCodes.AM05, "AM05")]
    [InlineData(ReasonCodes.AM09, "AM09")]
    [InlineData(ReasonCodes.AM10, "AM10")]
    public void AmountCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.BE01, "BE01")]
    [InlineData(ReasonCodes.BE04, "BE04")]
    [InlineData(ReasonCodes.BE05, "BE05")]
    [InlineData(ReasonCodes.BE06, "BE06")]
    [InlineData(ReasonCodes.BE07, "BE07")]
    public void BankEntityCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.DT01, "DT01")]
    [InlineData(ReasonCodes.DT02, "DT02")]
    [InlineData(ReasonCodes.DT03, "DT03")]
    [InlineData(ReasonCodes.DT04, "DT04")]
    [InlineData(ReasonCodes.DT05, "DT05")]
    public void DateTimeCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.FF01, "FF01")]
    [InlineData(ReasonCodes.FF02, "FF02")]
    [InlineData(ReasonCodes.FF03, "FF03")]
    [InlineData(ReasonCodes.FF04, "FF04")]
    [InlineData(ReasonCodes.FF05, "FF05")]
    [InlineData(ReasonCodes.FF06, "FF06")]
    [InlineData(ReasonCodes.FF07, "FF07")]
    public void FinancialFlowCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.MD01, "MD01")]
    [InlineData(ReasonCodes.MD02, "MD02")]
    [InlineData(ReasonCodes.MD03, "MD03")]
    [InlineData(ReasonCodes.MD06, "MD06")]
    [InlineData(ReasonCodes.MD07, "MD07")]
    public void MandateCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.RR01, "RR01")]
    [InlineData(ReasonCodes.RR02, "RR02")]
    [InlineData(ReasonCodes.RR03, "RR03")]
    [InlineData(ReasonCodes.RR04, "RR04")]
    public void RegulatoryReportingCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Fact]
    public void TimeoutCode_ShouldHaveCorrectValue()
    {
        // Assert
        ReasonCodes.TM01.ShouldBe("TM01");
    }

    [Theory]
    [InlineData(ReasonCodes.TS01, "TS01")]
    [InlineData(ReasonCodes.TS02, "TS02")]
    public void TechnicalCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ReasonCodes.NARR, "NARR")]
    [InlineData(ReasonCodes.FOCR, "FOCR")]
    [InlineData(ReasonCodes.FRAD, "FRAD")]
    public void SpecialCodes_ShouldHaveCorrectValues(string code, string expected)
    {
        // Assert
        code.ShouldBe(expected);
    }

    [Fact]
    public void ReasonCodes_AllCommonCodes_ShouldBePresent()
    {
        // Assert - Account codes (AC01-AC14)
        ReasonCodes.AC01.ShouldNotBeNull();
        ReasonCodes.AC04.ShouldNotBeNull();
        ReasonCodes.AC06.ShouldNotBeNull();
        ReasonCodes.AC13.ShouldNotBeNull();
        ReasonCodes.AC14.ShouldNotBeNull();

        // Amount codes (AM01-AM10)
        ReasonCodes.AM01.ShouldNotBeNull();
        ReasonCodes.AM02.ShouldNotBeNull();
        ReasonCodes.AM04.ShouldNotBeNull();
        ReasonCodes.AM05.ShouldNotBeNull();
        ReasonCodes.AM09.ShouldNotBeNull();
        ReasonCodes.AM10.ShouldNotBeNull();

        // Bank Entity codes (BE01-BE07)
        ReasonCodes.BE01.ShouldNotBeNull();
        ReasonCodes.BE04.ShouldNotBeNull();
        ReasonCodes.BE05.ShouldNotBeNull();
        ReasonCodes.BE06.ShouldNotBeNull();
        ReasonCodes.BE07.ShouldNotBeNull();

        // Date/Time codes (DT01-DT05)
        ReasonCodes.DT01.ShouldNotBeNull();
        ReasonCodes.DT02.ShouldNotBeNull();
        ReasonCodes.DT03.ShouldNotBeNull();
        ReasonCodes.DT04.ShouldNotBeNull();
        ReasonCodes.DT05.ShouldNotBeNull();

        // Financial Flow codes (FF01-FF07)
        ReasonCodes.FF01.ShouldNotBeNull();
        ReasonCodes.FF02.ShouldNotBeNull();
        ReasonCodes.FF03.ShouldNotBeNull();
        ReasonCodes.FF04.ShouldNotBeNull();
        ReasonCodes.FF05.ShouldNotBeNull();
        ReasonCodes.FF06.ShouldNotBeNull();
        ReasonCodes.FF07.ShouldNotBeNull();

        // Mandate codes (MD01-MD07)
        ReasonCodes.MD01.ShouldNotBeNull();
        ReasonCodes.MD02.ShouldNotBeNull();
        ReasonCodes.MD03.ShouldNotBeNull();
        ReasonCodes.MD06.ShouldNotBeNull();
        ReasonCodes.MD07.ShouldNotBeNull();

        // Regulatory Reporting codes (RR01-RR04)
        ReasonCodes.RR01.ShouldNotBeNull();
        ReasonCodes.RR02.ShouldNotBeNull();
        ReasonCodes.RR03.ShouldNotBeNull();
        ReasonCodes.RR04.ShouldNotBeNull();

        // Timeout codes
        ReasonCodes.TM01.ShouldNotBeNull();

        // Technical codes (TS01-TS02)
        ReasonCodes.TS01.ShouldNotBeNull();
        ReasonCodes.TS02.ShouldNotBeNull();

        // Special codes
        ReasonCodes.NARR.ShouldNotBeNull();
        ReasonCodes.FOCR.ShouldNotBeNull();
        ReasonCodes.FRAD.ShouldNotBeNull();
    }
}
