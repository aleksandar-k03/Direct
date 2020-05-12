using Direct.ModelsCreation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Direct.Types.SQLServer
{
  public class DirectDatabaseSqlServer : DirectDatabaseBase
  {
    public DirectDatabaseSqlServer(string databaseName, string defaultDatabaseScheme, string connectionString) : base(databaseName, defaultDatabaseScheme, connectionString) { }

    public override DirectDatabaseType DatabaseType => DirectDatabaseType.SQLServer;
    public override DMGenerator Generator => new DirectDatabaseSqlServerModelGenerator(this);
    public override string CurrentDateQueryString => "CURRENT_TIMESTAMP";
    public override string QueryScopeID => "SCOPE_IDENTITY()";
    public override string SelectTopOne => "SELECT TOP 1 * FROM [].{0};";
    public override string ConstructDateTimeParam(DateTime dt) => string.Format("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
    public override string ConstructVariable(string name) => string.Format("SET @{0} =", name);
    public override DbConnection GetConnection() 
      => new SqlConnection(this.ConnectionString);
    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e) { }
    protected override string OnBeforeCommandOverride(string command) => command;

    internal override string QueryLoadSingle { get => "SELECT TOP 1 {0} FROM [].{1} {2};"; }
    internal override string QueryContructLoadWithLimit { get => "SELECT {limit} {0} FROM [].{1} {2} {3}"; }
    internal override string QueryLimit { get => "TOP "; }

    public override string ConstructDatabaseNameAndScheme(string query) 
      => query.Trim().Replace("[].", string.Format("{0}.", this.DatabaseName));

  }
}
