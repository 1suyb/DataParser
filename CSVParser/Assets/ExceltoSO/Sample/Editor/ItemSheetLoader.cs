using System.IO;
using UnityEditor;
using UnityEngine;
namespace SOLoader.FromExcel
{
     public class ItemSheetLoader : EditorWindow
     {
         private string excelPath = "Assets/ExceltoSO/Sample/TestExcel.xlsx";
         private string savePath;
         [MenuItem("Tools/ItemSheetLoader")]
         public static void ShowWindow()
         {
            EditorWindow.GetWindow<ItemSheetLoader>("ItemSheet");
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
             ItemSheetSO assets = ScriptableObject.CreateInstance<ItemSheetSO>();
             assets.Datas = ExcelDataLoader.Load<ItemSheet>(excelPath);
             string path = Path.Combine(savePath,"ItemSheetDB.asset");
             AssetDatabase.CreateAsset(assets,path);
             AssetDatabase.SaveAssets();
             AssetDatabase.Refresh();
             Debug.Log("ScriptableObject has been loaded!");
         }
     }
}
