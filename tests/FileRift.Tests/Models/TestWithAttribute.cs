using FileRift.Attributes;

namespace FileRift.Tests.Models;

public class TestWithAttribute
{
    [ColumnName("F_Name")]
    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    public int Age { get; set; }
}