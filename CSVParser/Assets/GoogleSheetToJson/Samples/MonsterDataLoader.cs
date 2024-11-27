using System;
using System.Collections.Generic;
using UnityEngine;
public class MonsterDataLoader
{
		private List<MonsterData> dataList;
		private Dictionary<int,MonsterData> dataDict;

		public MonsterDataLoader(string path)
		{
			string jsonData;
			jsonData = Resources.Load<TextAsset>(path).text;
			dataList = JsonUtility.FromJson<Wrapper>(jsonData).datas;
			dataDict = new Dictionary<int,MonsterData>();
			foreach(var item in dataList)
			{
				dataDict.Add(item.ID,item);
			}
		}
		private class Wrapper
		{
			public List<MonsterData> datas;
		}

		public MonsterData Get(int id)
		{
			if(dataDict.ContainsKey(id))
			{
				return dataDict[id];
			}
			return null;
		}

		public List<MonsterData> GetAllData()
		{
			return dataList;
		}

}
