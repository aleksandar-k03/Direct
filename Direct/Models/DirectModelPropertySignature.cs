using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Direct.Models
{
  [Serializable()]
  internal class DirectModelPropertySignature
  {
    public PropertyInfo PropertyInfo { get; protected set; } = null;
    public string AttributeName { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;

    public string Name { get => string.IsNullOrEmpty(this.AttributeName) ? this.PropertyName : this.AttributeName; }
    public bool UpdateDateTime = false;
    public bool IsPrimary { get; set; } = false;
    public bool Nullable = false;


    public bool NotUpdatable = false;
    public bool HasDefaultValue = false;

    public DirectModelPropertySignature(PropertyInfo info)
    {
      this.PropertyInfo = info;
      this.PropertyName = info.Name;

      Object[] attributes = info.GetCustomAttributes(typeof(DColumn), true);
      if (attributes.Length > 0)
      {
        DColumn attribute = (DColumn)attributes[0];
        this.IsPrimary = attribute.IsPrimary;
        this.AttributeName = attribute.Name;
        this.UpdateDateTime = attribute.DateTimeUpdate;
        this.Nullable = attribute.Nullable;
        this.NotUpdatable = attribute.NotUpdatable;
        this.HasDefaultValue = attribute.HasDefaultValue;
      }
    }
  }
}
