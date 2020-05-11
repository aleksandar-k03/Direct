using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Direct.ModelsCreation
{

  public class DMGTableInfo
  {
    // Data 

    public string Schema { get; set; }
    public string TableName { get; set; }
    public List<DMGColumnInfo> Columns { get; set; } = new List<DMGColumnInfo>();


    // Fabrics

    private string CammelCase(string input)
      => input.First().ToString().ToUpper() + input.Substring(1);

    public string FileName { get => this.ClassName + "DM"; }
    public string ClassName
    {
      get
      {
        string[] split = this.TableName.Split('_');
        if (split.Length == 0)
          return this.CammelCase(split[0]);

        string result = "";
        foreach (string s in split)
          if (s.Length > 2)
            result += this.CammelCase(s);

        return result;
      }
    }
  }

  public class DMGColumnInfo
  {
    public string Name { get; set; }
    public bool IsPrimary { get; set; } // if it is first that is loaded
    public string TranslatedType { get; set; }
    public string Type { get; set; }
    public string Default { get; set; }
    public string Length { get; set; }
    public bool IsNullable { get; set; }
  }

}
