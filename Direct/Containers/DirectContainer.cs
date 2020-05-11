using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Direct
{
  public enum DirectColumnType { TypeInt, TypeGuid, TypeDateTime, TypeBit, TypeDataByte, TypeString, Unknown }

  public class DirectContainer
  {
    public dynamic Data { get; protected set; } = null;
    public List<DirectContainerRow> DirectRows { get; protected set; } = new List<DirectContainerRow>();

    public bool HasValue { get { return this.Data != null; } }
    public string[] ColumnNames => this.DirectRows.Count > 0 ? this.DirectRows[0].ColumnNames : new string[] { };
    public int ColumnCount { get { return this.ColumnNames.Length; } }
    public int RowsCount { get { return this.DirectRows != null ? this.DirectRows.Count : 0; } }
    public DirectContainerRow DefaultRow => this.DirectRows.Count > 0 ? this.DirectRows[0] : null;

    public DirectContainerRow this[int i]
    {
      get
      {
        if (this.DirectRows == null || i < 0 || i > this.DirectRows.Count)
          return null;
        return this.DirectRows[i];
      }
    }

    public IEnumerable<DirectContainerRow> Rows
    {
      get
      {
        foreach (var row in this.DirectRows)
          yield return row;
      }
    }

    public DirectContainer(dynamic rawData)
    {
      this.Data = rawData;
      foreach (var row in this.Data)
        this.DirectRows.Add(new DirectContainerRow(row));
    }


    // SUMMARY: Get string by Column name and Row count
    public virtual string GetString(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return string.Empty;
      return this.DirectRows[depth].GetString(columnName);
    }
    public virtual bool? GetBool(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return null;
      return this.DirectRows[depth].GetBool(columnName);
    }
    public virtual bool GetBoolean(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return false;
      bool? result = this.DirectRows[depth].GetBool(columnName);
      return result.HasValue ? result.Value : false;
    }
    public virtual DateTime? GetDate(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return null;
      return this.DirectRows[depth].GetDate(columnName);
    }
    public virtual int? GetInt(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return null;
      return this.DirectRows[depth].GetInt(columnName);
    }
    public virtual long? GetLong(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return null;
      return this.DirectRows[depth].GetLong(columnName);
    }
    public virtual Guid? GetGuid(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return null;
      return this.DirectRows[depth].GetGuid(columnName);
    }
    public virtual double? GetDouble(string columnName, int depth = 0)
    {
      if (depth < 0 || depth > this.DirectRows.Count) return null;
      return this.DirectRows[depth].GetDouble(columnName);
    }


    //// SUMMARY: Convert informations into class
    //public T Convert<T>(int depth = 0)
    //{
    //  if (this._table == null || depth > this._table.Rows.Count)
    //    return default(T);

    //  T temp = (T)Activator.CreateInstance(typeof(T));

    //  foreach (string column in this.ColumnNames)
    //  {
    //    PropertyInfo property = (from p in typeof(T).GetProperties() where p.Name.Equals(column) select p).FirstOrDefault();
    //    if (property == null || !property.CanWrite)
    //      continue;
    //    ConvertProperty<T>(temp, property, column, depth);
    //  }

    //  return temp;
    //}

    //public void ConvertProperty<T>(T temp, PropertyInfo property, string column, int depth)
    //{
    //  string typename = property.PropertyType.Name;
    //  try
    //  {
    //    switch (typename.ToLower())
    //    {
    //      case "string":
    //        property.SetValue(temp, this.GetString(column, depth));
    //        break;
    //      case "int32":
    //        int? intResult = this.GetInt(column, depth);
    //        if (intResult.HasValue)
    //          property.SetValue(temp, intResult.Value);
    //        break;
    //      case "datetime":
    //        DateTime? dateTimeResult = this.GetDate(column, depth);
    //        if (dateTimeResult.HasValue)
    //          property.SetValue(temp, dateTimeResult.Value);
    //        break;
    //      case "double":
    //        double? doubleResult = this.GetDouble(column, depth);
    //        if (doubleResult.HasValue)
    //          property.SetValue(temp, doubleResult.Value);
    //        break;
    //      case "boolean":
    //        bool? boolResult = this.GetBool(column, depth);
    //        if (boolResult.HasValue)
    //          property.SetValue(temp, boolResult.Value);
    //        break;

    //      case "nullable`1":
    //        if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.Double"))
    //        {
    //          double? doubleNullResult = this.GetDouble(column, depth);
    //          if (doubleNullResult.HasValue)
    //            property.SetValue(temp, doubleNullResult.Value);
    //          break;
    //        }
    //        else if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.Int32"))
    //        {
    //          int? intNullResult = this.GetInt(column, depth);
    //          if (intNullResult.HasValue)
    //            property.SetValue(temp, intNullResult.Value);
    //          break;
    //        }
    //        else if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.DateTime"))
    //        {
    //          DateTime? dateTimeNullResult = this.GetDate(column, depth);
    //          if (dateTimeNullResult.HasValue)
    //            property.SetValue(temp, dateTimeNullResult.Value);
    //          break;
    //        }
    //        else if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.Int64,"))
    //        {
    //          long? longResult = this.GetLong(column, depth);
    //          if (longResult.HasValue)
    //            property.SetValue(temp, longResult.Value);
    //          break;
    //        }
    //        break;

    //      default:
    //        break;
    //    }
    //  }
    //  catch (Exception e)
    //  {
    //    int a = 0;
    //  }
    //}

    //public List<T> ConvertList<T>()
    //{
    //  List<T> list = new List<T>();
    //  if (this._table == null || this._table.Rows.Count == 0)
    //    return list;

    //  for (int i = 0; i < this._table.Rows.Count; i++)
    //    list.Add(this.Convert<T>(i));

    //  return list;
    //}


  }
}
