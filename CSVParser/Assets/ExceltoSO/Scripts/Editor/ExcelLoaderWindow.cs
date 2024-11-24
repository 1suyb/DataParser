using UnityEditor;
using UnityEngine;

namespace SOLoader.FromExcel
{
    public class ExcelLoaderWindow : EditorWindow
    {
        private string excelPath;
        private string savePath;
        private string enumSheetName = "Enums";
        
        [MenuItem("Tools/SOLoader From Excel")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<ExcelLoaderWindow>("SOLoader From Excel");
        }

        private void OnGUI()
        {
            GUILayout.Label("SOLoader From Excel",EditorStyles.boldLabel);
            excelPath = EditorGUILayout.TextField("Excel Path", excelPath);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            enumSheetName = EditorGUILayout.TextField("enumSheetName", enumSheetName);
            if (GUILayout.Button("Make Scripts"))
            {
                ExcelParser.Parse(excelPath, enumSheetName, savePath);
            }
            
        }
    }
}

