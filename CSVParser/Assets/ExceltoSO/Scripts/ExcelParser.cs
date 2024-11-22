using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

namespace ScriatableObjectBuilderFromExcel
{
    public class ExcelParser
    {
        public static void Parse(string enumTableName = "Enums")
        {
            string filePath = "Assets/ExceltoSO/TestExcel.xlsx";
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();

                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        if (result.Tables[i].TableName == enumTableName)
                        {
                            WriteEnum(result.Tables[i]);
                        }
                        else
                        {
                            WriteDataClass(result.Tables[i]);
                        }
                    }
                    /*Debug.Log(result.Tables[0].TableName);
                    //시트 개수만큼 반복
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        //해당 시트의 행데이터(한줄씩)로 반복
                        for (int j = 0; j < result.Tables[i].Rows.Count; j++)
                        {
                            for (int k = 0; k < result.Tables[i].Rows[j].ItemArray.Length; k++)
                            {
                                Debug.Log(result.Tables[i].Rows[j].ItemArray[k].ToString());
                            }
                            Debug.Log("--------------------------");
                            /#1#/해당행의 0,1,2 셀의 데이터 파싱
                            string data1 = result.Tables[i].Rows[j][0].ToString();
                            string data2 = result.Tables[i].Rows[j][1].ToString();
                            string data3 = result.Tables[i].Rows[j][2].ToString();
                            Debug.Log(data1);
                            Debug.Log(data2);
                            Debug.Log(data3);#1#
                        }
                    }*/
                }
            }
        }
        public static void WriteDataClass(DataTable sheet)
        {
            Debug.Log(sheet.TableName);
        }

        public static void WriteEnum(DataTable sheet)
        {
            Debug.Log("Im ExcelParser to Enum");
            Debug.Log(sheet.TableName);
        }

        public static void WriteLoader()
        {
            
        }
    }

}
