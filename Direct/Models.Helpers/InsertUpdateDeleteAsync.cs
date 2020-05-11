using Direct.Results;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Direct.Models;

namespace Direct
{
  public static partial class DirectModelHelper
  {

    public static async Task InsertOrUpdateAsync(this DirectDatabaseBase db, DirectModel model)
    {
      if (model.IntegerPrimary && model.ID.HasValue)
        await UpdateAsync(db, model);
      else
        await InsertAsync<DirectModel>(db, model);
    }

    public static async Task<T> InsertAsync<T>(this DirectDatabaseBase db, DirectModel model) where T : DirectModel
    {
      string command = string.Format(DirectModelHelper.GetDatabase(model).QueryConstructInsertQuery,
        model.GetTableName(),
        model.Snapshot.GetPropertyNamesForInsert(), model.Snapshot.GetPropertyValuesForInsert());
      DirectExecuteResult result = await db.ExecuteAsync(command);
      if (result.IsSuccessfull && result.LastID.HasValue)
      {
        model.ID = (int)result.LastID;
        model.Snapshot.SetSnapshot();
        return (T)model;
      }
      return (T)model;
    }

    public static async Task<int?> UpdateAsync(this DirectDatabaseBase db, DirectModel model)
    {
      if (model.IntegerPrimary && !model.ID.HasValue)
        throw new Exception("ID is not set, maybe this table was not loaded");
      
      // UPDATE MobilePaywall.core.A SET A=1 WHERE AID=1
      string command = string.Format(DirectModelHelper.GetDatabase(model).QueryConstructUpdateQuery,
        model.GetTableName(),
        model.Snapshot.GetUpdateData(),
        model.GetIdNameValue(), 
        (model.IntegerPrimary ? model.ID.Value.ToString() : string.Format("'{0}'", model.GetStringID())));

       DirectExecuteResult result = await db.ExecuteAsync(command);
      if (!result.IsSuccessfull)
        return null;
      else
      {
        model.Snapshot.SetSnapshot();
        return result.NumberOfRowsAffected;
      }
    }

    public static async Task<bool> DeleteAsync(this DirectDatabaseBase db, DirectModel model)
    {
      if (model.IntegerPrimary && !model.ID.HasValue)
        throw new Exception("THIS model has not ID");

      string command = string.Format(DirectModelHelper.GetDatabase(model).QueryDelete,
        model.GetTableName(),
        model.GetIdNameValue(),
        (model.IntegerPrimary ? model.ID.Value.ToString() : string.Format("'{0}'", model.GetStringID())));
      DirectExecuteResult result = await db.ExecuteAsync(command);
      if (result.IsSuccessfull)
      {
        model.ID = null;
        model.Snapshot.SetSnapshot();
        return true;
      }
      return false;
    }

  }
}
