using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestSO", menuName = "Datas/TestSO")]
public class MonsterSheetSO : ScriptableObject
{
    public List<MonsterSheet> Datas = new List<MonsterSheet>();
}
