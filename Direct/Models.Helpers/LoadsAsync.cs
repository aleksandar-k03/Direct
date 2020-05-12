using Direct.Models;
using Direct.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Direct
{
  public static partial class DirectModelHelper
  {

    public static async IAsyncEnumerable<T> LoadEnumerableAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      await foreach (var row in loader.Database.Loader.LoadEnumerableAsync<T>(loader.ContructLoad()))
      {
        row.SetDatabase(loader.Database);
        yield return row;
      }
      yield break;
    }

    //public static async Task<List<T>> LoadAllAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    //  => await loader.Where("[id]>0").LoadAsync();

    public static async Task<T> LoadAsync<T>(this DirectQueryLoader<T> loader, int id) where T : DirectModel
    {
      string command = string.Format(loader.Database.QueryContructLoadByID,
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.Instance.GetIdNameValue(), id);

      var data = await loader.Database.LoadSingleAsync<T>(command);
      if(data != null)
      {
        data.Snapshot.SetSnapshot();
        data.SetDatabase(loader.Database);
      }
      return data;
    }

    public static async Task<T> LoadByGuidAsync<T>(this DirectQueryLoader<T> loader, string id) where T : DirectModel
    {
      string command = string.Format(loader.Database.QueryContructLoadByStringID,
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.Instance.GetIdNameValue(), id);

      var data = await loader.Database.LoadSingleAsync<T>(command);
      if(data != null)
      {
        data.Snapshot.SetSnapshot();
        data.SetDatabase(loader.Database);
      }
      return data;
    }

    public static async Task<T> LoadAsync<T>(this DirectQueryLoader<T> loader, string query) where T : DirectModel
    {
      var data = await loader.Database.LoadSingleAsync<T>(query); ;
      if (data != null)
      {
        data.Snapshot.SetSnapshot();
        data.SetDatabase(loader.Database);
      }
      return data;
    }

    public static async Task<List<T>> LoadAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      string command = string.Format(loader.Database.QueryContructLoadWithLimitConstruct<T>(loader),
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.WhereQuery,
        loader.Additional);

      List<T> result = new List<T>();
      foreach (var row in await loader.Database.LoadAsync<T>(command))
      {
        row.Snapshot.SetSnapshot();
        row.SetDatabase(loader.Database);
        result.Add(row);
      }

      return result;
    }



    /// <summary>
    /// Load single DirectModel async
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loader"></param>
    /// <returns></returns>
    public static async Task<T> LoadSingleAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      string command = string.Format(loader.Database.QueryLoadSingle,
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.WhereQuery);

      var data = await loader.Database.LoadSingleAsync<T>(command);
      if(data != null)
      {
        data.Snapshot.SetSnapshot();
        data.SetDatabase(loader.Database);
      }
      return data;
    }


    public static async Task<int> CountAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      string command = string.Format(loader.Database.QueryCount,
        loader.Instance.GetTableName(),
        loader.WhereQuery);

      int? result = await loader.Database.LoadIntAsync(command);
      if (result.HasValue)
        return result.Value;
      return -1;
    }


    /// <summary>
    /// Loads dynamic object (values not selected will not be present)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loader"></param>
    /// <returns></returns>
    public static async Task<dynamic> LoadDynamicAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      string command = string.Format(loader.Database.QueryContructLoad,
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.WhereQuery,
        loader.Additional);

      return (await loader.Database.LoadAsync(command)).RawData;
    }


  }
}
