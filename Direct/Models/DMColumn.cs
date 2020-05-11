using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Models
{
  public class DColumn : Attribute
  {
    public string Name { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public bool HasDefaultValue { get; set; } = false;
    public bool Nullable { get; set; } = false;
    public bool DateTimeUpdate { get; set; } = false;
    public bool NotUpdatable { get; set; } = false;
  }
}
