using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Direct
{
  public static class DirectBinary
  {

    public static byte[] Serialize(object data)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        BinaryFormatter bw = new BinaryFormatter();
        bw.Serialize(ms, data);
        return ms.ToArray();
      }
    }

    public static T Deserialize<T>(this byte[] param)
    {
      if (param == null)
        return default(T);

      bool hasAllZeroes = param.All(singleByte => singleByte == 0);
      if (hasAllZeroes)
        return default(T);

      try
      {
        T obj = default(T);
        using (MemoryStream ms = new MemoryStream(param))
        {
          IFormatter br = new BinaryFormatter();
          obj = (T)br.Deserialize(ms);
        }

        return obj;
      }
      catch (Exception e)
      {
        return default(T);
      }
    }


  }
}
