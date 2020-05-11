using System;
using System.Data;

namespace Direct.Results
{
  public class DirectLoadResult
  {
    public dynamic RawData { get; protected set; } = null;
    public DirectContainer Container { get; protected set; } = null;
    public Exception Exception { get; set; } = null;

    public bool HasException { get => Exception != null; }
    public bool HasResult { get => Exception == null && this.RawData != null; }

    public DirectLoadResult(dynamic data)
    {
      this.RawData = data;
      this.Container = new DirectContainer(this.RawData);
    }

    public DirectLoadResult(Exception e)
    {
      this.Exception = e;
    }

  }
}
