namespace FileRift.Tests.Models;

public class Test
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public int Age { get; set; }
    
    public string Test_Underscore { get; set; }

    public InternalTest Test2 { get; set; } = new InternalTest();
}

public class InternalTest
{
    public int Age { get; set; }
}