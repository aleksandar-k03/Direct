using Direct.Results;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Direct.Models;

namespace Direct
{
  public static partial class DirectModelHelper
  {

    public static DirectQueryLoader<T> Query<T>(this DirectDatabaseBase db) where T : DirectModel
      => new DirectQueryLoader<T>() { Database = db };

    public static DirectQueryLoader<T> Where<T>(this DirectQueryLoader<T> loader, string input) where T : DirectModel
    {
      if (input.Contains("[id]"))
        using (var tempValue = (T)Activator.CreateInstance(typeof(T), (DirectDatabaseBase)loader.Database))
          input = input.Replace("[id]", tempValue.IdName);

      loader.Where = input;
      return loader;
    }
    public static DirectQueryLoader<T> Where<T>(this DirectQueryLoader<T> loader, string input, params object[] parameters) where T : DirectModel
    {
      if (input.Contains("[id]"))
        using (var tempValue = (T)Activator.CreateInstance(typeof(T), (DirectDatabaseBase)loader.Database))
          input = input.Replace("[id]", tempValue.IdName);

      loader.SetWhere(input, parameters);
      return loader;
    }
    public static DirectQueryLoader<T> Select<T>(this DirectQueryLoader<T> loader, string input) where T : DirectModel
    {
      input = input.Trim();
      using (var tempValue = (T)Activator.CreateInstance(typeof(T), (DirectDatabaseBase)loader.Database))
      {
        if (input.Equals("*"))
          return loader;
        if (loader.Select.Equals("id"))
          loader.Select = tempValue.IdName;
        else if (!string.IsNullOrEmpty(input))
        {
          if (input.Contains("[id]"))
              input = input.Replace("[id]", tempValue.IdName);
          loader.Select = tempValue.IdName + "," + input;
        }
        else
          loader.Select = "*";
      }
      return loader;
    }
    public static DirectQueryLoader<T> Additional<T>(this DirectQueryLoader<T> loader, string input) where T : DirectModel
    {
      loader.Additional = input;
      return loader;
    }


  }
}
