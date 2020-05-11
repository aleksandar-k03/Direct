using Direct.Types.Mysql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Tests
{
  public class CCSubmitDirect : DirectDatabaseMysql
  {
    private static object LockObj = new object();
    private static CCSubmitDirect _instance = null;

    public static CCSubmitDirect Instance
    {
      get
      {
        return new CCSubmitDirect();
      }
    }

    public CCSubmitDirect()
      //: base("livesports", "Server=46.166.160.58; database=livesports; UID=livesports; password=a48i72V\"B?8>79Z", openConnection)
      : base("ccmonkeys", "Server=ccmonkeys.cerqlxjx1slg.eu-central-1.rds.amazonaws.com; database=ccmonkeys; UID=admin; password=adminpasssifra12345; Allow User Variables=True;")
    { }




    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e)
    {
      int a = 0;
    }
  }
}
