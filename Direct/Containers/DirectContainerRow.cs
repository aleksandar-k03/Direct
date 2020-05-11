using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Direct
{
  public class DirectContainerRow
  {
    private IDictionary<string, object> Dictionary = null;
    public string[] ColumnNames { get; protected set; } = null;
    public object[] RawDataArray { get; protected set; } = null;
    public dynamic RawData { get; protected set; } = null;

    // SUMMARY: Does this row has values at all
    public bool HasValue
    {
      get
      {
        return this.RawData != null;
      }
    }

    // SUMMARY: Return value by column index
    public object this[int columnIndex]
    {
      get
      {
        if (columnIndex > this.RawDataArray.Length)
          return null;
        return this.RawDataArray[columnIndex];
      }
    }
    public object this[string columnIndex]
    {
      get
      {
        if (!this.Dictionary.ContainsKey(columnIndex))
          return null;
        return this.Dictionary[columnIndex];
      }
    }

    public DirectContainerRow(dynamic row)
    {
      this.RawData = row;
      this.Dictionary = (IDictionary<string, object>)this.RawData;
      this.ColumnNames = this.Dictionary.Keys.ToArray();
      this.RawDataArray = this.Dictionary.Values.ToArray();
    }

    public virtual string GetString(string columnName)
    {
      try { return this[columnName].ToString(); }
      catch(Exception e) { return string.Empty; }
    }

    public virtual bool? GetBool(string columnName)
    {
      try { return (bool)this[columnName]; }
      catch (Exception e) { return null; }
    }

    public virtual DateTime? GetDate(string columnName)
    {
      try { return (DateTime?)this[columnName]; }
      catch (Exception e) { return null; }
    }

    public virtual int? GetInt(string columnName)
    {
      try { return (int?)this[columnName]; }
      catch (Exception e) { return null; }
    }

    public virtual long? GetLong(string columnName)
    {
      try { return (long?)this[columnName]; }
      catch (Exception e) { return null; }
    }

    public virtual decimal? GetDecimal(string columnName)
    {
      try { return (decimal?)this[columnName]; }
      catch (Exception e) { return null; }
    }

    public virtual double? GetDouble(string columnName)
    {
      try { return (double?)this[columnName]; }
      catch (Exception e) { return null; }
    }

    public virtual Guid? GetGuid(string columnName)
    {
      try { return (Guid?)this[columnName]; }
      catch (Exception e) { return null; }
    }

  }
}
