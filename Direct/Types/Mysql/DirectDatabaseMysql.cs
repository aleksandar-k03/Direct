using Direct.ModelsCreation;
using Direct.Results;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Types.Mysql
{
  public class DirectDatabaseMysql : DirectDatabaseBase
  {
    public DirectDatabaseMysql(string databaseName, string connectionString) : base(databaseName, connectionString)
    { }

    public override string CurrentDateQueryString => "CURRENT_TIMESTAMP";
    public override string QueryScopeID => "LAST_INSERT_ID()";
    public override string SelectTopOne => "SELECT * FROM [].{0} LIMIT 1";
    public override DirectDatabaseType DatabaseType => DirectDatabaseType.MySQL;

    public override DMGenerator Generator => new DirectDatabaseMysqlModelGenerator(this);

    //public override DirectModelGeneratorBase ModelsCreator => new MysqlModelsGenerator(this);
    public override string ConstructVariable(string name) => string.Format("SET @{0} :=", name);
    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e) { }

    protected override string OnBeforeCommandOverride(string command) => command;
    public override string ConstructDateTimeParam(DateTime dt) => string.Format("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
    public override DbConnection GetConnection() => new MySqlConnection(this.ConnectionString);

  }
}
