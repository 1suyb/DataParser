using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestSO", menuName = "Datas/TestSO")]
public class ItemSheetSO : ScriptableObject
{
    public List<ItemSheet> Datas = new List<ItemSheet>();
}
