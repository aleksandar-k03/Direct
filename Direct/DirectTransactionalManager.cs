using Direct.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct
{
  public class DirectTransactionalManager
  {
    private static object LockObj = new object();
    private bool IsRunExecuting { get; set; } = false;

    private DirectDatabaseBase _database = null;
    public int Limit { get; set; } = 2000;
    private List<string> _queries = new List<string>();
    private List<DirectModel> _queryModels = new List<DirectModel>();
    private List<DirectModel> _queryInserter = new List<DirectModel>();
    private Dictionary<DirectModel, string> _queryLoader = new Dictionary<DirectModel, string>();

    public int Count { get { return this._queries.Count + this._queryInserter.Count + this._queryLoader.Count + this._queryModels.Count; } }

    public DirectTransactionalManager(DirectDatabaseBase db)
    {
      this._database = db;
    }

    ///
    /// Insert values 
    ///

    // SUMMARY: Used to update last change of the model (not multiple times)
    public void Add(DirectModel model)
    {
      lock (LockObj)
      {
        if (!this._queryInserter.Contains(model) && !this._queryModels.Contains(model))
          this._queryModels.Add(model);

        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    public void Add(string query, params object[] parameters) => Add(this._database.Construct(query, parameters));
    public void Add(string command)
    {
      lock (LockObj)
      {
        this._queries.Add(command);
        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    public void Insert(DirectModel model)
    {
      lock (LockObj)
      {
        if (!this._queryInserter.Contains(model) && !this._queryModels.Contains(model))
          this._queryInserter.Add(model);

        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    public void Load(DirectModel model, string loadQuery)
    {
      lock (LockObj)
      {
        if (!this._queryLoader.ContainsKey(model))
          this._queryLoader.Add(model, loadQuery);

        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    ///
    /// Runners
    ///

    public Task RunAsync()
      => Task.Factory.StartNew(() => { Run(); });

    public void Run()
    {
      if (this.Count == 0) return;

      int originalTasks = this.Count;
      List<string> queries = new List<string>();
      List<DirectModel> queryModels = new List<DirectModel>();
      List<DirectModel> queryInserter = new List<DirectModel>();
      Dictionary<DirectModel, string> queryLoader = new Dictionary<DirectModel, string>();

      lock (LockObj)
      {
        queries = new List<string>(this._queries); this._queries.Clear();
        queryModels = new List<DirectModel>(this._queryModels); this._queryModels.Clear();
        queryInserter = new List<DirectModel>(this._queryInserter); this._queryInserter.Clear();
        queryLoader = new Dictionary<DirectModel, string>(this._queryLoader); this._queryLoader.Clear();
        GC.Collect();
      }

      string mainQuery = "";
      try
      {
        this.IsRunExecuting = true;

        foreach (string query in queries)
        {
          string qq = query.Trim();
          mainQuery += qq + (qq.EndsWith(";") ? "" : ";");
        }
        this._queries = new List<string>();

        DateTime create = DateTime.Now;
        Console.WriteLine(string.Format("TransactionalManager is starting with {0} tasks", originalTasks));

        foreach (var model in queryInserter)
          mainQuery += model.ConstructInsertQuery();

        foreach (var model in queryModels)
          mainQuery += model.ConstructUpdateQuery();

        if (!string.IsNullOrEmpty(mainQuery))
          this._database.Execute(mainQuery);

        double ms = (DateTime.Now - create).TotalMilliseconds;
        if (ms > 1500)
        {
          int a = 0;
        }
        Console.WriteLine(string.Format("TransactionalManager is finished {0} tasks after {1}", originalTasks, ms));
        //Console.WriteLine(mainQuery);
        Console.WriteLine();
        Console.WriteLine();
      }
      catch (Exception e)
      {
        int a = 0;
      }
      finally
      {
        this.IsRunExecuting = false;
      }
    }
    
  }
}
