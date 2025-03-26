using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace ExceltoJson
{
    public class ExcelToJson
    {
        private string _folderPath;
        private string _jsonSavePath;
        private string _classSavePath;
        private string _enumFileName;
        
        
        private Dictionary<string,Dictionary<string,int>> _enumData;

        public ExcelToJson(string folderPath,string jsonSavePath, string classSavePath, string enumFileName = "InfoEnum")
        {
            _folderPath = folderPath;
            _jsonSavePath = jsonSavePath;
            _classSavePath = classSavePath;
            _enumFileName = enumFileName;
        }

        public void CreateClass(List<string> datapaths)
        {
            if (CheckPath(_classSavePath))
            {
                CreateFolder(_classSavePath);
            }

            if (CheckPath(_jsonSavePath))
            {
                CreateFolder(_jsonSavePath);
            }
            
            foreach(string csvfilepath in datapaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(csvfilepath);
                string classSavePath = ClassSaveFilePath(fileName);
                string jsonSavePath = JsonSaveFilePath(fileName);
                if(fileName == _enumFileName)
                {
                    ProcessingEnum(csvfilepath,classSavePath,jsonSavePath);
                }
            }

            foreach (var filePath in datapaths)
            {
                if (filePath.Contains("meta"))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string classSavePath = ClassSaveFilePath(fileName);
                string jsonSavePath = JsonSaveFilePath(fileName);
                if(fileName != _enumFileName)
                {
                    ProcessingClass(filePath,classSavePath,jsonSavePath);
                }
            }
            Debug.Log("Create Class Complete");
        }

        public void CreateJson(List<string> datapaths)
        {
            if (CheckPath(_classSavePath))
            {
                CreateFolder(_classSavePath);
            }

            if (CheckPath(_jsonSavePath))
            {
                CreateFolder(_jsonSavePath);
            }
            
            foreach(string csvfilepath in datapaths)
            {
                if (csvfilepath.Contains("meta"))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(csvfilepath);
                string classSavePath = ClassSaveFilePath(fileName);
                string jsonSavePath = JsonSaveFilePath(fileName);
                if(fileName == _enumFileName)
                {
                    ProcessingEnum(csvfilepath,classSavePath,jsonSavePath);
                }
            }

            foreach (var filePath in datapaths)
            {
                if (filePath.Contains("meta"))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string classSavePath = ClassSaveFilePath(fileName);
                string jsonSavePath = JsonSaveFilePath(fileName);
                if(fileName != _enumFileName)
                {
                    ProcessingJson(filePath,jsonSavePath);
                }
            }
            Debug.Log("Create Json Complete");
        }
        
        public void Convert()
        {
            string[] dataFiles = GetDataFiles(_folderPath);
            if (CheckPath(_classSavePath))
            {
                CreateFolder(_classSavePath);
            }

            if (CheckPath(_jsonSavePath))
            {
                CreateFolder(_jsonSavePath);
            }
            
            foreach(string csvfilepath in dataFiles)
            {
                if (csvfilepath.Contains("meta"))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(csvfilepath);
                string classSavePath = ClassSaveFilePath(fileName);
                string jsonSavePath = JsonSaveFilePath(fileName);
                if(fileName == _enumFileName)
                {
                    Processing(csvfilepath,classSavePath,jsonSavePath,true);
                }
            }

            foreach (var filePath in dataFiles)
            {
                if (filePath.Contains("meta"))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string classSavePath = ClassSaveFilePath(fileName);
                string jsonSavePath = JsonSaveFilePath(fileName);
                if(fileName != _enumFileName)
                {
                    Processing(filePath,classSavePath,jsonSavePath,false);
                }
            }
            Debug.Log("Convert Complete");
        }
        private string ClassSaveFilePath(string fileName) => Path.Combine(_classSavePath, $"{fileName}.cs");
        private string JsonSaveFilePath(string fileName) => Path.Combine(_jsonSavePath, $"{fileName}.json");
        private bool CheckPath(string folderPath) => Directory.Exists(folderPath);
        private void CreateFolder(string folderPath) => Directory.CreateDirectory(folderPath);
        private string[] GetDataFiles(string folderPath)
        {
            if (CheckPath(folderPath))
            {
                CreateFolder(folderPath);
            }
            List<string> csvFiles = new List<string>();
            string[] files = Directory.GetFiles(folderPath);
            return files;
        }

        private void ProcessingEnum(string filePath, string classSavePath, string jsonSavePath)
        {
            var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            try
            {
                DataSet result = reader.AsDataSet();
                ConvertToEnum(result.Tables[0], classSavePath);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                reader.Close();
                stream.Close();
            }
        }

        private void ProcessingClass(string filePath, string classSavePath, string jsonSavePath)
        {
            var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            try
            {
                DataSet result = reader.AsDataSet();
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                ConvertToClass(fileName, result.Tables[0], classSavePath, jsonSavePath);
                
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                reader.Close();
                stream.Close();
            }
        }

        private void ProcessingJson(string filePath, string jsonSavePath)
        {
            var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            try
            {
                DataSet result = reader.AsDataSet();
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                ConvertToJson(fileName, result.Tables[0], jsonSavePath);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                reader.Close();
                stream.Close();
            }
        }

        private void Processing(string filePath, string classSavePath, string jsonSavePath, bool isEnum = false)
        {
            var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {

                var reader = ExcelReaderFactory.CreateReader(stream);
                DataSet result = reader.AsDataSet();

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (isEnum)
                {
                    ConvertToEnum(result.Tables[0], classSavePath);
                }
                else
                {
                    ConvertToClass(fileName, result.Tables[0], classSavePath, jsonSavePath);
                    ConvertToJson(fileName, result.Tables[0], jsonSavePath);
                }

                reader.Close();

            }
            catch (IOException e)
            {
                Debug.LogError(e);
                
            }
            finally
            {
                stream.Close();
            }
            

        }

        private void ConvertToEnum(DataTable file, string savePath)
        {
            _enumData = new Dictionary<string, Dictionary<string, int>>();
            StringBuilder sb = new StringBuilder();
            for(int i = 0 ; i<file.Rows.Count;i++)
            {
                DataRow data = file.Rows[i];
                _enumData.Add(data.ItemArray[0].ToString(),new Dictionary<string, int>());
                sb.AppendLine($"public enum {data.ItemArray[0]}");
                sb.AppendLine("{");
                for(int j = 1; j<data.ItemArray.Length;j++)
                {
                    if(data.ItemArray[j].ToString() == "")
                    {
                        continue;
                    }
                    if(!_enumData[data.ItemArray[0].ToString()].ContainsKey(data.ItemArray[j].ToString()))
                    {
                        _enumData[data.ItemArray[0].ToString()].Add(data.ItemArray[j].ToString(),j-1);
                        sb.AppendLine($"{data.ItemArray[j]} = {j-1},");
                    }
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }
            File.WriteAllText(savePath,sb.ToString());
        }

        private void ConvertToClass(string className, DataTable file, string savePath, string jsonSavePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine($"public class {className} : LoadedInfoBase");
            sb.AppendLine("{");
            DataRow headers = file.Rows[0];
            DataRow types = file.Rows[1];
            DataRow description = file.Rows[2];
            if (headers.ItemArray.Length != types.ItemArray.Length)
            {
                Debug.LogError($"{className} 파일의 Header와 Type의 길이가 다릅니다.");
                return;
            }
            for (int i = 0; i < headers.ItemArray.Length; i++)
            {
                string type = types.ItemArray[i].ToString();
                if (headers[i].ToString() == "ID" || headers[i].ToString().Contains("*")) // *표시가 있는 열은 무시
                {
                    continue;
                }
                else if (type.Contains("List<Enum<"))
                {
                    type = type.Substring(10, type.Length - 12);
                    if (!_enumData.ContainsKey(type))
                    {
                        Debug.LogError($"{className} 파일의 {type} enum 데이터가 없습니다.");
                        return;
                    }
                    type = $"List<{type}>";
                }
                else if (type.Contains("Enum<"))
                {
                    type = type.Substring(5, type.Length - 6);
                    if (!_enumData.ContainsKey(type))
                    {
                        Debug.LogError($"{className} 파일의 {type} enum 데이터가 없습니다.");
                        return;
                    }
                }
                sb.AppendLine("     /// <summary>");
                sb.AppendLine("     /// " + description[i]);
                sb.AppendLine("     /// </summary>");
                sb.AppendLine($"    public {type} {headers[i]};");
            }
            sb.AppendLine("}");
            sb.AppendLine();
            File.WriteAllText(savePath,sb.ToString());
        }
        private void ConvertToJson(string className, DataTable file, string savePath)
        {
            StringBuilder sb = new StringBuilder();
            DataRow headers = file.Rows[0];
            DataRow types = file.Rows[1];
            DataRow description = file.Rows[2];
            sb.AppendLine("{");
            sb.AppendLine($"\t\"items\" : [");
            for (int i = 3; i < file.Rows.Count; i++)
            {
                DataRow data = file.Rows[i];

                if(data.ItemArray.Length != headers.ItemArray.Length)
                {
                    Debug.LogError($"{className} 파일의 {i}번째 줄이 데이터와 Header의 길이가 다릅니다.");
                    return;
                }
                sb.AppendLine("\t{");
                for (int j = 0; j < headers.ItemArray.Length; j++)
                {
                    if (headers[j].ToString().Contains("*"))    // *표시가 있는 열은 무시
                    {
                        continue;
                    }
                    string type = types[j].ToString();
                    if (type.Contains("List<"))
                    {
                        sb.AppendLine($"\t\t\"{headers[j]}\" : [");
                        string[] listData = data[j].ToString().Split(',');
                        if (type.Contains("Enum<"))
                        {
                            type = type.Substring(10, type.Length - 12);
                            if (!_enumData.ContainsKey(type))
                            {
                                Debug.LogError($"{className} 파일의 {type}enum 데이터가 없습니다.");
                                return;
                            }

                            for (int k = 0; k < listData.Length; k++)
                            {
                                if (!_enumData[type].ContainsKey(listData[k]))
                                {
                                    Debug.LogError($"{className} 파일의 {listData[k]} enum 데이터가 없습니다.");
                                    return;
                                }
                                sb.Append($"\t\t\t{_enumData[type][listData[k]]}");
                                if (k != listData.Length - 1)
                                {
                                    sb.Append(",\n");
                                }
                                else
                                {
                                    sb.Append("\n");
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < listData.Length; k++)
                            {
                                sb.Append($"\t\t\t{listData[k]}");
                                if (k != listData.Length - 1)
                                {
                                    sb.Append(",\n");
                                }
                                else
                                {
                                    sb.Append("\n");
                                }
                            }
                        }
                        sb.Append("\t\t]");
                    }
                    else if (type.Contains("Enum<"))
                    {
                        type = type.Substring(5, type.Length - 6);
                        if (!_enumData.ContainsKey(type))
                        {
                            Debug.LogError($"{className} 파일의 {type[j]} enum 데이터가 없습니다.");
                            return;
                        }

                        sb.Append($"\t\t\"{headers[j]}\" : {_enumData[type][data[j].ToString()]}");
                    }
                    else if (type.Contains("Vector3"))
                    {
                        string[] vectorData = data[j].ToString().Split(',');
                        sb.AppendLine($"\t\t\"{headers[j]}\" : {{");
                        sb.AppendLine($"\t\t\t\"x\" : {vectorData[0]},");
                        sb.AppendLine($"\t\t\t\"y\" : {vectorData[1]},");
                        sb.AppendLine($"\t\t\t\"z\" : {vectorData[2]}");
                        sb.Append("\t\t}");
                    }
                    else if (type.Contains("Vector2"))
                    {
                        string[] vectorData = data[j].ToString().Split(',');
                        sb.AppendLine($"\t\t\"{headers[j]}\" : {{");
                        sb.AppendLine($"\t\t\t\"x\" : {vectorData[0]},");
                        sb.AppendLine($"\t\t\t\"y\" : {vectorData[1]}");
                        sb.Append("\t\t\t}");
                    }
                    else if (type.Contains("bool"))
                    {
                        sb.Append($"\t\t\"{headers[j]}\" : {data[j].ToString().ToLower()}");
                    }
                    else if (type.Contains("string"))
                    {
                        sb.Append($"\t\t\"{headers[j]}\" : \"{data[j]}\"");
                    }
                    else
                    {
                        sb.Append($"\t\t\"{headers[j]}\" : {data[j]}");
                    }

                    if (j != headers.ItemArray.Length - 1)
                    {
                        sb.Append(",\n");
                    }
                    else
                    {
                        sb.Append("\n");
                    }
                }
                sb.Append("\t\t}");
                if(i!=file.Rows.Count-1)
                {
                    sb.Append(",\n");
                }
                else
                {
                    sb.Append("\n");
                }
            }
            sb.AppendLine($"\t]");
            sb.AppendLine("}");
            File.WriteAllText(savePath,sb.ToString());
        }
    }
}
