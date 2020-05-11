using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.ModelsCreation
{
  public class DMGInitialSqlResponse
  {
    public string schema { get; set; }
    public string table_name { get; set; }
    public string column_name { get; set; }
    public string column_position { get; set; }
    public string column_default { get; set; }
    public string type { get; set; }
    public string length { get; set; }
    public string is_nullable { get; set; }
  }
}
