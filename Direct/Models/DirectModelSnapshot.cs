using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Direct.Models
{
  [Serializable()]
  public class DirectModelSnapshot
  {
    private static List<string> IgnoredPropertyNames = new List<string>() { "ID", "OnAfterInsert", "OnAfterUpdate", "DatabaseType" };
    internal List<DirectModelPropertySignature> PropertySignatures { get; set; } = null;
    internal PropertyInfo[] Properties { get; set; } = null;
    private Dictionary<string, string> SnapshotObjects = null;
    internal string IdName { get; set; } = string.Empty;
    internal PropertyInfo IdPropertyInfo { get; set; } = null;

    private DirectModel Model { get; set; } = null;
    private string _propertyNamesForInsert = string.Empty;

    public DirectModelSnapshot(DirectModel model)
    {
      this.Model = model;
      this.PrepareProperties();
    }

    public void PrepareProperties()
    {
      if (this.Properties != null) return;
      this.Properties = this.Model.GetType().GetProperties();
      foreach (var prop in this.Properties)
        if (prop.Name.Equals("ID"))
          this.IdPropertyInfo = prop;

      PropertySignatures = new List<DirectModelPropertySignature>();
      for (int i = 0; i < this.Properties.Length; i++)
        if (!IgnoredPropertyNames.Contains(this.Properties[i].Name))
        {
          var signature = new DirectModelPropertySignature(this.Properties[i]);
          if(signature.IsPrimary)
          {
            this.IdPropertyInfo = this.Properties[i];
            if (Properties[i].PropertyType == typeof(string))
            {
              this.Model.IntegerPrimary = false;
              Properties[i].SetValue(this.Model, this.Model.InternalID);
            }
          }
          PropertySignatures.Add(signature);
        }
    }

    internal bool ContainsPropery(string propertyName)
    {
      foreach (var prop in this.PropertySignatures)
        if (prop.Name.Equals(propertyName))
          return true;
      return false;
    }

    internal PropertyInfo GetProperty(string propertyName)
    {
      if (propertyName.ToLower().Equals("id"))
        return this.IdPropertyInfo;

      for (int i = 0; i < this.PropertySignatures.Count; i++)
        if (this.PropertySignatures[i].Name.Equals(propertyName))
          return this.Properties[i];
      return null;
    }

    /// <summary>
    /// Update Property value
    /// </summary>
    /// <param name="propertyName">Name of the propery</param>
    /// <param name="value">New value</param>
    /// <returns>If property does not exists, return false, else true</returns>
    internal bool UpdatePropertyValue(string propertyName, object value)
    {
      PropertyInfo pi = this.GetProperty(propertyName);
      if (pi == null)
        return false;

      pi.SetValue(this.Model, value);
      return true;
    }

    ///
    /// Main manipulation
    ///

    internal string GetSqlValue(int propertyID, bool throwNullableException)
    {
      if (this.PropertySignatures[propertyID].UpdateDateTime && Model.GetDatabase() != null)
        return Model.GetDatabase().CurrentDateQueryString;

      string value = this.GetValue(this.PropertySignatures[propertyID]);
      if (value.Equals("NULL"))
      {
        if (throwNullableException && this.PropertySignatures[propertyID].Nullable == false)
          throw new Exception(string.Format("Property with name '{0}' is set as NOT NULLABLE", this.PropertySignatures[propertyID].PropertyName));
        return value;
      }

      return Model.GetDatabase().GetObjectQueryValue(Properties[propertyID], this.PropertySignatures[propertyID], this.Model);
    }

    internal string GetValue(DirectModelPropertySignature signature)
    {
      var val = Model.GetType().GetProperty(signature.PropertyName).GetValue(this.Model, null);

      // override nullable strings
      if (val != null && Model.GetType().GetProperty(signature.PropertyName).PropertyType == typeof(string) && string.IsNullOrEmpty(val.ToString()) && signature.Nullable)
        return "NULL";

      return val == null ? "NULL" : val.ToString();
    }

    ///
    /// INSERT
    ///

    internal string GetPropertyNamesForInsert(bool includeAll = false)
    {
      if (!string.IsNullOrEmpty(this._propertyNamesForInsert))
        return this._propertyNamesForInsert;

      this.PrepareProperties();

      for (int i = 0; i < this.PropertySignatures.Count; i++)
      {
        if (!includeAll && this.PropertySignatures[i].NotUpdatable)
          continue;

        string sqlValue = this.GetSqlValue(i, false);


        if (!includeAll && sqlValue.Equals("NULL") && this.PropertySignatures[i].Nullable)
          continue;

        if (!includeAll && sqlValue.Equals("NULL") && this.PropertySignatures[i].HasDefaultValue)
          continue;

        if (!includeAll && this.PropertySignatures[i].IsPrimary && this.Model.IntegerPrimary == true)
          continue;

        this._propertyNamesForInsert += (!string.IsNullOrEmpty(this._propertyNamesForInsert) ? "," : "") + this.PropertySignatures[i].Name;
      }
      return this._propertyNamesForInsert;
    }

    internal string GetPropertyValuesForInsert(bool includeAll = false) 
    {
      string result = "";

      for (int i = 0; i < this.PropertySignatures.Count; i++)
      {
        if (!includeAll && this.PropertySignatures[i].NotUpdatable)
          continue;

        string value = this.GetSqlValue(i, throwNullableException: true);

        if (!includeAll && value.Equals("NULL") && this.PropertySignatures[i].Nullable)
          continue;

        if (!includeAll && value.Equals("NULL") && this.PropertySignatures[i].HasDefaultValue)
          continue;
        if (!includeAll && this.PropertySignatures[i].IsPrimary && this.Model.IntegerPrimary == true)
          continue;

        result += (!string.IsNullOrEmpty(result) ? "," : "") + value;
      }

      return result;
    }

    ///
    /// UPDATE
    ///

    internal string GetUpdateData()
    {
      this.PrepareProperties();
      if (this.PropertySignatures.Count == 0)
        throw new Exception("Properties are not OK");
      string result = "";

      List<int> affectedIds = this.GetAffected();
      if (affectedIds == null)
        for (int i = 0; i < this.PropertySignatures.Count; i++)
        {
          if (this.PropertySignatures[i].NotUpdatable) continue;
          result += (result.Equals("") ? "" : ",") + string.Format("{0}={1}", this.PropertySignatures[i].Name, this.GetSqlValue(i, throwNullableException: true));
        }
      else
        foreach (int id in affectedIds)
          result += (result.Equals("") ? "" : ",") + string.Format("{0}={1}", this.PropertySignatures[id].Name, this.GetSqlValue(id, throwNullableException: true));

      return result;
    }

    ///
    /// SNAPSHOT
    ///

    public string GetCurrentValue(string name)
    {
      if (this.SnapshotObjects.ContainsKey(name))
        return this.SnapshotObjects[name];
      return string.Empty;
    }

    internal void SetSnapshot()
    {
      this.PrepareProperties();
      this.SnapshotObjects = new Dictionary<string, string>();
      for (int i = 0; i < this.PropertySignatures.Count; i++)
      {
        if (this.PropertySignatures[i].NotUpdatable) continue;

        this.SnapshotObjects.Add(this.PropertySignatures[i].Name, this.GetSqlValue(i, throwNullableException: false));
      }
    }

    internal void DeleteSnapshot() => this.SnapshotObjects = null;

    internal List<int> GetAffected()
    {
      if (this.SnapshotObjects == null)
        return null;

      List<int> propertiesChanged = new List<int>();
      for (int i = 0; i < this.PropertySignatures.Count; i++)
      {
        if (this.PropertySignatures[i].NotUpdatable) continue;
        if (!this.SnapshotObjects.ContainsKey(this.PropertySignatures[i].Name))
          throw new Exception("WTF? Some properties are changed in meanwhile");
        if (this.SnapshotObjects[this.PropertySignatures[i].Name] != this.GetSqlValue(i, throwNullableException: false))
          propertiesChanged.Add(i);
      }

      return propertiesChanged;
    }


  }
}
