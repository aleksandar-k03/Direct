using Direct.Helpers;
using Direct.Models;
using Direct.ModelsCreation;
using Direct.Results;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Direct
{
  public enum DirectDatabaseExceptionType { OnEnumerable, OnLoad, OnLoadAsync, OnExecute, OnExecuteAsync }
  public enum DirectDatabaseType { MySQL, SQLite, SQLServer }
  public abstract partial class DirectDatabaseBase : IDisposable
  {
    public string DatabaseName { get; protected set; } = string.Empty;
    public string DatabaseScheme { get; protected set; } = string.Empty;
    public string DatabaseSchemeString { get => string.IsNullOrEmpty(this.DatabaseScheme) ? "" : this.DatabaseScheme + "."; }
    public DirectDatabaseImplementation Loader { get; protected set; } = null;
    public DirectTransactionalManager TransactionalManager { get; protected set; } = null;
    public abstract DMGenerator Generator { get; }

    protected string ConnectionString { get; private set; } = string.Empty;

    ///
    /// ABSTRACTION
    ///

    public abstract DirectDatabaseType DatabaseType { get; }
    public abstract string CurrentDateQueryString { get; }
    public abstract string QueryScopeID { get; }
    public abstract string SelectTopOne { get; }

    public abstract void OnException(DirectDatabaseExceptionType type, string query, Exception e);
    public abstract string ConstructVariable(string name);
    public abstract DbConnection GetConnection();
    public abstract string ConstructDateTimeParam(DateTime dt);
    protected abstract string OnBeforeCommandOverride(string command);


    ///
    /// QUERY CONSTRUCTION
    ///

    internal virtual string QueryContructLoadByID { get => "SELECT {0} FROM [].{1} WHERE {2}={3};"; }
    internal virtual string QueryLoadSingle { get => "SELECT {0} FROM [].{1} {2} LIMIT 1;"; }
    internal virtual string QueryCount { get => "SELECT COUNT(*) FROM [].{0} {1};"; }
    internal virtual string QueryContructLoadByStringID { get => "SELECT {0} FROM [].{1} WHERE {2}='{3}';"; }
    internal virtual string QueryContructLoad { get => "SELECT {0} FROM [].{1} {2} {3}"; }
    internal virtual string QueryContructLoadWithLimit { get => "SELECT {0} FROM [].{1} {2} {3} {limit}"; }
    internal virtual string QueryLimit { get => "LIMIT "; }

    internal virtual string QueryContructLoadWithLimitConstruct<T>(DirectQueryLoader<T> loader) where T : DirectModel
    {
      if (loader.Limit.HasValue == false)
        return this.QueryContructLoadWithLimit.Replace("{limit}", string.Empty);
      return this.QueryContructLoadWithLimit.Replace("{limit}", string.Format("{0} {1}", this.QueryLimit, loader.Limit.Value));
    }

    internal virtual string QueryConstructInsertQuery { get => "INSERT INTO [].{0} ({1}) VALUES ({2});"; }
    internal virtual string QueryConstructUpdateQuery { get => "UPDATE [].{0} SET {1} WHERE {2}={3};"; }
    internal virtual string QueryConstructUpdateUpdatedQuery { get => "UPDATE [].{0} SET {1}={2} WHERE {3}={4};"; }
    internal virtual string QueryDelete { get => "DELETE FROM [].{0} WHERE {1}={2};"; }

    ///
    /// CONSTRUCTORS
    ///

    public DirectDatabaseBase(string databaseName, string connectionString)
      : this(databaseName, string.Empty, connectionString) { }
    public DirectDatabaseBase(string databaseName, string databaseScheme, string connectionString)
    {
      this.DatabaseName = databaseName;
      this.DatabaseScheme = databaseScheme;
      this.ConnectionString = connectionString;
      this.Loader = new DirectDatabaseImplementation(this);
      this.TransactionalManager = new DirectTransactionalManager(this);

    }

    ~DirectDatabaseBase() => OnDispose();
    public void Dispose() => OnDispose();
    protected void OnDispose()
    {
      // here we will close DirectConnection
      this.TransactionalManager.RunAsync();
    }

    ///
    /// Methods
    ///
    internal string PrepareQuery(string query) => this.OnBeforeCommandOverride(this.ConstructDatabaseNameAndScheme(query));

    ///
    /// LOAD METHODS (dapper)
    ///
    public IEnumerable<DirectContainerRow> LoadEnumerable(string command, params object[] parameters) => this.Loader.LoadEnumerable(command, parameters);
    public IEnumerable<DirectContainerRow> LoadEnumerable(string command) => this.Loader.LoadEnumerable(command);
    public IEnumerable<DirectContainerRow> LoadEnumerable(DbConnection connection, string command, params object[] parameters) => this.Loader.LoadEnumerable(connection, command, parameters);
    public IEnumerable<DirectContainerRow> LoadEnumerable(DbConnection connection, string command) => this.Loader.LoadEnumerable(connection, command);

    public IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(string command, params object[] parameters) => this.Loader.LoadEnumerableAsync(command, parameters);
    public IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(string command) => this.Loader.LoadEnumerableAsync(command);
    public IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(DbConnection connection, string command, params object[] parameters) => this.Loader.LoadEnumerableAsync(connection, command, parameters);
    public IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(DbConnection connection, string command) => this.Loader.LoadEnumerableAsync(connection, command);

    public DirectLoadResult Load(string query, params object[] parameters) => this.Loader.Load(query, parameters);
    public DirectLoadResult Load(string command) => this.Loader.Load(command);
    public DirectLoadResult Load(DbConnection connection, string query, params object[] parameters) => this.Loader.Load(connection, query, parameters);
    public DirectLoadResult Load(DbConnection connection, string command) => this.Loader.Load(connection, command);

    public Task<DirectLoadResult> LoadAsync(string query, params object[] parameters) => this.Loader.LoadAsync(query, parameters);
    public Task<DirectLoadResult> LoadAsync(string command) => this.Loader.LoadAsync(command);
    public Task<DirectLoadResult> LoadAsync(DbConnection connection, string query, params object[] parameters) => this.Loader.LoadAsync(connection, query, parameters);
    public Task<DirectLoadResult> LoadAsync(DbConnection connection, string command) => this.Loader.LoadAsync(connection, command);

    public DirectExecuteResult Execute(string query, params object[] parameters) => this.Loader.Execute(query, parameters);
    public DirectExecuteResult Execute(string command) => this.Loader.Execute(command);
    public DirectExecuteResult Execute(DbConnection connection, string query, params object[] parameters) => this.Loader.Execute(connection, query, parameters);
    public DirectExecuteResult Execute(DbConnection connection, string inputCommand) => this.Loader.Execute(connection, inputCommand);

    public Task<DirectExecuteResult> ExecuteAsync(string query, params object[] parameters) => this.Loader.ExecuteAsync(query, parameters);
    public Task<DirectExecuteResult> ExecuteAsync(string command) => this.Loader.ExecuteAsync(command);
    public Task<DirectExecuteResult> ExecuteAsync(DbConnection connection, string query, params object[] parameters) => this.Loader.ExecuteAsync(connection, query, parameters);
    public Task<DirectExecuteResult> ExecuteAsync(DbConnection connection, string command) => this.Loader.ExecuteAsync(connection, command);


    ///
    /// Dapper loads
    ///

    public IEnumerable<T> Load<T>(string command, params object[] parameters) => this.Loader.Load<T>(command, parameters);
    public IEnumerable<T> Load<T>(string command) => this.Loader.Load<T>(command);
    public IEnumerable<T> Load<T>(DbConnection connection, string command, params object[] parameters) => this.Loader.Load<T>(connection, command, parameters);
    public IEnumerable<T> Load<T>(DbConnection connection, string command) => this.Loader.Load<T>(connection, command);

    public Task<IEnumerable<T>> LoadAsync<T>(string command, params object[] parameters) => this.Loader.LoadAsync<T>(command, parameters);
    public Task<IEnumerable<T>> LoadAsync<T>(string command) => this.Loader.LoadAsync<T>(command);
    public Task<IEnumerable<T>> LoadAsync<T>(DbConnection connection, string command, params object[] parameters) => this.Loader.LoadAsync<T>(connection, command, parameters);
    public Task<IEnumerable<T>> LoadAsync<T>(DbConnection connection, string command) => this.Loader.LoadAsync<T>(connection, command);

    public virtual T LoadSingle<T>(string command, params object[] parameters) => this.Loader.LoadSingle<T>(command, parameters);
    public T LoadSingle<T>(string command) => this.Loader.LoadSingle<T>(command);
    public virtual T LoadSingle<T>(DbConnection connection, string command, params object[] parameters) => this.Loader.LoadSingle<T>(connection, command, parameters);
    public T LoadSingle<T>(DbConnection connection, string command) => this.Loader.LoadSingle<T>(connection, command);

    public virtual Task<T> LoadSingleAsync<T>(string command, params object[] parameters) => this.Loader.LoadSingleAsync<T>(command, parameters);
    public Task<T> LoadSingleAsync<T>(string command) => this.Loader.LoadSingleAsync<T>(command);
    public virtual Task<T> LoadSingleAsync<T>(DbConnection connection, string command, params object[] parameters) => this.Loader.LoadSingleAsync<T>(connection, command, parameters);
    public Task<T> LoadSingleAsync<T>(DbConnection connection, string command) => this.Loader.LoadSingleAsync<T>(connection, command);


    ///
    /// HELPERS
    ///

    public virtual string ConstructDatabaseNameAndScheme(string query)
    {
      query = query.Trim()
        .Replace("[].", string.Format("{0}.{1}", this.DatabaseName, this.DatabaseSchemeString));
        //.Replace("'[today]'", this.ConstructDateTimeParam(DateTime.Today))
        //.Replace("'[tomorrow]'", this.ConstructDateTimeParam((DateTime.Today.AddDays(1))))
        //.Replace("'[bweek]'", this.ConstructDateTimeParam((DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday))))
        //.Replace("'[eweek]'", this.ConstructDateTimeParam(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek)))
        //.Replace("'[bmonth]'", this.ConstructDateTimeParam(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)))
        //.Replace("'[emonth]'", this.ConstructDateTimeParam(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))));
      return query;
    }

    // Construct query with multiple parameters
    public virtual string Construct(string query, params object[] parameters)
    {
      if (query.ToLower().Trim().StartsWith("insert into") && !query.ToLower().Contains("values"))
      {
        string[] split = query.Split('(');
        if (split.Length == 2)
        {
          split = split[1].Split(',');
          query += " VALUES ( ";
          for (int i = 0; i < split.Length; i++) query += "{" + i + "}" + (i != split.Length - 1 ? "," : "");
          query += " ); ";
        }
      }

      for (int i = 0; i < parameters.Length; i++)
      {
        string pattern = "{" + i + "}";
        string value = this.GetObjectQueryValue(parameters[i]);
        query = query.Replace(pattern, value);
      }
      return this.ConstructDatabaseNameAndScheme(query);
    }

  }
}
