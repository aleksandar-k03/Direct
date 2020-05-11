using Direct.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Bulk
{
  public class BulkModel
  {
    public int Priority { get; protected set; } = 1;
    public DirectModel Model { get; protected set; } = null;

    public BulkModel(DirectModel directModel, int priority = 1)
    {
      this.Priority = priority;
      this.Model = directModel;
    }

  }
}
