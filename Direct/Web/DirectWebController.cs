using Direct.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Direct.Web
{

  public abstract class DirectWebController<T> : ControllerBase, IDirectWebController<T>
    where T : DirectModel
  {

    public abstract DirectDatabaseBase Database { get; }

    protected IActionResult ReturnErrorMessage(string message, Exception e = null)
      => this.BadRequest(new DirectWebControllerResponse() { Message = message, Status = false, Exception = e });

    //
    //  SELECT
    //

    protected abstract bool HasPrivilegesForSelect();

    /// <summary>
    /// Return single item from database based on ID
    /// </summary>
    /// <param name="id">numeric or string id of the element</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Produces("application/json")]
    public virtual async Task<IActionResult> GetSingle(string id)
    {
      if (!this.HasPrivilegesForSelect())
        return this.ReturnErrorMessage("Not enough Privileges");

      // In case that we are hitting get all 
      if (id.Equals("*"))
        return await this.Select();

      return this.Ok(await Database.Query<T>().Where("[id]={0}", id).LoadAsync());
    }


    /// <summary>
    /// Returns all from database
    /// </summary>
    /// <param name="values">Is Select parameter</param>
    /// <param name="limit">Is database limit</param>
    /// <returns></returns>
    [HttpGet("")]
    [HttpGet("{values}/{limit}")]
    [Produces("application/json")]
    public virtual async Task<IActionResult> Select(string values = "*", int limit = 500)
    {
      try
      {
        if (!this.HasPrivilegesForSelect())
          return this.ReturnErrorMessage("Not enough Privileges");

        string whereParamsFromQuery = HttpUtility.UrlDecode(this.Request.QueryString.ToString().Replace("?", string.Empty).Replace("&", " AND ").Split(';')[0]);

        // special cases
        whereParamsFromQuery =
          whereParamsFromQuery.Replace("key=", "`key`=");

        return this.Ok(await Database.Query<T>().Select(values).Where(whereParamsFromQuery).Additional("LIMIT " + limit).LoadDynamicAsync());
      }
      catch(Exception e)
      {
        return this.ReturnErrorMessage("Error", e);
      }
    }


    //
    //  UPDATE
    //

    protected abstract bool HasPrivilegesForUpdate();
    protected virtual void OnAfterUpdate(T entry) { }

    [HttpPost("{id}")]
    [HttpPatch("{id}")]
    [Produces("application/json")]
    public virtual async Task<IActionResult> Update(string id)
    {
      try
      {
        if (!this.HasPrivilegesForUpdate())
          return this.ReturnErrorMessage("Not enough Privileges");

        JObject postData = await this.Request.GetPostData();
        if (postData == null)
          return this.ReturnErrorMessage("No data sent");

        T existingValue = await Database.Query<T>().Where("[id]={0}", id).LoadSingleAsync();

        if (existingValue == null)
          return this.ReturnErrorMessage("No entry with id " + id);

        foreach(var snap in existingValue.Snapshot.PropertySignatures)
        {
          if (snap.IsPrimary)
            continue;

          JToken val = postData[snap.AttributeName];
          if (val == null)
            continue;

          snap.UpdateValue(existingValue, val);
        }

        this.OnAfterUpdate(existingValue);

        return this.Ok(new DirectWebControllerResponseUpdate()
        {
          AffectedRows = await this.Database.UpdateAsync(existingValue)
        });
      }
      catch(Exception e)
      {
        return this.ReturnErrorMessage("", e);
      }
    }

    //
    // INSERT 
    //

    protected abstract bool HasPrivilegesForInsert();
    protected virtual void OnAfterInsert(T entry) { }

    [HttpPut("")]
    [Produces("application/json")]
    public virtual async Task<IActionResult> Insert()
    {
      try
      {
        if (this.HasPrivilegesForInsert() == false)
          return this.ReturnErrorMessage("Not enough Privileges");

        JObject postData = await this.Request.GetPostData();
        if (postData == null)
          return this.ReturnErrorMessage("No data sent");

        T dummy = (T)Activator.CreateInstance(typeof(T));
        dummy.SetDatabase(this.Database);

        foreach (var snap in dummy.Snapshot.PropertySignatures)
        {
          if (snap.IsPrimary)
            continue;

          JToken val = postData[snap.AttributeName];
          if(val == null)
          {
            // we dont need to wory about this values because database will handle it
            if (snap.Nullable || snap.HasDefaultValue)
              continue;

            return this.ReturnErrorMessage($"Missing data for the '{snap.AttributeName}'.");
          }

          snap.UpdateValue(dummy, val);
        }

        dummy = await this.Database.InsertAsync<T>(dummy);
        this.OnAfterInsert(dummy);

        return this.Ok(dummy);
      }
      catch(Exception e)
      {
        return this.ReturnErrorMessage("", e);
      }
    }


    //
    // DELETE
    //

    protected abstract bool HasPrivilegesForDelete();
    protected virtual void OnAfterDelete(T entry, bool isDeleted) { }

    [HttpDelete("{id}")]
    [Produces("application/json")]
    public virtual async Task<IActionResult> Delete(string id)
    {
      try
      {

        if (this.HasPrivilegesForDelete() == false)
          return this.ReturnErrorMessage("Not enough Privileges");


        T existingValue = await Database.Query<T>().Where("[id]={0}", id).LoadSingleAsync();
        if (existingValue == null)
          return this.ReturnErrorMessage("No entry with id " + id);

        bool isDeleted = await existingValue.DeleteAsync(this.Database);
        this.OnAfterDelete(existingValue, isDeleted);

        return this.Ok(new DirectWebControllerResponseDelete()
        {
          IsDeleted = isDeleted
        });
      }
      catch (Exception e)
      {
        return this.ReturnErrorMessage("", e);
      }
    }

  }
}
