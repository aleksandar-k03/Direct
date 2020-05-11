using Dapper;
using Direct.Results;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direct
{
  public class DirectDatabaseImplementation
  {
    private DirectDatabaseBase Database = null;

    public DirectDatabaseImplementation(DirectDatabaseBase database)
    {
      this.Database = database;
    }

    ///
    /// Loads dynamic
    ///

    public virtual IEnumerable<T> Load<T>(string command, params object[] parameters) => this.Load<T>(this.Database.Construct(command, parameters));
    public IEnumerable<T> Load<T>(string command) => this.Load<T>(null, command);
    public virtual IEnumerable<T> Load<T>(DbConnection connection, string command, params object[] parameters) => this.Load<T>(connection, this.Database.Construct(command, parameters));
    public IEnumerable<T> Load<T>(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();
      try
      {
        command = this.Database.PrepareQuery(command);
        return connection.Query<T>(command);
      }
      catch(Exception e)
      {
        this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        return null;
      }
      finally
      {
        if (!hadConnection)
          connection.CloseAsync();
      }
    }
    
    ///
    /// Load dynamic async
    ///

    public virtual Task<IEnumerable<T>> LoadAsync<T>(string command, params object[] parameters) => this.LoadAsync<T>(this.Database.Construct(command, parameters));
    public Task<IEnumerable<T>> LoadAsync<T>(string command) => this.LoadAsync<T>(null, command);
    public virtual Task<IEnumerable<T>> LoadAsync<T>(DbConnection connection, string command, params object[] parameters) => this.LoadAsync<T>(connection, this.Database.Construct(command, parameters));
    public async Task<IEnumerable<T>> LoadAsync<T>(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();
      try
      {
        command = this.Database.PrepareQuery(command);
        return await connection.QueryAsync<T>(command);
      }
      catch(Exception e)
      {
        this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        return null;
      }
      finally
      {
        if (!hadConnection)
          await connection.CloseAsync();
      }
    }

    ///
    /// Load singles
    ///

    public virtual T LoadSingle<T>(string command, params object[] parameters) => this.LoadSingle<T>(this.Database.Construct(command, parameters));
    public T LoadSingle<T>(string command) => LoadSingle<T>(null, command);
    public virtual T LoadSingle<T>(DbConnection connection, string command, params object[] parameters) => this.LoadSingle<T>(connection, this.Database.Construct(command, parameters));
    public T LoadSingle<T>(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();
      try
      {
        command = this.Database.PrepareQuery(command);
        return connection.QuerySingle<T>(command);
      }
      catch(Exception e)
      {
        this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        return default(T);
      }
      finally
      {
        if (!hadConnection)
          connection.CloseAsync();
      }
    }

    ///
    /// Load singles async
    ///

    public virtual Task<T> LoadSingleAsync<T>(string command, params object[] parameters) => this.LoadSingleAsync<T>(this.Database.Construct(command, parameters));
    public Task<T> LoadSingleAsync<T>(string command) => LoadSingleAsync<T>(null, command);
    public virtual Task<T> LoadSingleAsync<T>(DbConnection connection, string command, params object[] parameters) => this.LoadSingleAsync<T>(connection, this.Database.Construct(command, parameters));
    public async Task<T> LoadSingleAsync<T>(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      try
      {
        command = this.Database.PrepareQuery(command);
        return await connection.QuerySingleAsync<T>(command);
      }
      catch(Exception e)
      {
        //this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        return default(T);
      }
      finally
      {
        if (!hadConnection)
          await connection.CloseAsync();
      }
    }

    ///
    /// LoadEnumerable (DirectContainerRow)
    ///

    public virtual IEnumerable<DirectContainerRow> LoadEnumerable(string command, params object[] parameters) => this.LoadEnumerable(this.Database.Construct(command, parameters));
    public IEnumerable<DirectContainerRow> LoadEnumerable(string command) => LoadEnumerable(null, command);
    public virtual IEnumerable<DirectContainerRow> LoadEnumerable(DbConnection connection, string command, params object[] parameters) => this.LoadEnumerable(connection, this.Database.Construct(command, parameters));
    public IEnumerable<DirectContainerRow> LoadEnumerable(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      foreach (dynamic row in connection.Query(command))
        yield return new DirectContainerRow(row);

      if (!hadConnection)
        connection.CloseAsync();

      yield break;
    }


    ///
    /// LoadEnumerable (DirectContainerRow) ASYNC
    ///

    public virtual IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(string command, params object[] parameters) =>  this.LoadEnumerableAsync(this.Database.Construct(command, parameters));
    public IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(string command) => LoadEnumerableAsync(null, command);
    public virtual IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(DbConnection connection, string command, params object[] parameters) => this.LoadEnumerableAsync(connection, this.Database.Construct(command, parameters));
    public async IAsyncEnumerable<DirectContainerRow> LoadEnumerableAsync(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      foreach (dynamic row in await connection.QueryAsync(command))
        yield return new DirectContainerRow(row);

      if (!hadConnection)
        await connection.CloseAsync();

      yield break;
    }

    ///
    /// LoadEnumerable (T)
    ///

    public virtual IEnumerable<T> LoadEnumerable<T>(string command, params object[] parameters) => this.LoadEnumerable<T>(this.Database.Construct(command, parameters));
    public IEnumerable<T> LoadEnumerable<T>(string command) => LoadEnumerable<T>(null, command);
    public virtual IEnumerable<T> LoadEnumerable<T>(DbConnection connection, string command, params object[] parameters) => this.LoadEnumerable<T>(connection, this.Database.Construct(command, parameters));
    public IEnumerable<T> LoadEnumerable<T>(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      foreach (var row in connection.Query<T>(command))
      {
        //if(row.GetType() == typeof(Direct.Models.DirectModel))
        //{
        //  (row as Direct.Models.DirectModel).Database = Database;
        //}

        yield return row;
      }

      if (!hadConnection)
        connection.CloseAsync();

      yield break;
    }

    ///
    /// LoadEnumerable (T) ASYNC
    ///

    public virtual IAsyncEnumerable<T> LoadEnumerableAsync<T>(string command, params object[] parameters) => this.LoadEnumerableAsync<T>(this.Database.Construct(command, parameters));
    public IAsyncEnumerable<T> LoadEnumerableAsync<T>(string command) => LoadEnumerableAsync<T>(null, command);
    public virtual IAsyncEnumerable<T> LoadEnumerableAsync<T>(DbConnection connection, string command, params object[] parameters) => this.LoadEnumerableAsync<T>(connection, this.Database.Construct(command, parameters));
    public async IAsyncEnumerable<T> LoadEnumerableAsync<T>(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      foreach (var row in await connection.QueryAsync<T>(command))
        yield return row;

      if (!hadConnection)
        await connection.CloseAsync();

      yield break;
    }

    ///
    /// LOAD sync
    ///

    public virtual DirectLoadResult Load(string query, params object[] parameters) => this.Load(this.Database.Construct(query, parameters));
    public DirectLoadResult Load(string command) => Load(null, command);
    public virtual DirectLoadResult Load(DbConnection connection, string query, params object[] parameters) => this.Load(connection, this.Database.Construct(query, parameters));
    public DirectLoadResult Load(DbConnection connection, string command)
    {

      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      try
      {
        return new DirectLoadResult(connection.Query(command).AsList());
      }
      catch(Exception e)
      {
        //this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        //return new DirectLoadResult(e);
        this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        throw e;
      }
      finally
      {
        if (!hadConnection)
          connection.CloseAsync();
      }
    }

    ///
    /// LOAD async
    ///

    public virtual Task<DirectLoadResult> LoadAsync(string query, params object[] parameters) => this.LoadAsync(this.Database.Construct(query, parameters));
    public Task<DirectLoadResult> LoadAsync(string command) => LoadAsync(null, command);
    public virtual Task<DirectLoadResult> LoadAsync(DbConnection connection, string query, params object[] parameters) => this.LoadAsync(connection, this.Database.Construct(query, parameters));
    public async Task<DirectLoadResult> LoadAsync(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      try
      {
        return new DirectLoadResult((await connection.QueryAsync(command)).AsList());
      }
      catch (Exception e)
      {
        //this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        //return new DirectLoadResult(e);
        this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        throw e;
      }
      finally
      {
        if (!hadConnection)
          await connection.CloseAsync();
      }
    }

    ///
    /// Execute sync
    ///

    public virtual DirectExecuteResult Execute(string query, params object[] parameters) => this.Execute(this.Database.Construct(query, parameters));
    public DirectExecuteResult Execute(string command) => Execute(null, command);
    public virtual DirectExecuteResult Execute(DbConnection connection, string query, params object[] parameters) => this.Execute(connection, this.Database.Construct(query, parameters));
    public DirectExecuteResult Execute(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      try
      {
        if (string.IsNullOrEmpty(command))
          throw new Exception("empty query");

        if (command.ToLower().StartsWith("insert into"))
          return new DirectExecuteResult()
          {
            NumberOfRowsAffected = null,
            LastID = connection.QuerySingle<int>(command + string.Format("SELECT {0};", this.Database.QueryScopeID))
          };
        else
          return new DirectExecuteResult()
          {
            NumberOfRowsAffected = connection.Execute(command)
          };
      }
      catch (Exception e)
      {
        //this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        //return new DirectExecuteResult() { Exception = e };
        this.Database.OnException(DirectDatabaseExceptionType.OnExecute, command, e);
        throw e;
      }
      finally
      {
        if (!hadConnection)
          connection.CloseAsync();
      }
    }

    ///
    /// Execute async
    ///

    public virtual Task<DirectExecuteResult> ExecuteAsync(string query, params object[] parameters) => this.ExecuteAsync(this.Database.Construct(query, parameters));
    public Task<DirectExecuteResult> ExecuteAsync(string command) => ExecuteAsync(null, command);
    public virtual Task<DirectExecuteResult> ExecuteAsync(DbConnection connection, string query, params object[] parameters) => this.ExecuteAsync(connection, this.Database.Construct(query, parameters));
    public async Task<DirectExecuteResult> ExecuteAsync(DbConnection connection, string command)
    {
      bool hadConnection = connection != null;
      if (connection == null)
        connection = this.Database.GetConnection();

      command = this.Database.PrepareQuery(command);
      try
      {
        if (string.IsNullOrEmpty(command))
          throw new Exception("empty query");

        if (command.ToLower().StartsWith("insert into"))
          return new DirectExecuteResult()
          {
            NumberOfRowsAffected = 1,
            LastID = await connection.QuerySingleAsync<int>(command + string.Format("SELECT {0};", this.Database.QueryScopeID))
          };
        else
          return new DirectExecuteResult()
          {
            NumberOfRowsAffected = connection.Execute(command)
          };
      }
      catch (Exception e)
      {
        //this.Database.OnException(DirectDatabaseExceptionType.OnLoad, command, e);
        //return new DirectExecuteResult() { Exception = e };
        this.Database.OnException(DirectDatabaseExceptionType.OnLoadAsync, command, e);
        throw e;
      }
      finally
      {
        if (!hadConnection)
          connection.CloseAsync();
      }
    }



  }
}
