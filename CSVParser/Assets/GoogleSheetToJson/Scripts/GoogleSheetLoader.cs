using SOLoader.FromExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine;
using Palmmedia.ReportGenerator.Core.Common;
using System.Collections;
using static UnityEditor.Progress;


namespace JsonLoader.FromGoogleSheet
{
	public class GoogleSheetParser
	{
		private string sheetNameJson;
		private string SheetDataJson;
		private static Dictionary<string, Dictionary<string, int>> enumMappings;
		public static async void Parse(string sheetID, string apiKey, string enumTableName = "Enums", string savePath = "Assets")
		{
			List<string> sheetNames = await GetSheetNames(sheetID, apiKey);
			StringBuilder sb = new StringBuilder();
			List<List<List<string>>> sheetDatas = new List<List<List<string>>>();
			for (int i = 0; i < sheetNames.Count; i++)
			{
				sheetDatas.Add(await GetSheetDatas(sheetID, sheetNames[i], apiKey));
				if (sheetNames[i] == enumTableName)
				{
					WriteEnum(sheetNames[i], sheetDatas[i], savePath);
				}
				else
				{
					WriteDataClass(sheetNames[i], sheetDatas[i], savePath);
					WriteLoaderClass(sheetNames[i],sheetDatas[i], savePath);
				}
			}
			for(int i = 0; i< sheetNames.Count;i++)
			{
				if (sheetNames[i] != enumTableName)
				{
					WriteJson(sheetNames[i], sheetDatas[i], savePath);
				}
			}
			Debug.Log($" scripts have been created in {savePath}!");
		}
		public static async Task<List<string>> GetSheetNames(string sheetID, string apiKey)
		{
			string url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetID}?fields=sheets.properties.title&key={apiKey}";
			string result = await Connect(url);
			GoogleSpreadsheetInfo sheetNameInfo = JsonUtility.FromJson<GoogleSpreadsheetInfo>(result);
			List<string> sheetNames = new List<string>();
			foreach (var sheetName in sheetNameInfo.sheets)
			{
				sheetNames.Add(sheetName.properties.title);
			}
			return sheetNames;
		}
		public static async Task<List<List<string>>> GetSheetDatas(string sheetID, string sheetName, string apiKey)
		{
			string range = $"{sheetName}!A1:Z1000"; // Adjust the range as needed
			string url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetID}/values/{range}?key={apiKey}";

			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				string responseJson = Regex.Unescape(await response.Content.ReadAsStringAsync());
				Debug.Log(responseJson);
				List<List<string>> values = ParseValues(responseJson);
				if (values == null)
				{
					values = new List<List<string>>();
				}

				return values ?? new List<List<string>>();
			}
		}
		public static async Task<string> Connect(string url)
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				string responseJson = await response.Content.ReadAsStringAsync();
				return responseJson;
			}
		}
		private static List<List<string>> ParseValues(string json)
		{
			var values = new List<List<string>>();

			// "values": 부분 찾기
			int valuesStart = json.IndexOf("\"values\": [");
			if (valuesStart == -1)
			{
				return null;
			}
			json = json.Replace(" ", "")
							.Replace("\t", "")
							.Replace("\n", "");


			// values 데이터만 추출
			//int arrayStart = json.IndexOf('[', valuesStart);
			int arrayStart = valuesStart;
			int arrayEnd = json.LastIndexOf(']');
			if (arrayStart == -1 || arrayEnd == -1)
			{
				return null;
			}

			string valuesArray = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);

			// 각 행을 분리
			string[] rows = valuesArray.Split(new[] { "],[" }, StringSplitOptions.None);

			foreach (var row in rows)
			{
				// 행의 양 끝 대괄호 제거 및 항목 분리
				string cleanRow = row.Trim('[', ']');
				string[] items = cleanRow.Split(new[] { "\",\"" }, StringSplitOptions.None);

				// 각 항목의 따옴표 제거
				var parsedRow = new List<string>();
				foreach (var item in items)
				{
					parsedRow.Add(item.Trim('"'));
				}

				values.Add(parsedRow);
			}

			return values;
		}

		private static void WriteDataClass(string sheetName, List<List<string>> sheetDatas, string savePath)
		{
			string className = sheetName;
			List<string> fieldName = sheetDatas[0];
			List<string> fieldType = sheetDatas[1];
			if (fieldName.Count != fieldType.Count)
			{
				throw new WrongExcel("라인1과 라인2의 길이가 맞지 않습니다.");
			}
			int length = fieldName.Count;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("[Serializable]");
			sb.AppendLine($"public class {className}");
			sb.AppendLine("{");
			for (int i = 0; i < length; i++)
			{
				string type = fieldType[i];
				if (fieldType[i].StartsWith("Enum<")) {
					type = type.Substring(5, fieldType[i].Length - 6);
				}
				if (fieldType[i].StartsWith("List<Enum<"))
				{
					type = type.Substring(10, fieldType[i].Length - 12);
				}
				sb.AppendLine($"     public {type} {fieldName[i]};");
			}
			sb.AppendLine("}");
			savePath = Path.Combine(savePath, $"{className}.cs");
			File.WriteAllText(savePath, sb.ToString());
		}
		public static void WriteLoaderClass(string sheetName, List<List<string>> sheetDatas, string savePath)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityEngine;");

			sb.AppendLine($"public class {sheetName}Loader");
			sb.AppendLine("{");
			sb.AppendLine($"		private List<{sheetName}> dataList;");
			sb.AppendLine($"		private Dictionary<int,{sheetName}> dataDict;");
			sb.AppendLine();
			sb.AppendLine($"		public {sheetName}Loader(string path)");
			sb.AppendLine("		{");
			sb.AppendLine("			string jsonData;");
			sb.AppendLine("			jsonData = Resources.Load<TextAsset>(path).text;");
			sb.AppendLine("			dataList = JsonUtility.FromJson<Wrapper>(jsonData).datas;");
			sb.AppendLine($"			dataDict = new Dictionary<int,{sheetName}>();");
			sb.AppendLine("			foreach(var item in dataList)");
			sb.AppendLine("			{");
			sb.AppendLine("				dataDict.Add(item.ID,item);");
			sb.AppendLine("			}");
			sb.AppendLine("		}");
			sb.AppendLine("		private class Wrapper");
			sb.AppendLine("		{");
			sb.AppendLine($"			public List<{sheetName}> datas;");
			sb.AppendLine("		}");
			sb.AppendLine();
			sb.AppendLine($"		public {sheetName} Get(int id)");
			sb.AppendLine("		{");
			sb.AppendLine("			if(dataDict.ContainsKey(id))");
			sb.AppendLine("			{");
			sb.AppendLine("				return dataDict[id];");
			sb.AppendLine("			}");
			sb.AppendLine("			return null;");
			sb.AppendLine("		}");
			sb.AppendLine();
			sb.AppendLine($"		public List<{sheetName}> GetAllData()");
			sb.AppendLine("		{");
			sb.AppendLine("			return dataList;");
			sb.AppendLine("		}");
			sb.AppendLine();
			sb.AppendLine("}");
			savePath = Path.Combine(savePath, $"{sheetName}Loader.cs");
			File.WriteAllText(savePath, sb.ToString());
		}

		public static void WriteEnum(string sheetName, List<List<string>> sheetDatas, string savePath)
		{
			StringBuilder sb = new StringBuilder();
			enumMappings = new Dictionary<string, Dictionary<string, int>>();
			for (int row = 0; row < sheetDatas.Count; row++)
			{
				enumMappings.Add(sheetDatas[row][0], new Dictionary<string, int>());
				sb.AppendLine($"public enum {sheetDatas[row][0]}");
				sb.AppendLine("{");
				for (int colum = 1; colum < sheetDatas[row].Count; colum++)
				{
					sb.AppendLine($"		{sheetDatas[row][colum]},");
					enumMappings[sheetDatas[row][0]].Add(sheetDatas[row][colum], colum - 1);
				}
				sb.AppendLine("}");
			}
			savePath = Path.Combine(savePath, $"{sheetName}.cs");
			File.WriteAllText(savePath, sb.ToString());
		}

		public static void WriteJson(string sheetName, List<List<string>> sheetDatas, string savePath)
		{
			List<string> fieldName = sheetDatas[0];
			List<string> fieldType = sheetDatas[1];
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("\"datas\" : ");
			sb.Append("[");
			for(int row = 2; row < sheetDatas.Count; row++)
			{
				sb.Append("{");
				for (int colume = 0; colume < fieldName.Count; colume++)
				{
					string header = fieldName[colume];
					string type = fieldType[colume];
					if (type.StartsWith("List<"))
					{
						type = type.Substring(5, type.Length - 6);
						sb.Append($"\"{header}\" : ");
						sb.Append("[");
						if (type == "string")
						{
							List<string> list = ConvertValue(sheetDatas[row][colume], type, fieldName[colume]) as List<string>;
							for(int i = 0; i < list.Count; i++)
							{
								sb.Append($"\"{list[i].ToString()}\",");
							}
							
						}
						else if (type == "int")
						{
							List<int> list = ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]) as List<int>;
							for (int i = 0; i < list.Count; i++)
							{
								sb.Append($"{list[i].ToString()},");
							}
						}
						else if(type == "float")
						{
							List<float> list = ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]) as List<float>;
							for (int i = 0; i < list.Count; i++)
							{
								sb.Append($"{list[i].ToString()},");
							}
						}
						else if (type == "double")
						{
							List<double> list = ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]) as List<double>;
							for (int i = 0; i < list.Count; i++)
							{
								sb.Append($"{list[i].ToString()},");
							}
						}
						else if (type == "bool")
						{
							List<bool> list = ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]) as List<bool>;
							for (int i = 0; i < list.Count; i++)
							{
								sb.Append($"{list[i].ToString()},");
							}
						}
						//else if (type == "Vector3")
						//{
						//	List<Vector3> list = ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]) as List<Vector3>;
						//	for (int i = 0; i < list.Count; i++)
						//	{
						//		sb.Append($"{list[i].ToString().Replace("(","{").Replace(")","}")},");
						//	}
						//}
						//else if (type.StartsWith("Enum<"))
						//{
						//	header = header.Substring(5, header.Length - 6);
						//	List<int> list = ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]) as List<int>;
						//	for (int i = 0; i < list.Count; i++)
						//	{
						//		sb.Append($"{list[i].ToString()},");
						//	}
						//}
						sb.Remove(sb.Length - 1, 1);
						sb.Append("],");
					}
					else if (type.StartsWith("Enum<"))
					{
						header = header.Substring(5, header.Length - 6);
						sb.Append($"\"{header}\" : ");
						int value = (int)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
						sb.Append($"{value},");
					}
					else if (type == "int")
					{
						sb.Append($"\"{header}\" : ");
						int value = (int)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
						sb.Append($"{value},");
					}
					else if (type == "float")
					{
						sb.Append($"\"{header}\" : ");
						float value = (float)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
						sb.Append($"{value},");
					}
					else if (type == "double")
					{
						sb.Append($"\"{header}\" : ");
						double value = (double)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
						sb.Append($"{value},");
					}
					else if (type == "bool")
					{
						sb.Append($"\"{header}\" : ");
						bool value = (bool)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
						sb.Append($"{value},");
					}
					else if (type == "string")
					{
						sb.Append($"\"{header}\" : ");
						string value = (string)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
						sb.Append($"\"{value}\",");
					}
					//else if (type == "Vector3")
					//{
					//	sb.Append($"\"{header}\" : ");
					//	Vector3 value = (Vector3)ConvertValue(sheetDatas[row][colume], fieldType[colume], fieldName[colume]);
					//	sb.Append($"{value},");
					//}
					else
					{
						throw new Exception($"Unsupported data type: {fieldType[colume]}");
					}
				}
				sb.Remove(sb.Length - 1, 1);
				sb.Append("},");
			}
			sb.Remove(sb.Length - 1, 1);
			sb.Append("]");
			sb.Append("}");
			
			savePath = Path.Combine(savePath, $"{sheetName}.json");
			File.WriteAllText(savePath, sb.ToString());
		}

	

		private static object ConvertValue(string value, string fieldType, string fieldName)
		{

			// 빈 칸일 경우 기본값 설정
			if (string.IsNullOrWhiteSpace(value))
			{
				// 리스트 타입에서 빈 칸을 허용하지 않도록 메시지 출력
				if (fieldType.StartsWith("List<"))
				{
					throw new Exception($"리스트 타입에 빈칸 있음!");
				}

				// Enum 타입에서 빈 칸을 허용하지 않도록 메시지 출력
				if (fieldType.StartsWith("Enum<"))
				{
					throw new Exception($"리스트 타입에 빈칸 있음!!");
				}

				// 타입에 따른 기본값 처리
				switch (fieldType)
				{
					case "int":
						return 0;  // 기본값 0
					case "float":
						return 0.0f;  // 기본값 0.0f
					case "double":
						return 0.0;  // 기본값 0.0
					case "bool":
						return false;  // 기본값 false
					case "string":
						return "";  // 기본값 빈 문자열
					case "Vector3":
						return new Vector3(0, 0, 0);
					default:
						throw new Exception($"Unsupported data type: {fieldType}");
				}
			}

			if (fieldType.StartsWith("List<Enum<"))
			{
				string enumName = fieldType.Substring(10, fieldType.Length - 12);
				return value.Split(",").Select(v => { return int.Parse(v.Trim()); }).ToList();
			}
			else if (fieldType.StartsWith("List<"))
			{
				string itemType = fieldType.Substring(5, fieldType.Length - 6);
				if (itemType == "int")
				{
					return value.Split(',').Select(int.Parse).ToList();
				}
				else if (itemType == "float")
				{
					return value.Split(',').Select(float.Parse).ToList();
				}
				else if (itemType == "double")
				{
					return value.Split(',').Select(double.Parse).ToList();
				}
				else if (itemType == "bool")
				{
					return value.Split(',').Select(bool.Parse).ToList();
				}
				else if (itemType == "Vector3")
				{
					if (value.StartsWith("{") && value.EndsWith("}"))
					{
						value = value.Substring(1, value.Length - 2);
						string[] splitedValue = value.Split("},{");
						List<Vector3> vectorList = new List<Vector3>();
						foreach (string v in splitedValue)
						{
							string[] values = v.Split(",");
							Vector3 v3 = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
							vectorList.Add(v3);
						}
						return vectorList;
					}
					throw new Exception("잘못된 벡터 리스트 입니다");
				}
				else
				{
					return value.Split(',').Select(v => v.Trim()).ToList();
				}
			}
			else if (fieldType == "int")
			{
				return int.Parse(value);
			}
			else if (fieldType == "float")
			{
				return float.Parse(value);
			}
			else if (fieldType == "double")
			{
				return double.Parse(value);
			}
			else if (fieldType == "bool")
			{
				return bool.Parse(value);
			}
			else if (fieldType.StartsWith("Enum<"))
			{
				string enumType = fieldType.Substring(5, fieldType.Length - 6);
				return enumMappings[enumType][value];
			}
			else if (fieldType == "string")
			{
				return value;
			}
			else if(fieldType == "Vector3")
			{
				string[] splited = value.Split(",");
				Vector3 vec3 = new Vector3(int.Parse(splited[0]), int.Parse(splited[1]), int.Parse(splited[2]));
				return vec3;
			}
			else
			{
				throw new Exception($"Unsupported data type: {fieldType}");
			}
		}
	}
}