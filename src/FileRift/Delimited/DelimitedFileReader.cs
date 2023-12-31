﻿using System.Data;
using FileRift.Contracts;
using FileRift.Mappers;

namespace FileRift.Delimited;

public class DelimitedFileReader<T> : TypedFileReader<T> where T : class, new()
{
    public DelimitedFileReader(
        string fileName,
        bool hasHeader,
        char delimiter,
        char? escapeCharacter,
        ClassMap<T> map,
        bool shouldAutoTrim = false,
        bool shouldConvertWhitespaceToNulls = false,
        bool shouldIgnoreErrors = false,
        IEnumerable<string>? allowedDateFormats = null)
        : base(new DelimitedFileDataReader(
                   fileName,
                   hasHeader,
                   delimiter,
                   escapeCharacter,
                   shouldAutoTrim,
                   shouldConvertWhitespaceToNulls,
                   allowedDateFormats),
               map,
               shouldIgnoreErrors)
    {
    }

    internal DelimitedFileReader(
        IFileRiftDataReader reader,
        ClassMap<T> map,
        bool shouldIgnoreErrors = false) : base(reader, map, shouldIgnoreErrors)
    {
    }
}