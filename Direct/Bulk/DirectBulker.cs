using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Bulk
{
  public class DirectBulker
  {
    private static object LockObj = new object();

    public int MaximumNumberOfConnections = 3;
    private int CurrentNumberOfConnections = 0;
    public long RowsInserted { get; protected set; } = 0;

    private DirectDatabaseBase database = null;
    private List<BulkModel> Models = new List<BulkModel>();

    public DirectBulker(DirectDatabaseBase db)
    {
      this.database = db;
    }

    public void Add(BulkModel model)
    {
      lock (LockObj)
      {
        this.Models.Add(model);
      }
    }


    public void Run()
    {
      if (this.Models.Count == 0)
        return;

      if (this.CurrentNumberOfConnections >= this.MaximumNumberOfConnections)
      {
        Console.WriteLine($"DirectBulker:: MaxConnection of {MaximumNumberOfConnections} is reached. Wait until other connections catch up! ");
        do { } while (this.CurrentNumberOfConnections != 0);
        Console.WriteLine($"DirectBulker:: Catched up. Current number of connections is {MaximumNumberOfConnections}");
      }

      List<BulkModel> models = null;
      lock (LockObj)
      {
        models = new List<BulkModel>(this.Models);
        this.Models = new List<BulkModel>();
        GC.Collect();
      }

      Task.Factory.StartNew(() => { Execute(models); });
    }

    public void RunAndWait()
    {
      this.Run();
      do { } while (this.CurrentNumberOfConnections != 0);
    }

    private async Task Execute(List<BulkModel> models)
    {
      CurrentNumberOfConnections++;

      SortedDictionary<int, string> queries = new SortedDictionary<int, string>();

      foreach (var bulkModel in models)
      {

        if (!queries.ContainsKey(bulkModel.Priority))
        {
          string propertyForInsert_id = (bulkModel.Model.ID.HasValue ? bulkModel.Model.GetIdNameValue() + "," : string.Empty);

          string header = string.Format("INSERT INTO {0}.{1}{2} ({3}) VALUES ",
            bulkModel.Model.GetDatabase().DatabaseName, bulkModel.Model.GetDatabase().DatabaseSchemeString, bulkModel.Model.GetTableName(),
            propertyForInsert_id + bulkModel.Model.Snapshot.GetPropertyNamesForInsert(true));
          queries.Add(bulkModel.Priority, header);
        }

        // TODO: remove ID part because we will not need it after initial testings
        string id = bulkModel.Model.ID.HasValue ? bulkModel.Model.ID.Value + "," : string.Empty;
        queries[bulkModel.Priority] += string.Format("({0}),", id + bulkModel.Model.Snapshot.GetPropertyValuesForInsert(true));
      }

      string finalQuery = "";
      foreach (var t in queries)
      {
        finalQuery += (t.Value.Substring(0, t.Value.Length - 1) + ";") + Environment.NewLine;
      }

      DateTime dt = DateTime.Now;
      //var result = this.database.Execute("SET FOREIGN_KEY_CHECKS=0;START TRANSACTION;" + finalQuery+ "COMMIT;SET FOREIGN_KEY_CHECKS=1;");
      var result = this.database.Execute("START TRANSACTION;" + finalQuery + "COMMIT;");
      double ms = (DateTime.Now - dt).TotalMilliseconds;

      Console.WriteLine($"DirectBulker: Inserted {models.Count} objects in {ms}ms (in use ${CurrentNumberOfConnections}/{MaximumNumberOfConnections})! ");
      RowsInserted += models.Count;

      CurrentNumberOfConnections--;
    }

  }
}
