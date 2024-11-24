using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using ExcelDataReader;
using Unity.VisualScripting;
using UnityEngine;

namespace SOLoader.FromExcel
{
    public class ExcelDataLoader
    {
        public static List<T> Load<T>(string filePath) where T : new()
        {
            List<T> data = new List<T>();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();
                    Type targetClass = typeof(T);
                    string className = targetClass.Name;
                    int sheetIndex = -1;
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        if (result.Tables[i].TableName == className)
                        {
                            sheetIndex = i;
                            break;
                        }
                    }
                    if (sheetIndex == -1)
                    {
                        throw new WrongExcel("잘못된 ExcelFile입니다");
                    }
                    DataTable table = result.Tables[sheetIndex];
                    DataRow fieldNames = table.Rows[0];
                    DataRow fileTypes = table.Rows[1];
                    int columLength = fieldNames.ItemArray.Length;
                    
                    for (int i = 2; i < table.Rows.Count; i++)
                    {
                        T obj = new T();
                        for (int j = 0; j < columLength; j++)
                        {
                            string fieldName = fieldNames.ItemArray[j].ToString().Trim();
                            string value = table.Rows[i].ItemArray[j].ToString().Trim();
                            FieldInfo fieldInfo = targetClass.GetField(fieldName);
                            if (fieldInfo != null)
                            {
                                object convertedValue;

                                if (fieldInfo.FieldType.IsEnum)
                                {
                                    if (value == "")
                                    {
                                        convertedValue = fieldInfo.FieldType.Default();
                                    }
                                    else
                                    {
                                        convertedValue = Enum.Parse(fieldInfo.FieldType, value);
                                    }
                                }
                                else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    Type itemType = fieldInfo.FieldType.GetGenericArguments()[0]; // List의 요소 타입
                                    var list = (IList)Activator.CreateInstance(fieldInfo.FieldType); // List 객체 생성

                                    string[] values = value.Split(','); // 예시로 ','로 구분된 문자열을 리스트로 변환

                                    foreach (string item in values)
                                    {
                                        object itemValue = Convert.ChangeType(item.Trim(), itemType); // 각 요소를 변환
                                        list.Add(itemValue);
                                    }

                                    convertedValue = list;
                                }
                                else
                                {
                                    if (value == "")
                                    {
                                        convertedValue = fieldInfo.FieldType.Default();
                                    }
                                    else
                                    {
                                        convertedValue = Convert.ChangeType(value, fieldInfo.FieldType);
                                    }
                                    
                                }

                                fieldInfo.SetValue(obj, convertedValue);
                            }
                        }
                        data.Add(obj);
                    }
                }
            }

            return data;
        }
    }
}

