using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ExceltoJson
{
    public class ExcelToJsonEditorWindow : EditorWindow
    {
        private static PathData _pathData;
        private string _excelFileFolderPath => _pathData.ExcelFileFolderpath;
        private string _jsonSavePath => _pathData.JsonSavePath;
        private string _classSavePath=> _pathData.ClassSavePath;
        private string _enumFileName => _pathData.EnumFileName;
        
        private Dictionary<string, bool> fileToggles = new Dictionary<string, bool>();
        public List<string> selectedFiles = new List<string>();
    
        private Vector2 scrollPosition;
        
        [MenuItem("Tools/ExcelToJson")]
        public static void ShowWindow() {
            EditorWindow.GetWindow<ExcelToJsonEditorWindow>("Excel To Json");
            if (_pathData == null)
            {
                if(File.Exists("Assets/Plugins/ExcelToJson/PathData.asset"))
                {
                    _pathData = AssetDatabase.LoadAssetAtPath<PathData>("Assets/Plugins/ExcelToJson/PathData.asset");
                }
                else
                {
                    _pathData = ScriptableObject.CreateInstance<PathData>();
                    AssetDatabase.CreateAsset(_pathData, "Assets/Plugins/ExcelToJson/PathData.asset");
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Path Setting", EditorStyles.boldLabel);
            _pathData.ExcelFileFolderpath = EditorGUILayout.TextField("Excel File Folder Path:", _pathData.ExcelFileFolderpath);
            _pathData.JsonSavePath = EditorGUILayout.TextField("Json Save Path:", _pathData.JsonSavePath);
            _pathData.ClassSavePath = EditorGUILayout.TextField("Class Save Path:", _pathData.ClassSavePath);
            _pathData.EnumFileName = EditorGUILayout.TextField("Enum File Name:", _pathData.EnumFileName);
            
            ExcelToJson converter = new ExcelToJson(_excelFileFolderPath, _jsonSavePath, _classSavePath, _enumFileName);
            if (GUILayout.Button("CreateJson"))
            {
                converter.CreateJson(selectedFiles);
            }

            if (GUILayout.Button("CreateClass"))
            {
                converter.CreateClass(selectedFiles);
            }

            if (GUILayout.Button("All Convert"))
            {
                converter.Convert();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Current Status:");
            GUILayout.Label($"Excel File Folder Path: {_excelFileFolderPath}");
            GUILayout.Label($"Json Save Path: {_jsonSavePath}");
            GUILayout.Label($"Class Save Path: {_classSavePath}");
            GUILayout.Label($"Enum File Name: {_enumFileName}");

            if (GUILayout.Button("Load Files"))
            {
                LoadFiles(_excelFileFolderPath);
            }

            // 파일 목록 표시
            if (fileToggles.Count > 0)
            {
                EditorGUILayout.LabelField("Files in Folder:", EditorStyles.boldLabel);

                // 스크롤 영역 추가
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

                // Dictionary를 변경하지 않기 위해 복사본 사용
                var keys = new List<string>(fileToggles.Keys);

                foreach (var file in keys)
                {
                    if (file.Contains("meta"))
                    {
                        continue;
                    }

                    // 가로 레이아웃 시작
                    GUILayout.BeginHorizontal();

                    // 파일 경로 라벨 (전체 표시 가능)
                    EditorGUILayout.LabelField(file, GUILayout.ExpandWidth(true));

                    // 토글 버튼
                    bool isSelected = fileToggles[file];
                    bool newToggleState = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20)); // 토글 버튼 고정 너비

                    // 상태 변경 처리
                    if (newToggleState != isSelected)
                    {
                        fileToggles[file] = newToggleState;

                        if (newToggleState)
                        {
                            if (!selectedFiles.Contains(file))
                            {
                                selectedFiles.Add(file);
                            }
                        }
                        else
                        {
                            selectedFiles.Remove(file);
                        }
                    }

                    // 가로 레이아웃 종료
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }


            if (selectedFiles.Count > 0)
            {
                EditorGUILayout.LabelField("Selected Files:", EditorStyles.boldLabel);

                foreach (string file in selectedFiles)
                {
                    EditorGUILayout.LabelField(file);
                }
            }

        }

        private void LoadFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);

                fileToggles.Clear();
                foreach (string file in files)
                {
                    if (!fileToggles.ContainsKey(file))
                    {
                        fileToggles[file] = false; // 초기 상태는 선택되지 않음
                    }
                }
            }
            else
            {
                Debug.LogWarning("Invalid folder path!");
            }
        }
    }
    
    

}
