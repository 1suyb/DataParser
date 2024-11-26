using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace JsonLoader.FromGoogleSheet
{
	public class GoogleSheetParser
	{
		private string sheetNameJson;
		private string SheetDataJson;
		public static async void Parse(string sheetID, string apiKey, string enumTableName = "Enums", string savePath = "Assets")
		{
			List<string> sheetNames = await GetSheetNames(sheetID, apiKey);
			await GetSheetDatas(sheetID, sheetNames[0], apiKey);
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
				Debug.Log(sheetName.properties.title);
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

				string responseJson = await response.Content.ReadAsStringAsync();
				Debug.Log(responseJson);
				List<List<string>> values = ParseValues(responseJson);
				foreach(var item in values[0])
				{
					Debug.Log(item);
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
		public static List<List<string>> ParseValues(string json)
		{
			var values = new List<List<string>>();

			// "values": 부분 찾기
			int valuesStart = json.IndexOf("\"values\": [");
			if (valuesStart == -1)
			{
				throw new Exception("Cannot find 'values' key in JSON.");
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
				throw new Exception("Cannot find array brackets for 'values'.");
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
	}
}
