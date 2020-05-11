using Direct.ModelsCreation;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Direct.Types.SQLite
{
	public class DirectDatabaseSqlLite : DirectDatabaseBase
	{
		public DirectDatabaseSqlLite(string databaseName, string connectionString) : base(databaseName, connectionString) { }
		public DirectDatabaseSqlLite(string databaseName, string databaseScheme, string connectionString) : base(databaseName, databaseScheme, connectionString) { }

    public override string CurrentDateQueryString => "CURRENT_TIMESTAMP";
    public override string QueryScopeID => "LAST_INSERT_ID()";
    public override string SelectTopOne => "SELECT * FROM {0} LIMIT 1";
    public override DirectDatabaseType DatabaseType => DirectDatabaseType.SQLite;
    //public override DirectModelGeneratorBase ModelsCreator => new MysqlModelsGenerator(this);
    public override string ConstructVariable(string name) => string.Format("SET @{0} :=", name);
    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e) { }

    protected override string OnBeforeCommandOverride(string command) => command;
    public override string ConstructDateTimeParam(DateTime dt) => string.Format("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
    public override DbConnection GetConnection() => new SqliteConnection(this.ConnectionString);


    internal override string QueryContructLoadByID => "SELECT {0} FROM {1} WHERE {2}={3};";
    internal override string QueryLoadSingle => "SELECT {0} FROM {1} {2} LIMIT 1";
    internal override string QueryContructLoadByStringID => "SELECT {0} FROM {1} WHERE {2}='{3}';";
    internal override string QueryContructLoad => "SELECT {0} FROM {1} {2} {3}";

    internal override string QueryConstructInsertQuery => "INSERT INTO {0} ({1}) VALUES ({2});";
    internal override string QueryConstructUpdateQuery => "UPDATE {0} SET {1} WHERE {2}={3};";
    internal override string QueryConstructUpdateUpdatedQuery => "UPDATE {0} SET {1}={2} WHERE {2}={3};";
    internal override string QueryDelete => "DELETE FROM {0} WHERE {1}={2};";

    public override DMGenerator Generator => null;
  }
}
