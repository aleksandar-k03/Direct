using Direct.ModelsCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Direct.Types.Mysql
{
  public class DirectDatabaseMysqlModelGenerator : DMGenerator
  {

    public DirectDatabaseMysqlModelGenerator(DirectDatabaseBase db) : base(db) { }
    protected override string[] IgnoreTables => new string[]
    {
       
    };

    protected override string Query =>
      @"SELECT
    '' AS 'schema',
    TABLE_NAME AS 'table_name',
    COLUMN_NAME AS 'column_name',
    DATA_TYPE AS 'type',
    CHARACTER_MAXIMUM_LENGTH AS 'length',
    COLUMN_DEFAULT AS 'column_default',
    IS_NULLABLE AS 'is_nullable'
FROM INFORMATION_SCHEMA.COLUMNS
where TABLE_SCHEMA != 'information_schema' AND TABLE_SCHEMA != 'mysql' AND TABLE_SCHEMA != 'performance_schema' AND TABLE_SCHEMA != 'sys'";

    protected override string GetColumnType(string type, bool isNullable)
    {
      string result = "";
      switch (type)
      {
        case "smallint":
        case "int":
          result = "int";
          break;
        case "bigint":
          result = "uint";
          break;
        case "longtext":
        case "varchar":
        case "text":
          result = "string";
          isNullable = false;
          break;
        case "timestamp":
          result = "DateTime";
          break;
        case "double":
        case "decimal":
          result = "double";
          break;
        case "tinyint":
          result = "bool";
          break;
        case "binary":
        case "varbinary":
        case "mediumblob":
          result = "byte[]";
          return result;
          break;
      }

      result += (isNullable ? "?" : "");
      return result;
    }
  }
}
