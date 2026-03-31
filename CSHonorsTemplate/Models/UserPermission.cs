using System;
using System.Collections.Generic;

namespace CSHonorsTemplate.Models;

public partial class UserPermission
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public bool IsAdmin { get; set; }
}
