# FileRift

## Description

FileRift is a versatile NuGet package designed for reading and soon writing tabular data from files. It supports various
delimited formats such as comma-separated, pipe-separated, and tab-separated files and also supports fixed width files.

## Installation

FileRift can be easily installed using Nuget using the following command

```
dotnet add FileRift
```

## Features

### Typed Reader

FileRift provides a simple way to read delimited files and map them into a class. Example usage:

Take a look at the Csv file below.
```
Id,FIRST_NAME,LAST_NAME,AGE,IS_STUDENT,
cd0cf662-9983-4152-8230-2a6f225ad985, John, Doe, 25, true
b3436c0e-7eb4-4620-b9eb-7890c3462fbe, Jane, "Mary Doe", 22, false
```

This can be mapped into a class with the following code
```csharp
var reader = FileRiftBuilder
    .Delimited(pathToFile)
    .AutoConfigure()
    .Build<Person>();

IEnumerable<Person> data = reader.Read();
```

That's it!
That's it! The BuildAutoConfiguredReader method does a few things.
1. It assumes that the file has a header.
2. It scans the first 20 rows to try to understand the file type.
3. It automatically generates a class map that will ignore casing, ignore underscores, spaces and hypens

## Explicit Configuration
If you do not want the class mapper to be auto configured, you can do the following.
```csharp
var classMap = new ClassMap<Person>();
classMap.AddColumnMap("FirstName", x => x.FirstName)
    .AddColumnMap("LastName", x => x.LastName)
    .AddColumnMap("Age", x => x.Age)
    .AddColumnMap("IsStudent", x => x.IsStudent)
    .AddColumnMap("Id", x => x.Id);

// Building the file reader
 var fileReader = FileReaderBuilder
    .Delimited(pathToFile)
    .HasHeaders()
    .AutoConfigure()
    .Build(classMap);

// Reading the file
IEnumerable<Person> results = fileReader.Read()
```

Note that we are not configuring the delimters or escape characters. FileRift
automatically reads the first 20 lines of the file and tries to detect the 
delimiters and separators.

### Configuring the Reader
You can also choose to configure the reader manually. You can do that as described below.
```csharp
// Setting up the class mapping
var classMap = new ClassMap<Person>();
classMap.AddColumnMap("FirstName", x => x.FirstName)
    .AddColumnMap("LastName", x => x.LastName)
    .AddColumnMap("Age", x => x.Age)
    .AddColumnMap("IsStudent", x => x.IsStudent)
    .AddColumnMap("Id", x => x.Id);

// Building the file reader
 var fileReader = FileReaderBuilder.BuildDelimitedReader(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithQuote('\"')
            .Build(classMap);

// Reading the file
IEnumerable<Person> results = fileReader.Read()

// Process results here.
            
```

### Access DataReader

FileRift also allows direct access to the underlying DataReader for more flexibility:
```csharp

using var reader = FileRiftBuilder
    .Delimited(pathToFile)
    .HasHeader()
    .AutoConfigure()
    .BuildDataReader():

while(reader.Read()) 
{
    var name = reader.GetString("First Name");
    var age = reader.GetInt32("Age");
    var id = reader.GetGuid("Id");
    var dateOfBirth = reader.GetDateTime("Date Of Birth");
    
    // Process data here
}
```

### Support Date Format per file

FileRift supports specifying valid date formats to read dates. If formats are not provided,
FileRift parses dates using the standard DateTime.Parse method.

```csharp
var classMap = new ClassMap<Person>();
classMap.AddColumnMap("FirstName", x => x.FirstName)
    .AddColumnMap("LastName", x => x.LastName)
    .AddColumnMap("Age", x => x.Age)
    .AddColumnMap("IsStudent", x => x.IsStudent)
    .AddColumnMap("Id", x => x.Id);

 var fileReader = FileRiftBuilder.BuildDelimitedReader(pathToFile)
            .HasHeaders()
            .WithDelimiter('|')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .Build(classMap);

```

The key line there is `.WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")`.

The formatting tokens are available
in [Microsoft's Documentation](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)

## Fixed Length File Reader

FileRift also supports reading FixedWidth files. If you're not sure what a fixed width file here, take a look at the
file below

```
cd0cf662-9983-4152-8230-2a6f225ad985John                Doe                 25true 
b3436c0e-7eb4-4620-b9eb-7890c3462fbeJane                Mary Doe            22false
```

The first 36 characters of this file has the "Id" field. The second 20 characters has the first name.
The next 20 characters contains the last name. The last 5 characters contains whether they are a student.

Fixed width files do not have headers embedded in the file.

FileRift also supports reading Fixed Width Files similarly.

```csharp
var pathToFile = Path.Join(_basePath, "Files", "FixedWidthFile.txt");
var classMap = new ClassMap<Person>();
classMap
    .AddColumnMap("Id", x => x.Id)
    .AddColumnMap("FirstName", x => x.FirstName)
    .AddColumnMap("LastName", x => x.LastName)
    .AddColumnMap("Age", x => x.Age)
    .AddColumnMap("IsStudent", x => x.IsStudent);

// var fileReader = new DelimitedFileReader<Person>(pathToFile, true, ',', '\"', classMap, true);
var columns = new List<FixedWidthColumnInfo>()
{
    new(1, 36, "Id"),
    new(2, 20, "FirstName"),
    new(3, 20, "LastName"),
    new(4, 2, "Age"),
    new(5, 5, "IsStudent"),
};

// var fileReader = new FixedWidthFileReader<Person>(pathToFile, columns, classMap);
var fileReader = FileRiftBuilder.FixedWidth(pathToFile)
    .WithColumns(columns)
    .Build(classMap);

var results = fileReader.Read().ToList();
```

The code above allows us to provide a configuration for the fixed width file. The file can then be mapped into a class
or read with a DataReader.

## Configuring ClassMaps

It becomes tedious to configure ClassMaps every time you need to parse a file. So FileRift supports pre-configuring
ClassMaps for reuse.

```csharp
var classMap = new ClassMap<Person>();
// ... (as before)

var classMaps = new ClassMaps();
classMaps.RegisterClassMap(classMap);
```

Now, you can read a file at any time without reconfiguring the ClassMap:

```csharp
 var pathToFile = Path.Join(_basePath, "Files", "CsvWithHeader.csv");
var fileReader = FileRiftBuilder
    .Delimited(pathToFile)
    .HasHeaders()
    .WithDelimiter(',')
    .WithEscapeCharacter('\"')
    .Build<Person>();

var results = fileReader.Read().ToList();
```

## Roadmap
- Allow mapping into types that have a constructor with parameters.
- Add Documentation for ignoring line level errors and how to access the error log