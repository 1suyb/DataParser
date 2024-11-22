using UnityEngine;
using UnityEditor;
using CSVtoSO;

[CreateAssetMenu(fileName = "ItemDataLoader",menuName = "DataScriptableObject/ItemDataLoader")]
public class ItemDataCSVDataLoader: ScriptableObject
{
     [SerializeField] private TextAsset _textAsset;
     [SerializeField] private string _path;

     [ContextMenu("Generate")]
     public void Generate()
     {
	    ItemDataDB assets = ScriptableObject.CreateInstance<ItemDataDB>();
	    assets.Data = CSVDataLoader<ItemData>.LoadData(_textAsset.text);
		string path = $"{_path}/ItemDataDB.asset";
		    path = AssetDatabase.GenerateUniqueAssetPath(path);
		    AssetDatabase.CreateAsset(assets, path);
		    AssetDatabase.SaveAssets();
		    AssetDatabase.Refresh();
     }
}
