using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemSheet
{
     public int ID;
     public string Name;
     public string Description;
     public ItemType ItemType;
     public float Attack;
     public float Defense;
     public float Speed;
     public List<int> Things;
}
