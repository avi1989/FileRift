﻿using FileRift.Delimited;

namespace FileRift.Tests.Delimited;

public class DelimitedRowSplitterTests
{
    [Fact]
    public void SplitRow_Csv_SplitsRow()
    {
        // Arrange
        var splitter = new DelimitedRowSplitter(',', null);
        var row = "part1,part2,part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2", result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void SplitRow_Csv_RowContainsEscapeCharacter_DoesNotSplitEscapedParts()
    {
        // Arrange
        var splitter = new DelimitedRowSplitter(',', '\"');
        var row = "part1,\"part2,stillPart2\",part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2,stillPart2", result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void SplitRow_Csv_RowDoesNotContainDelimiter_ReturnsWholeRow()
    {
        // Arrange
        var splitter = new DelimitedRowSplitter(',', null);
        var row = "part1 part2 part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Single(result);
        Assert.Equal(row, result[0]);
    }

    [Fact]
    public void SplitRow_Tsv_SplitsRow()
    {
        var splitter = new DelimitedRowSplitter('\t', null);
        var row = "part1\tpart2\tpart3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2", result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void SplitRow_TsvWithEscapeCharacter_SplitsRow()
    {
        var splitter = new DelimitedRowSplitter('\t', '\"');
        var row = "part1\t\"part2\tpart2.1\"\tpart3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2\tpart2.1", result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void SplitRow_Psv_SplitsRow()
    {
        // Arrange
        var splitter = new DelimitedRowSplitter('|', null);
        var row = "part1|part2|part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2", result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void SplitRow_PsvWithEscapeCharacter_SplitsRow()
    {
        // Arrange
        var splitter = new DelimitedRowSplitter('|', '\"');
        var row = "part1|part2|part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2", result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void SplitRow_PartContainsSpaces_NotTrimSpacesByDefault()
    {
        var splitter = new DelimitedRowSplitter(',', null);
        var row = "part1,part2 , part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2 ", result[1]);
        Assert.Equal(" part3", result[2]);
    }

    [Fact]
    public void SplitRow_PartContainsSpaces_TrimSpacesIfRequested()
    {
        var splitter = new DelimitedRowSplitter(',', null, true);
        var row = "part1, part2 ,part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2", result[1]);
        Assert.Equal("part3", result[2]);
    }
    
    [Fact]
    public void SplitRow_PartContainsSpaces_TrimSpacesIfRequestedEvenIfEscaped()
    {
        var splitter = new DelimitedRowSplitter(',', '\"', true);
        var row = "part1,\" part2 \",part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("part2", result[1]);
        Assert.Equal("part3", result[2]);
    }
    
    [Fact]
    public void SplitRow_ShouldNotConvertWhitespacesToNulls_ByDefault()
    {
        var splitter = new DelimitedRowSplitter(',', '\"', true);
        var row = "part1,,part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Equal("", result[1]);
        Assert.Equal("part3", result[2]);
    }
    
    [Fact]
    public void SplitRow_ShoulConvertWhitespacesToNulls_IfRequested()
    {
        var splitter = new DelimitedRowSplitter(',', '\"', true, true);
        var row = "part1,,part3";

        // Act
        var result = splitter.SplitRow(row);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("part1", result[0]);
        Assert.Null(result[1]);
        Assert.Equal("part3", result[2]);
    }

    [Fact]
    public void ShouldSplit_CrazyRow()
    {
        var splitter = new DelimitedRowSplitter(',', '\"', true, true);
        var row =
            "\"42\",\"471760866\",Ty\"Anthony\",\"J\",\"Anderson\",\"\",\"1213\",\"Canter Pl\",\"                    \",\"Roebuck\",\"29376\",\"\",\"\",\"\",\"\",\"\",\"Male\",\"Black/African\",\"20230707\",\"20011202\",\"\",\"N\",\"N\",\"N\",\"N\",\"N\",\"N\",\"\",\"119\",\"Roebuck Bethlehem\",\"\",\"031\",\"12\",\"01\",\"06\",\"\",\"04\",\"Active\",\"\",\"";
        
        var result = splitter.SplitRow(row);
        
        Assert.Equal(40, result.Length);
    }
}