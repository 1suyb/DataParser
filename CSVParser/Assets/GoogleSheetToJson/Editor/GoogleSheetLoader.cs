using UnityEditor;
using UnityEngine;

namespace JsonLoader.FromGoogleSheet
{
    public class GoogleSheetLoader : EditorWindow
    {
        private string sheetId;
        private string apiKey;
        private string savePath;
        private string enumSheetName = "Enums";
        
        [MenuItem("Tools/JsonData From GoogleSheet")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<GoogleSheetLoader>("JsonData From GoogleSheet");
        }

        private void OnGUI()
        {
            GUILayout.Label("JsonData From GoogleSheet", EditorStyles.boldLabel);
			sheetId = EditorGUILayout.TextField("Sheet Id", sheetId);
			apiKey = EditorGUILayout.TextField("ApiKey", apiKey);
			savePath = EditorGUILayout.TextField("Save Path", savePath);
            enumSheetName = EditorGUILayout.TextField("enumSheetName", enumSheetName);
            if (GUILayout.Button("Make Scripts"))
            {
				GoogleSheetParser.Parse(sheetId, apiKey, enumSheetName, savePath);
            }
            
        }
    }
}

