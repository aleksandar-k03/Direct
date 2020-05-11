using Direct.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Web
{
  internal static class DirectWebControllerHelper
  {
    public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null)
    {
      if (encoding == null)
        encoding = Encoding.UTF8;

      using (StreamReader reader = new StreamReader(request.Body, encoding))
        return await reader.ReadToEndAsync();
    }

    public static void UpdateValue(this DirectModelPropertySignature snap, DirectModel model, JToken value)
    {
      if (snap.PropertyInfo.PropertyType == typeof(string))
        snap.PropertyInfo.SetValue(model, value.ToString());

      else if (snap.PropertyInfo.PropertyType == typeof(int) ||
        snap.PropertyInfo.PropertyType == typeof(int?))
        snap.PropertyInfo.SetValue(model, (int)value);

      else if (snap.PropertyInfo.PropertyType == typeof(uint) ||
        snap.PropertyInfo.PropertyType == typeof(uint?))
        snap.PropertyInfo.SetValue(model, (uint)value);

      else if (snap.PropertyInfo.PropertyType == typeof(DateTime) ||
        snap.PropertyInfo.PropertyType == typeof(DateTime?))
        snap.PropertyInfo.SetValue(model, (DateTime)value);

      else if (snap.PropertyInfo.PropertyType == typeof(double) ||
        snap.PropertyInfo.PropertyType == typeof(double?))
        snap.PropertyInfo.SetValue(model, (double)value);

      else if (snap.PropertyInfo.PropertyType == typeof(bool) ||
        snap.PropertyInfo.PropertyType == typeof(bool?))
        snap.PropertyInfo.SetValue(model, (bool)value);
    }

    public static async Task<JObject> GetPostData(this HttpRequest request)
    {
      string rawPostData = await request.GetRawBodyStringAsync();
      if (string.IsNullOrEmpty(rawPostData))
        return null;

      return JsonConvert.DeserializeObject<JObject>(rawPostData);
    }


  }
}
