using Direct.Helpers;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Models
{
  [Serializable()]
  public abstract class DirectModel : IDisposable
  {

    internal string InternalID { get; private set; } = string.Empty;
    internal DirectModelSnapshot Snapshot { get; set; } = null;
    internal string IdName { get; set; } = string.Empty;
    internal string TableName { get; set; } = string.Empty;
    public virtual string GetSchemaName() => "";
    internal string BulkVariableName => string.Format("{0}_{1}", this.IdName, this.InternalID);
    protected DirectDatabaseBase Database { get; set; } = null;
    internal bool IntegerPrimary { get; set; } = true;
    public virtual DirectDatabaseType DatabaseType { get => this.GetDatabase().DatabaseType; }
    protected virtual DirectDatabaseBase DatabaseReference => null; // default reference to the manager in case that Database instance is not present

    public int? ID
    {
      get
      {
        try
        {
          if (!this.IntegerPrimary) return null;
          int? result = (int?)this.Snapshot.IdPropertyInfo.GetValue(this);
          return result.HasValue && result.Value == 0 ? null : result;
        }
        catch(Exception e)
        {
          return null;
        }
      }
      set
      {
        if (!this.IntegerPrimary) return;
        this.Snapshot.IdPropertyInfo.SetValue(this, value);
      }
    }
    public string GetStringID()
    {
      if (this.IntegerPrimary) return string.Empty;
      string result = (string)this.Snapshot.IdPropertyInfo.GetValue(this);
      if (!string.IsNullOrEmpty(result))
        return result;
      return this.InternalID;
    }
    public string SetStringID(string id)
    {
      if (this.IntegerPrimary) return string.Empty;
      this.Snapshot.IdPropertyInfo.SetValue(this, id);
      this.InternalID = id;
      return id;
    }
    public PropertyInfo GetProperty(string propertyName) => this.Snapshot.GetProperty(propertyName);
    public DirectDatabaseBase GetDatabase() => this.Database == null ? this.DatabaseReference : this.Database;
    public void SetDatabase(DirectDatabaseBase db) => this.Database = db;
    public string GetInternalID() => this.InternalID;
    public bool HasChanges()
    {
      return this.Snapshot.GetAffected().Count > 0;
    }

    ///
    /// CONSTRUCTOR && DECONSTRUCTOR
    ///

    
    public DirectModel(string tableName, string id_name, DirectDatabaseBase db)
    {
      this.TableName = tableName;
      this.IdName = id_name;
      this.Database = db;
      this.InternalID = this.ConstructSignature() + Guid.NewGuid().ToString().Replace("-", string.Empty);

      this.Snapshot = new DirectModelSnapshot(this);
      this.PrepareProperties();
      this.Snapshot.SetSnapshot();
    }

    ~DirectModel() => OnDispose();
    public void Dispose() => OnDispose();

    protected void OnDispose()
    {
      if (this.Database != null)
        this.Database.Dispose();
    }

    protected string ConstructSignature()
    {
      string[] split = (from s in this.TableName.Split('_') where s.Length > 2 select s).ToArray();
      if (split.Length == 1)
        return split[0].Substring(0, 3).ToUpper();
      else
        return split[0].Substring(0, 1).ToUpper() + split[1].Substring(0, 2).ToUpper();
    }

    ///
    /// Get data
    ///

    internal string GetTableName() 
      => string.Format("{0}{1}",
        !string.IsNullOrEmpty(this.GetSchemaName()) ? string.Format("[{0}]", this.GetSchemaName()) + "." : "", 
        string.Format("[{0}]", this.TableName)); 

    internal string GetIdNameValue() => this.IdName;

    public int? WaitID(int forSeconds = 30)
    {
      if (this.ID.HasValue)
        return this.ID;

      DateTime started = DateTime.Now;
      do
      {
        if ((DateTime.Now - started).TotalSeconds >= forSeconds)
          break;
      }
      while (this.ID == null);

      return this.ID;
    }
    public int WaitIDExplicit(int forSeconds = 30)
    {
      int? id = this.WaitID(30);
      if (!id.HasValue)
        throw new Exception("We did not get any response");
      return id.Value;
    }

    ///
    /// Overrides
    ///

    internal List<Action<DirectModel>> OnAfterInsertActions = new List<Action<DirectModel>>();
    internal List<Action<DirectModel>> OnAfterUpdateActions = new List<Action<DirectModel>>();
    internal List<Action<DirectModel>> OnAfterDeleteActions = new List<Action<DirectModel>>();

    public void AddOnAfterInsert(Action<DirectModel> action) => this.OnAfterInsertActions.Add(action);
    public void AddOnAfterUpdate(Action<DirectModel> action) => this.OnAfterUpdateActions.Add(action);
    public void AddOnAfterDelete(Action<DirectModel> action) => this.OnAfterDeleteActions.Add(action);

    public virtual void OnBeforeInsert() { }
    public virtual void OnBeforeUpdate() { }
    public virtual void OnBeforeDelete() { }


    //
    // SUMMARY: Properties manipulation
    internal void PrepareProperties()
      => this.Snapshot.PrepareProperties();

    internal DirectDatabaseBase GetDatabase(DirectDatabaseBase db = null)
    {
      if (db != null) return db;
      return this.GetDatabase();
      throw new Exception("Database is not set!!");
    }


    ///
    /// LOAD
    /// 


    ///
    /// INSERT
    /// 

    public void Insert(DirectDatabaseBase db = null)
    {
      foreach (var action in OnAfterInsertActions)
        action?.Invoke(this);
      this.GetDatabase(db).Insert<DirectModel>(this);
    }
    public T Insert<T>(DirectDatabaseBase db = null) where T : DirectModel
    {
      foreach (var action in OnAfterInsertActions)
        action?.Invoke(this);
      return this.GetDatabase(db).Insert<T>(this);
    }

    public Task InsertAsync(DirectDatabaseBase db = null)
    {
      foreach (var action in OnAfterInsertActions)
        action?.Invoke(this);
      return this.GetDatabase(db).InsertAsync<DirectModel>(this);
    }
    public Task<T> InsertAsync<T>(DirectDatabaseBase db = null) where T : DirectModel
    {
      foreach (var action in OnAfterInsertActions)
        action?.Invoke(this);

      return this.GetDatabase(db).InsertAsync<T>(this);
    }

    public void InsertLater(DirectDatabaseBase db = null)
    {
      foreach (var action in OnAfterInsertActions)
        action?.Invoke(this);
      this.GetDatabase(db).TransactionalManager.Insert(this);
    }
    public void InsertOrUpdate(DirectDatabaseBase db = null) => this.GetDatabase(db).InsertOrUpdate(this);
    public async Task InsertOrUpdateAsync(DirectDatabaseBase db = null) => await this.GetDatabase(db).InsertOrUpdateAsync(this);


    /// 
    /// UPDATE
    /// 

    public void Update(bool forceUpdateIfNothingIsChanged = false, DirectDatabaseBase db = null)
    {
      if (forceUpdateIfNothingIsChanged == false && this.Snapshot.GetAffected().Count == 0)
      {
        var updated = this.Snapshot.GetProperty("updated");
        if(updated != null)
        {
          updated.SetValue(this, DateTime.Now);
          this.GetDatabase(db).Execute(this.ConstructUpdateUpdatedQuery());
        }
        return;
      }

      foreach (var action in OnAfterUpdateActions)
        action?.Invoke(this);

      this.GetDatabase(db).Update(this);
    }
    public void UpdateLater(bool forceUpdateIfNothingIsChanged = false)
    {
      if (forceUpdateIfNothingIsChanged == false && this.Snapshot.GetAffected().Count == 0)
      {
        var updated = this.Snapshot.GetProperty("updated");
        if (updated != null)
        {
          updated.SetValue(this, DateTime.Now);
          this.GetDatabase().TransactionalManager.Add(this.ConstructUpdateUpdatedQuery());
        }
        return;
      }

      if (forceUpdateIfNothingIsChanged == false && this.Snapshot.GetAffected().Count == 0)
        return;

      foreach (var action in OnAfterUpdateActions)
        action?.Invoke(this);

      this.GetDatabase().TransactionalManager.Add(this);
    }
    public async Task<int?> UpdateAsync(bool forceUpdateIfNothingIsChanged = false, DirectDatabaseBase db = null)
    {
      if (forceUpdateIfNothingIsChanged == false && this.Snapshot.GetAffected().Count == 0)
      {
        var updated = this.Snapshot.GetProperty("updated");
        if (updated != null)
        {
          updated.SetValue(this, DateTime.Now);
          await this.GetDatabase(db).ExecuteAsync(this.ConstructUpdateUpdatedQuery());
        }
        return 0;
      }

      foreach (var action in OnAfterUpdateActions)
        action?.Invoke(this);

      return await this.GetDatabase(db).UpdateAsync(this);
    }


    /// 
    /// DELETE
    /// 

    public bool Delete(DirectDatabaseBase db = null)
    {
      var result = this.GetDatabase(db).Delete(this);
      if (result)
        foreach (var action in OnAfterUpdateActions)
          action?.Invoke(this);
      return result;
    }
    public async Task<bool> DeleteAsync(DirectDatabaseBase db = null)
    {
      var result = await this.GetDatabase(db).DeleteAsync(this);
      if (result)
        foreach (var action in OnAfterUpdateActions)
          action?.Invoke(this);
      return result;
    }


  }
}
