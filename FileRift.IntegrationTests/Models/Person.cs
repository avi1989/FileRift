﻿namespace FileRift.IntegrationTests.Models;

public class Person
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public bool IsStudent { get; set; }
    
    public string Nulls { get; set; }
}