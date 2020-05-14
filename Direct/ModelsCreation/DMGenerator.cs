using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Direct.ModelsCreation
{
  /*

    Shared:
      [date]
      [database_name]

    Base
      [database_type]
      [direct_manager]

    Table Model Class:
      [table_class_name]
      [table_id]
      [table_name]
		  [table_content]
      [table_schema]

    Column:
      [dcolumn]
      [column_type]
      [column_name]
      [column_value]

   */
  public abstract class DMGenerator
  {
    protected DirectDatabaseBase Database = null;
    protected string DatabaseFilteredName = string.Empty;
    protected abstract string[] IgnoreTables { get; }
    protected abstract string Query { get; }

    public DMGenerator(DirectDatabaseBase db)
    {
      this.Database = db;
    }

    /// ABSTRACTS
    /// 

    protected abstract string GetColumnType(string input, bool isNullable);


    /// PUBLICS
    /// 

    public void SaveTo(string folderPath)
    {
      if (!Directory.Exists(folderPath))
        throw new Exception("Directory does not exists!");

      string modelsDir = folderPath + @"/models";
      string definitionsDir = folderPath + @"/definitions";
      if (!Directory.Exists(modelsDir)) Directory.CreateDirectory(modelsDir);
      if (!Directory.Exists(definitionsDir)) Directory.CreateDirectory(definitionsDir);
      modelsDir += @"/"; definitionsDir += @"/";

      this.DatabaseFilteredName = this.Database.DatabaseName.First().ToString().ToUpper() + this.Database.DatabaseName.Substring(1);

      Console.WriteLine("Starting to generate data for database " + this.Database.DatabaseName);
      var data = this.GetTables();
      Console.WriteLine("Data Loaded");

      File.WriteAllText(modelsDir + "ModelBase.cs", this.ConstructBaseClass());
      foreach(var table in data)
      {
        File.WriteAllText(modelsDir + table.FileName + ".cs", this.ConstructTableClass(table));
        File.WriteAllText(definitionsDir + table.FileName + ".cs", this.ConstructDefinitionTableClass(table));
        Console.WriteLine("Created " + table.ClassName);
      }
    }

    /// SHARED AND ATTRUTES
    /// 

    protected string GetTemplateContent(string name)
    {
      string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + 
        string.Format(@"\ModelsCreation\Template.{0}.txt", name);
      return File.ReadAllText(path);
    }


    /// PRIVATES
    /// 

    public virtual List<DMGTableInfo> GetTables()
    {
      List<DMGTableInfo> result = new List<DMGTableInfo>();
      var data = this.Database.Load<DMGInitialSqlResponse>(this.Query);

      foreach (var row in data)
      {
        if (this.IgnoreTables.Contains<string>(row.table_name))
          continue;

        var table = (from r in result where r.TableName.Equals(row.table_name) select r).FirstOrDefault();
        if (table == null)
        {
          table = new DMGTableInfo() { Schema = row.schema, TableName = row.table_name };
          result.Add(table);
        }

        var column = new DMGColumnInfo()
        {
          Name = row.column_name,
          Default = row.column_default,
          Length = row.length,
          IsPrimary = table.Columns.Count == 0,
          Type = row.type,
          IsNullable = row.is_nullable != null && row.is_nullable.Equals("YES")
        };
        column.TranslatedType = this.GetColumnType(column.Type, column.IsNullable);

        table.Columns.Add(column);
      }

      return result;
    }


    // SUMMARY: Base class generation
    private string ConstructBaseClass()
    {
      string data = this.FillSharedData(this.GetTemplateContent("Base"));
      data = data.Replace("[database_type]", this.Database.DatabaseType.ToString())
        .Replace("[direct_manager]", this.Database.GetType().FullName);
      return data;
    }

    // SUMMARY: Create Model class
    private string ConstructTableClass(DMGTableInfo table)
    {
      string data = this.FillSharedData(this.GetTemplateContent("Model"));
      data = data.Replace("[table_name]", table.TableName)
        .Replace("[table_class_name]", table.ClassName)
        .Replace("[table_id]", table.Columns[0].Name)
        .Replace("[table_schema]", table.Schema);

      string columnData = "";
      foreach(var column in table.Columns)
      {
        string columnTemplate = this.GetTemplateContent("Column")
          .Replace("[column_name]", column.Name)
          .Replace("[column_type]", this.GetColumnType(column.Type, column.IsNullable))
          .Replace("[column_value]", this.GetDefaultValue(column))
          .Replace("[dcolumn]", this.GetDColumnData(column));

        columnData += columnTemplate;
      }

      return data.Replace("[table_content]", columnData); ;
    }

    // SUMMARY: Create data for definition class
    private string ConstructDefinitionTableClass(DMGTableInfo table)
    {
      string data = this.FillSharedData(this.GetTemplateContent("Definition"));
      data = data.Replace("[table_class_name]", table.ClassName);
      return data;

    }

    // SUMMARY: Fill data that are same on all template files
    private string FillSharedData(string input)
      => input.Replace("[date]", DateTime.Now.ToString())
        .Replace("[database_name]", this.DatabaseFilteredName);

    // SUMMARY: Reconstruct default value
    private string GetDefaultValue(DMGColumnInfo column)
    {
      if (string.IsNullOrEmpty(column.Default))
      {
        if (column.TranslatedType.Contains("?"))
          return "null";
        else
          return "default";
      }

      string data = column.Default.Replace("(", string.Empty).Replace(")", string.Empty);

      if (column.TranslatedType.Equals("string"))
        return string.Format("\"{0}\"", data);

      if (column.TranslatedType.Equals("Guid"))
        return "Guid.NewGuid()";

      if (column.TranslatedType.Equals("DateTime") || column.TranslatedType.Equals("DateTime?"))
        return "DateTime.Now";

      if (column.TranslatedType.Equals("bool") || column.TranslatedType.Equals("bool?"))
        return data.Equals("1") ? "true" : "false";

      return data;
    }

    // SUMMARY: Construct DMColumn data
    private string GetDColumnData(DMGColumnInfo column)
    {
      string result = "";
      if (column.IsPrimary)
        result += ", IsPrimary=true";
      if (!string.IsNullOrEmpty(column.Default))
        result += ", HasDefaultValue=true";
      if (column.IsNullable)
        result += ", Nullable=true";

      if(column.TranslatedType.Equals("DateTime"))
      {
        if(column.Name.ToLower().Equals("updated"))
          result += ", DateTimeUpdate=true, NotUpdatable=true";
        else if(column.Name.ToLower().Equals("created"))
          result += ", NotUpdatable=true";
        else if(!string.IsNullOrEmpty(column.Default))
          result += ", NotUpdatable=true";
      }
      //if (column.TranslatedType.Equals("DateTime") && !string.IsNullOrEmpty(column.Default))
      //  result += ", DateTimeUpdate=true, NotUpdatable=true";

      return result;
    }

  }
}
