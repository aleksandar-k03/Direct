using Direct.Types.Mysql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{
  public class CCSubmitDirect : DirectDatabaseMysql
  {
    private static object LockObj = new object();
    private static CCSubmitDirect _instance = null;

    public static CCSubmitDirect Instance
    {
      get
      {
        //if (_instance != null)
        //  return _instance;
        //_instance = new CCSubmitDirect();
        //return _instance;
        return new CCSubmitDirect();
      }
    }

    public CCSubmitDirect()
      : base("ccmonkeys", "Server=ccmonkeys.cerqlxjx1slg.eu-central-1.rds.amazonaws.com; database=ccmonkeys; UID=admin; password=adminpasssifra12345; Allow User Variables=True;")
    { }


    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e)
    {
      int a = 0;
    }
  }
}
