using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Results
{
  public class DirectExecuteResult
  {
    public long? LastID { get; set; } = null;
    public int? NumberOfRowsAffected { get; set; } = null;
    public Exception Exception { get; set; } = null;

    public bool IsSuccessfull { get => Exception == null; }


  }
}
