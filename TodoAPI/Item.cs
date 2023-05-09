using System;
using System.Collections.Generic;

namespace TodoAPI;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsComplete { get; set; }
}
