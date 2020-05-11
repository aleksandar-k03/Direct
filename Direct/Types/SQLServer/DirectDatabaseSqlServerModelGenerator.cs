using Direct.ModelsCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Direct.Types.SQLServer
{

  public class DirectDatabaseSqlServerModelGenerator : DMGenerator
  {

    public DirectDatabaseSqlServerModelGenerator(DirectDatabaseBase db) : base(db) { }

    protected override string[] IgnoreTables => new string[]
    {
       "sysdiagrams", "database_firewall_rules"
    };

    protected override string Query => 
@" SELECT
    TABLE_SCHEMA AS 'schema' ,
    TABLE_NAME AS 'table_name',
    COLUMN_NAME AS 'column_name',
    ORDINAL_POSITION AS 'column_position',
    COLUMN_DEFAULT as 'column_default' ,
    DATA_TYPE AS 'type' ,
    CHARACTER_MAXIMUM_LENGTH AS 'length',
    IS_NULLABLE AS 'is_nullable'
FROM  INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA != 'sys'
ORDER BY TABLE_NAME, ORDINAL_POSITION;";

    protected override string GetColumnType(string input, bool isNullable)
    {
      string result;
      switch (input.ToLower())
      {

        case "smallint":
        case "mediumint":
        case "bigint":
        case "int":
          result = "int";
          break;

        case "tinyint":
        case "bit":
          result = "bool";
          break;

        case "uniqueidentifier":
          result = "Guid";
          break;

        case "nchar":
        case "char":
          result = "char";
          break;

        case "float":
        case "decimal":
          result =  "double";
          break;

        case "longtext":
        case "text":
        case "ntext":
        case "varchar":
        case "nvarchar": 
          result = "string";
          isNullable = false;
          break;

        case "timestamp":
        case "datetime":
        case "datetime2": 
          result = "DateTime";
          break;

        case "varbinary": 
          result = "byte[]";
          break;

        default: 
          result ="[unknown]";
          break;
      }

      result += (isNullable ? "?" : "");
      return result;
    }
  }
}
