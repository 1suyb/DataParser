using System;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace SOLoader.FromExcel
{
    
    public class WrongExcel : Exception
    {
        public WrongExcel(string message) : base(message) { }
    }
    
    public class ExcelParser
    {
        private static string qualifier = "public";
        public static void Parse(string filePath, string enumTableName = "Enums",  string savePath = "Assets")
        {
            EnsureFolderExists(savePath);
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();

                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        if (result.Tables[i].TableName == enumTableName)
                        {
                            WriteEnum(result.Tables[i], savePath);
                        }
                        else
                        {
                            WriteDataClass(result.Tables[i], savePath);
                            WriteSOClass(result.Tables[i], savePath);
                            WriteLoader(result.Tables[i].TableName, savePath, filePath);
                        }
                    }
                }
            }
            Debug.Log($"{filePath} scripts have been created!");
        }
        
        public static void WriteDataClass(DataTable sheet, string savePath)
        {
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
            sb.AppendLine("[Serializable]");
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");
            for (int i = 0; i < length; i++)
            {
                sb.AppendLine($"     {qualifier} {fieldType.ItemArray[i]} {fieldName.ItemArray[i]};");
            }
            sb.AppendLine("}");
            savePath = Path.Combine(savePath, $"{className}.cs");
            File.WriteAllText(savePath,sb.ToString());
        }

        public static void WriteSOClass(DataTable sheet, string savePath)
        {
            string className = sheet.TableName;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("");
            sb.AppendLine("[CreateAssetMenu(fileName = \"TestSO\", menuName = \"Datas/TestSO\")]");
            sb.AppendLine($"public class {className}SO : ScriptableObject");
            sb.AppendLine("{");
            sb.AppendLine($"    public List<{className}> Datas = new List<{className}>();");
            sb.AppendLine("}");
            savePath = Path.Combine(savePath, $"{className}SO.cs");
            File.WriteAllText(savePath, sb.ToString());
        }

        public static void WriteEnum(DataTable sheet,string savePath)
        {
            string fileName = $"{sheet.TableName}.cs";
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
            savePath = Path.Combine(savePath, fileName);
            File.WriteAllText(savePath,sb.ToString());
        }

        public static void WriteLoader(string className, string savePath, string excelPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using UnityEditor;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("namespace SOLoader.FromExcel");
            sb.AppendLine("{");
            sb.AppendLine($"     public class {className}Loader : EditorWindow");
            sb.AppendLine("     {");
            sb.AppendLine($"         private string excelPath = \"{excelPath}\";");
            sb.AppendLine("         private string savePath;");
            sb.AppendLine($"         [MenuItem(\"Tools/{className}Loader\")]");
            sb.AppendLine("         public static void ShowWindow()");
            sb.AppendLine("         {");
            sb.AppendLine($"            EditorWindow.GetWindow<{className}Loader>(\"{className}\");");
            sb.AppendLine("         }");
            sb.AppendLine("");
            sb.AppendLine("         private void OnGUI()");
            sb.AppendLine("         {");
            sb.AppendLine("             GUILayout.Label(\"PathSetting\",EditorStyles.boldLabel);");
            sb.AppendLine("             excelPath = EditorGUILayout.TextField(\"ExcelPath :\",excelPath);");
            sb.AppendLine("             savePath = EditorGUILayout.TextField(\"SavePath :\",savePath);");
            sb.AppendLine("             if(GUILayout.Button(\"Make Object\"))");
            sb.AppendLine("             {");
            sb.AppendLine($"                     Generate();");
            sb.AppendLine("             }");
            sb.AppendLine("         }");
            sb.AppendLine("");
            sb.AppendLine("         private void Generate()");
            sb.AppendLine("         {");
            sb.AppendLine($"             {className}SO assets = ScriptableObject.CreateInstance<{className}SO>();");
            sb.AppendLine($"             assets.Datas = ExcelDataLoader.Load<{className}>(excelPath);");
            sb.AppendLine($"             string path = Path.Combine(savePath,\"{className}DB.asset\");");
            sb.AppendLine("             AssetDatabase.CreateAsset(assets,path);");
            sb.AppendLine("             AssetDatabase.SaveAssets();");
            sb.AppendLine("             AssetDatabase.Refresh();");
            sb.AppendLine("             Debug.Log(\"ScriptableObject has been loaded!\");");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("}");
            
            savePath = Path.Combine(savePath, $"{className}Loader.cs");
            File.WriteAllText(savePath,sb.ToString());
        }
        private static void EnsureFolderExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

    }

}
