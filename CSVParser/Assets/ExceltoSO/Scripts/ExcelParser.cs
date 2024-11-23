using System;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace ScriatableObjectBuilderFromExcel
{
    
    public class ExcelParser
    {
        private static string Qualifier = "public";
        public static void Parse(string filePath, string enumTableName = "Enums",  string savePath = "Assets")
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();

                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        if (result.Tables[i].TableName == enumTableName)
                        {
                            WriteEnum(result.Tables[i]);
                        }
                        else
                        {
                            WriteDataClass(result.Tables[i]);
                        }
                    }
                }
            }
        }

        public class WrongExcel : Exception
        {
            public WrongExcel(string message) : base(message) { }
        }
        public static void WriteDataClass(DataTable sheet)
        {
            Debug.Log(sheet.TableName);
            string className = sheet.TableName;
            DataRow fieldName = sheet.Rows[0];
            DataRow fieldType = sheet.Rows[1];
            if (fieldName.ItemArray.Length != fieldType.ItemArray.Length)
            {
                throw new WrongExcel("라인1과 라인2의 길이가 맞지 않습니다.");
            }
            int length = fieldName.ItemArray.Length;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("");
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");
            for (int i = 0; i < length; i++)
            {
                sb.AppendLine($"     {Qualifier} {fieldType.ItemArray[i]} {fieldName.ItemArray[i]};");
            }
            sb.AppendLine("}");
            File.WriteAllText("Assets/Test.cs",sb.ToString());
            Debug.Log(sb.ToString());
        }

        public static void WriteEnum(DataTable sheet)
        {
            Debug.Log("Im ExcelParser to Enum");
            Debug.Log(sheet.TableName);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sheet.Rows.Count; i++)
            {
                sb.AppendLine($"public enum {sheet.Rows[i].ItemArray[0]}");
                sb.AppendLine("{");
                for (int j = 1; j < sheet.Rows[i].ItemArray.Length; j++)
                {
                    sb.AppendLine($"     {sheet.Rows[i].ItemArray[j]},");
                }
                sb.AppendLine("}");
            }
            Debug.Log(sb.ToString());
            File.WriteAllText("Assets/TestEnums.cs",sb.ToString());
        }

        public static void WriteLoader()
        {
            
        }
    }

}
