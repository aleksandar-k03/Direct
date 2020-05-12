using Direct.Helpers;
using Direct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Direct
{
  public static class DirectHelperObjectToQueryValue
  {


    internal static string EscapeString(this string input)
    {
      if (string.IsNullOrEmpty(input))
        return input;

      if (input.Length >= 1 && input[input.Length - 1] == '\\')
        input = input.Substring(0, input.Length - 1);

      //return System.Security.SecurityElement.Escape(input.ToString()
      //  .Replace("'", string.Empty));
      return input.ToString().Replace("'", string.Empty);
    }


    internal static string GetObjectQueryValue(this DirectDatabaseBase db, PropertyInfo obj, DirectModelPropertySignature signature, object parentObject)
    {
      var model = parentObject as DirectModel;
      if (obj == null)
        return "NULL";

      if (signature.UpdateDateTime)
        if(db != null)
          return db.CurrentDateQueryString;
        else
          return "CURRENT_TIMESTAMP";


      object value = obj.GetValue(parentObject);
      var type = obj.PropertyType;

      if (type == typeof(DirectTime))
        return db.CurrentDateQueryString;
      else if (type == typeof(DirectScopeID))
        return db.QueryScopeID;
      else if (type == typeof(bool))
        return (bool)value ? "1" : "0";
      else if (type == typeof(int) || type == typeof(double) || type == typeof(long)
        || type == typeof(uint) || type == typeof(ulong) || type == typeof(short)
        || type == typeof(int?) || type == typeof(double?) || type == typeof(long?)
        || type == typeof(uint?) || type == typeof(ulong?) || type == typeof(short?))
        return value.ToString();
      else if (type == typeof(string) || type == typeof(Guid) || type == typeof(String) || type == typeof(char))
      {
        string extra = model.GetDatabase().DatabaseType == DirectDatabaseType.SQLServer ? "N" : ""; // for adding utf8 data
        return string.Format(extra + "'{0}'", value.ToString().EscapeString());
      }
      else if(type == typeof(byte[]))
      {
        byte[] data = (byte[])value;
        bool hasAllZeroes = data.All(singleByte => singleByte == 0);
        if (hasAllZeroes)
          return "NULL";
        string hex = StringToHex.ToHexString(data, false);
        //data.ToList().ForEach(b => hex += b.ToString("x2"));
        return string.Format("X'{0}'", hex);
      }
      else if (type == typeof(DateTime) || type == typeof(DateTime?))
      {
        if(value == null)
          return "NULL";

        DateTime? dt = value as DateTime?;
        if (dt != null)
          if (db != null)
            return db.ConstructDateTimeParam(dt.Value);
          else
            return string.Format("'{0}'", dt.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        else
          return "NULL";
      }

      return "NULL";
    }


    internal static string GetObjectQueryValue(this DirectDatabaseBase db, object obj)
    {
      if (obj == null)
        return "NULL";

      var type = obj.GetType();
      if (type == typeof(DirectTime))
        return db.CurrentDateQueryString;
      else if (type == typeof(DirectScopeID))
        return db.QueryScopeID;
      else if (type == typeof(int) || type == typeof(double) || type == typeof(long)
        || type == typeof(uint) || type == typeof(ulong) || type == typeof(short))
        return obj.ToString();
      else if (type == typeof(string) || type == typeof(String) || type == typeof(char))
        return string.Format("'{0}'", obj.ToString().EscapeString());
      else if (type == typeof(bool))
        return (bool)obj == true ? "1" : "0";
      else if (type == typeof(string[]) || type == typeof(List<string>))
      {
        string value = "";
        if (type == typeof(List<string>)) obj = ((List<string>)obj).ToArray(); // convert it first to array so we can easly work with it
        foreach (string a in (string[])obj)
          value += (!string.IsNullOrEmpty(value) ? "," : "") + string.Format("'{0}'", a.EscapeString());
        return value;
      }
      else if (type == typeof(int[]) || type == typeof(List<int>))
      {
        string value = "";
        if (type == typeof(List<int>)) obj = ((List<int>)obj).ToArray(); // convert it first to array so we can easly work with it
        foreach (int a in (int[])obj)
          value += (!string.IsNullOrEmpty(value) ? "," : "") + a;
        return value;
      }
      else if (type == typeof(DateTime))
      {
        DateTime? dt = obj as DateTime?;
        if (dt != null)
          return db.ConstructDateTimeParam(dt.Value);
        else
          return "NULL";
      }

      return "NULL";
    }


  }
}
