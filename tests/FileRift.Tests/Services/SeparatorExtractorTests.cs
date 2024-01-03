﻿using FileRift.Services;

namespace FileRift.Tests.Services;

public class SeparatorExtractorTests
{
    [Fact]
    public void GetSeparator_ShouldDetectCsv_Correctly()
    {
        string[] rows =
        [
            "A,B,C,D,E",
            "ABC,DEF,GHI,JKL,MNO",
            "Apple, Banana, Cat, Dog, Elephant",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows);
        Assert.Equal(',', result);
    }

    [Fact]
    public void GetSeparator_ShouldDetectPsv_Correctly()
    {
        string[] rows =
        [
            "A|B|C|D|E",
            "ABC|DEF|GHI|JKL|MNO",
            "Apple| Banana| Cat| Dog| Elephant",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows);
        Assert.Equal('|', result);
    }

    [Fact]
    public void GetSeparator_ShouldDetectTsv_Correctly()
    {
        string[] rows =
        [
            "A\tB\tC\tD\tE",
            "ABC\tDEF\tGHI\tJKL\tMNO",
            "Apple\t Banana\t Cat\t Dog\t Elephant",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows);
        Assert.Equal('\t', result);
    }

    [Fact]
    public void GetSeparator_ShouldDetectSpaceDelimitedFile_Correctly()
    {
        string[] rows =
        [
            "A B C D E",
            "ABC DEF GHI JKL MNO",
            "Apple Banana Cat Dog Elephant",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows);
        Assert.Equal(' ', result);
    }

    [Theory]
    [InlineData('^')]
    [InlineData('!')]
    [InlineData('#')]
    public void GetSeparator_ShouldDetectOtherSeparators_Correctly(char separator)
    {
        string[] rows =
        [
            $"A{separator}B{separator}C{separator}D{separator}E",
            $"ABC{separator}DEF{separator}GHI{separator}JKL{separator}MNO",
            $"Apple{separator} Banana{separator} Cat{separator} Dog{separator} Elephant",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows);
        Assert.Equal(separator, result);
    }

    [Fact]
    public void GetSeparator_Should_ReturnNull_IfMultipleSeparatorsAreDetected()
    {
        string[] rows =
        [
            "A,|B,|C,|D,|E",
            "ABC,|DEF,|GHI,|JKL,|MNO",
            "Apple,| Banana,| Cat,| Dog,| Elephant",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows);
        Assert.Null(result);
    }

    [Theory]
    [InlineData('^')]
    [InlineData('!')]
    [InlineData('#')]
    public void GetSeparator_ShouldManageEscapeCharacters_CorrectlyForAllSeparators(char separator)
    {
        string[] rows =
        [
            $"\"A{separator}B{separator}\"C{separator}D{separator}E",
            $"ABC{separator}GHI{separator}JKL",
            $"Apple{separator} Cat{separator} Dog",
        ];

        var sut = new SeparatorExtractor();
        var result = sut.GetSeparator(rows, '\"');
        Assert.Equal(separator, result);
    }
}