using System.IO;
using UnityEditor;
using UnityEngine;
namespace SOLoader.FromExcel
{
     public class MonsterSheetLoader : EditorWindow
     {
         private string excelPath = "Assets/ExceltoSO/Sample/TestExcel.xlsx";
         private string savePath;
         [MenuItem("Tools/MonsterSheetLoader")]
         public static void ShowWindow()
         {
            EditorWindow.GetWindow<MonsterSheetLoader>("MonsterSheet");
         }

         private void OnGUI()
         {
             GUILayout.Label("PathSetting",EditorStyles.boldLabel);
             excelPath = EditorGUILayout.TextField("ExcelPath :",excelPath);
             savePath = EditorGUILayout.TextField("SavePath :",savePath);
             if(GUILayout.Button("Make Object"))
             {
                     Generate();
             }
         }

         private void Generate()
         {
             MonsterSheetSO assets = ScriptableObject.CreateInstance<MonsterSheetSO>();
             assets.Datas = ExcelDataLoader.Load<MonsterSheet>(excelPath);
             string path = Path.Combine(savePath,"MonsterSheetDB.asset");
             AssetDatabase.CreateAsset(assets,path);
             AssetDatabase.SaveAssets();
             AssetDatabase.Refresh();
             Debug.Log("ScriptableObject has been loaded!");
         }
     }
}
