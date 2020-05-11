using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Web
{
  internal sealed class DirectWebControllerResponse
  {
    public bool Status { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public Exception Exception { get; set; } = null;
  }

  internal sealed class DirectWebControllerResponseUpdate
  {
    public bool Status { get; set; } = true;
    public int? AffectedRows { get; set; } = null;
  }

  internal sealed class DirectWebControllerResponseDelete
  {
    public bool IsDeleted { get; set; } = true;
  }

  internal sealed class DirectWebControllerResponseInsert
  {
    public bool Status { get; set; } = true;
  }

}
