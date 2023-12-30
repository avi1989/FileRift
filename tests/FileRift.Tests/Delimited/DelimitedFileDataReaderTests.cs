using FileRift.Contracts;
using FileRift.Delimited;
using NSubstitute;

namespace FileRift.Tests.Delimited;

public class DelimitedFileDataReaderTests
{
    private const string SampleCsv = """
                                     Series_reference,Period,Data_value,Suppressed,STATUS,UNITS,Magnitude,Subject,Group,Series_title_1,Series_title_2,Series_title_3,Series_title_4,Series_title_5
                                     BDCQ.SEA1AA,2011.06,80078,,F,Number,0,Business Data Collection - BDC,Industry by employment variable,Filled jobs,"Agriculture, Forestry and Fishing",Actual,,
                                     BDCQ.SEA1AA,2011.09,78324,,F,Number,0,Business Data Collection - BDC,Industry by employment variable,Filled jobs,"Agriculture, Forestry and Fishing",Actual,,
                                     """;

    private MemoryStream MemoryStream => new MemoryStream(System.Text.Encoding.UTF8.GetBytes(SampleCsv));

    [Fact]
    public void Read_ReturnsTrue_IfAnotherRowExists()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        var r = sut.Read();
        Assert.True(r);
    }

    [Fact]
    public void Read_ReturnsFalse_IfLastRowProcessed()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();
        sut.Read();
        var r = sut.Read();

        Assert.False(r);
    }

    [Fact]
    public void Constructor_SetsFieldCount_IfHeadersPresent()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "123", "123" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        Assert.Equal(2, sut.FieldCount);
    }

    [Fact]
    public void Constructor_DoesNotSetFieldCount_IfHeadersNotPresent()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "123", "123" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        Assert.Equal(-1, sut.FieldCount);
    }

    [Fact]
    public void Indexer_ReturnsValueInColumn_ByIndex()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "1", "2", "3" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);

        sut.Read();
        Assert.Equal("2", sut[1]);
    }

    [Fact]
    public void GetBoolean_ParsesResult_ToBoolean()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "1", "true", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.True(sut.GetBoolean(1));
        Assert.False(sut.GetBoolean(2));
    }

    [Fact]
    public void GetDateTime_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "1", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(new DateTime(2020, 2, 1), sut.GetDateTime(1));
    }

    [Fact]
    public void GetDateTime_UsesFormat_IfProvidedToParseResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "1", "18_09_2001", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter,
            new List<string>() { "dd_MM_yyyy" });
        sut.Read();
        Assert.Equal(new DateTime(2001, 9, 18), sut.GetDateTime(1));
    }

    [Fact]
    public void GetDecimal_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13.3", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(13.3M, sut.GetDecimal(0));
    }

    [Fact]
    public void GetFloat_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13.3", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(13.3F, sut.GetFloat(0));
    }

    [Fact]
    public void GetDouble_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13.3", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(13.3, sut.GetDouble(0));
    }

    [Fact]
    public void GetInt16_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(13, sut.GetInt16(0));
    }

    [Fact]
    public void GetInt32_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(13, sut.GetInt32(0));
    }

    [Fact]
    public void GetInt64_ParsesResult()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "2020-02-01", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(13, sut.GetInt64(0));
    }

    [Fact]
    public void GetInt16_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetInt16(1); });
    }

    [Fact]
    public void GetInt32_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetInt32(1); });
    }

    [Fact]
    public void GetInt64_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetInt64(1); });
    }

    [Fact]
    public void GetFloat_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetFloat(1); });
    }

    [Fact]
    public void GetDouble_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetDouble(1); });
    }

    [Fact]
    public void GetDecimal_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetDecimal(1); });
    }

    [Fact]
    public void GetDateTime_ThrowException_IfParseFails()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<FormatException>(() => { sut.GetDateTime(1); });
    }

    [Fact]
    public void StringIndexer_ReturnsValueByColumnName()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();
        Assert.Equal(firstName, sut["firstName"]);
        Assert.Equal(lastName, sut["lastName"]);
        Assert.Equal(age, sut["age"]);
    }

    [Fact]
    public void StringIndexer_ThrowsInvalidOperationExceptionIfNoHeaders()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Throws<InvalidOperationException>(() => sut["firstName"]);
    }

    [Fact]
    public void StringIndexer_ThrowsArgumentExceptionIfInvalidColumnRequested()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();
        Assert.Throws<ArgumentException>(() => sut["invalidColumn"]);
    }

    [Fact]
    public void GetOrdinal_ReturnsIndexByColumnName()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();
        Assert.Equal(0, sut.GetOrdinal("firstName"));
        Assert.Equal(1, sut.GetOrdinal("lastName"));
        Assert.Equal(2, sut.GetOrdinal("age"));
    }

    [Fact]
    public void GetOrdinal_ThrowsInvalidOperationException_IfNoHeaders()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<InvalidOperationException>(() => { Assert.Equal(0, sut.GetOrdinal("firstName")); });
    }

    [Fact]
    public void NextResult_Always_ReturnsFalse()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.False(sut.NextResult());
    }

    [Fact]
    public void RecordsAffected_Always_ReturnsNegative1()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(-1, sut.RecordsAffected);
    }

    [Fact]
    public void GetGuid_Returns_ParsedGuid()
    {
        var guid = Guid.NewGuid();
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", guid.ToString(), "age" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(guid, sut.GetGuid(1));
    }

    [Fact]
    public void Depth_Always_Returns1()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(1, sut.Depth);
    }

    [Fact]
    public void GetName_Returns_ColumnNameForIndex()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        Assert.Equal("firstName", sut.GetName(0));
        Assert.Equal("lastName", sut.GetName(1));
        Assert.Equal("age", sut.GetName(2));
    }

    [Fact]
    public void GetName_ThrowsInvalidOperationException_IfNoHeaders()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        Assert.Throws<InvalidOperationException>(() => sut.GetName(0));
    }

    [Fact]
    public void GetChar_Returns_CharValue()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "A", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Equal('A', sut.GetChar(1));
    }

    [Fact]
    public void GetChar_ThrowsError_IfMoreThan1Character()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "AB", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Throws<InvalidOperationException>(() => { sut.GetChar(1); });
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetChar_ReturnsNullChar_IfEmptyOrNull(string value)
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", value, "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.Equal('\0', sut.GetChar(1));
    }

    [Fact]
    public void IsDBNull_ReturnsTrue_IfNull()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", null, "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.True(sut.IsDBNull(1));
    }

    [Fact]
    public void IsDBNull_ReturnsFakse_IfNotNull()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();

        Assert.False(sut.IsDBNull(1));
    }

    [Fact]
    public void GetData_Returns_This()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);

        Assert.Equal(sut, sut.GetData(1));
    }

    [Fact]
    public void GetByte_Returns_This()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();

        byte expected = 255;
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", expected.ToString(), "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        Assert.Equal(expected, sut.GetByte(1));
    }

    [Fact]
    public void GetValues_SetsValuesInArrayPassedIn()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!).ReturnsForAnyArgs(new string[] { "13", "24", "false" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        sut.Read();
        var result = new string[3];
        sut.GetValues(result);

        Assert.Equal("13", result[0]);
        Assert.Equal("24", result[1]);
        Assert.Equal("false", result[2]);
    }

    [Fact]
    public void GetSchemaTable_ReturnsDataTableWithHeaders()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        var dataTable = sut.GetSchemaTable();
        Assert.Equal("firstName", dataTable.Columns[0].ColumnName);
        Assert.Equal("lastName", dataTable.Columns[1].ColumnName);
        Assert.Equal("age", dataTable.Columns[2].ColumnName);
    }

    [Fact]
    public void GetSchemaTable_ThrowsError_IfNoHeaders()
    {
        var firstName = "John";
        var lastName = "Doe";
        var age = "32";
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { firstName, lastName, age });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, false, rowSplitter);
        Assert.Throws<InvalidOperationException>(() => { sut.GetSchemaTable(); });
    }

    [Fact]
    public void GetChars_SetsCharactersIntoBuffer()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "A", "A" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();

        var result = new char[10];
        sut.GetChars(0, 3, result, 0, 5);
        Assert.Equal(new char[] { 'D', 'E', 'F', 'G', 'H', '\0', '\0', '\0', '\0', '\0' }, result);
    }

    [Fact]
    public void GetChars_OnlySetsCharacters_UntilEnd()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "A", "A" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();

        var result = new char[10];
        sut.GetChars(0, 24, result, 0, 5);
        Assert.Equal(new char[] { 'Y', 'Z', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' }, result);
    }

    [Fact]
    public void GetChars_OnlySetsCharacters_UntilBufferHasSpace()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "A", "A" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();

        var result = new char[2];
        sut.GetChars(0, 0, result, 0, 5);
        Assert.Equal(new char[] { 'A', 'B' }, result);
    }

    [Fact]
    public void GetChars_ReturnsActualNumberOfCharactersRead()
    {
        var rowSplitter = Substitute.For<IRowSplitter>();
        rowSplitter.SplitRow(default!)
            .ReturnsForAnyArgs(new string[] { "firstName", "lastName", "age" },
                new string[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "A", "A" });
        using var sut = new DelimitedFileDataReader(this.MemoryStream, true, rowSplitter);
        sut.Read();

        var result = new char[10];
        var readCount = sut.GetChars(0, 24, result, 0, 5);
        Assert.Equal(2, readCount);
    }

    public void Dispose()
    {
        this.MemoryStream.Dispose();
    }
}