using System;
using System.Reflection;

namespace Direct.Helpers
{
  public static class DirectCastHelper
  {

    public static object ChangeType(object value, Type conversion)
    {
      var t = conversion;

      if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
      {
        if (value == null)
        {
          return null;
        }

        t = Nullable.GetUnderlyingType(t);
      }

      return Convert.ChangeType(value, t);
    }

    public static void ConvertProperty<T>(T temp, PropertyInfo property, string value)
    {
      string typename = property.PropertyType.Name;
      try
      {
        switch (typename.ToLower())
        {
          case "string":
            property.SetValue(temp, value);
            break;
          case "int32":
            int intResult;
            if (int.TryParse(value, out intResult))
              property.SetValue(temp, intResult);
            break;
          case "datetime":
            DateTime dateTimeResult;
            if (DateTime.TryParse(value, out dateTimeResult))
              property.SetValue(temp, dateTimeResult);
            break;
          case "double":
            double doubleresult;
            if (double.TryParse(value, out doubleresult))
              property.SetValue(temp, doubleresult);
            break;
          case "boolean":
            if (value.Equals("1")) property.SetValue(temp, true);
            if (value.Equals("0")) property.SetValue(temp, false);
            break;
          case "long":
            long longresult;
            if (long.TryParse(value, out longresult))
              property.SetValue(temp, longresult);
            break;

          case "nullable`1":
            if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.Double"))
            {
              double doubleResult1;
              if (double.TryParse(value, out doubleResult1))
                property.SetValue(temp, doubleResult1);
              else
                property.SetValue(temp, null);
              break;
            }
            else if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.Int32"))
            {
              int intResult1;
              if (int.TryParse(value, out intResult1))
                property.SetValue(temp, intResult1);
              else
                property.SetValue(temp, null);
              break;
            }
            else if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.DateTime"))
            {
              DateTime dateTimeNullResult;
              if (DateTime.TryParse(value, out dateTimeNullResult))
                property.SetValue(temp, dateTimeNullResult);
              else
                property.SetValue(temp, null);
              break;
            }
            else if (property.PropertyType.FullName.StartsWith("System.Nullable`1[[System.Int64,"))
            {
              long longResult1;
              if (long.TryParse(value, out longResult1))
                property.SetValue(temp, longResult1);
              else
                property.SetValue(temp, null);
              break;
            }
            break;

          default:
            break;
        }
      }
      catch (Exception e)
      {
        int a = 0;
      }
    }

  }
}
