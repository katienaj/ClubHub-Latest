using System;
using System.Collections.Generic;

namespace CSHonorsTemplate.Models;

public partial class Person
{
    public int PersonId { get; set; }
    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? PreferredName { get; set; }
    public string FullName => $"{PreferredName ?? FirstName} {LastName}";

    public string? Unit { get; set; }

    public string? UnitAbbreviation { get; set; }

    public string PersonType { get; set; } = null!;

    public ICollection<Join> Joins { get; set; } = new List<Join>();
    
}
