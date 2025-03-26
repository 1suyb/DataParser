using UnityEngine;

public class PathData : ScriptableObject
{
    [HideInInspector]
    public string ExcelFileFolderpath = "Assets/Project/Data/ExcelData";
    [HideInInspector]
    public string JsonSavePath = "Assets/Resources/Data/Json";
    [HideInInspector]
    public string ClassSavePath = "Assets/Project/Scripts/DataScripts";
    [HideInInspector]
    public string EnumFileName = "InfoEnums";
}
